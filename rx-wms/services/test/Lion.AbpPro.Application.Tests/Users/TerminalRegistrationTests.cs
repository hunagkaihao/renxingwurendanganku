using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lion.AbpPro.ConfigurationOptions;
using Lion.AbpPro.Users;
using Lion.AbpPro.Users.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Xunit;

namespace Lion.AbpPro.Tests.Users
{
    /// <summary>使用真实注册服务和可控设备响应验证授权、账号创建及绑定启用边界，无需连接硬件或业务数据库。</summary>
    public class TerminalRegistrationTests
    {
        /// <summary>创建已登录但没有管理员角色的会话不能调用人员注册。</summary>
        [Fact]
        public async Task Ordinary_User_Cannot_Register()
        {
            var fixture = new RegistrationFixture();
            fixture.CurrentUser.IsInRole("admin").Returns(false);
            await Assert.ThrowsAsync<AbpAuthorizationException>(() => fixture.Service.CheckAccessAsync());
            await Assert.ThrowsAsync<AbpAuthorizationException>(() => fixture.Service.CreateUserAsync(new IdentityUserCreateDto()));
            await Assert.ThrowsAsync<AbpAuthorizationException>(() => fixture.Service.EnrollAsync(fixture.User.Id, fixture.FaceInput));
            Assert.Empty(fixture.Bindings);
            Assert.Equal(0, fixture.Handler.RequestCount);
        }

        /// <summary>入口要求登录及用户查看、编辑权限；管理员角色由普通账号拒绝用例验证，新账号额外要求创建权限。</summary>
        [Fact]
        public void Registration_Requires_Admin_And_Identity_Permissions()
        {
            var attributes = typeof(TerminalRegistrationAppService).GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Cast<AuthorizeAttribute>().ToList();
            Assert.Contains(attributes, attribute => attribute.Roles == null && attribute.Policy == null);
            Assert.Contains(attributes, attribute => attribute.Policy == IdentityPermissions.Users.Default);
            Assert.Contains(attributes, attribute => attribute.Policy == IdentityPermissions.Users.Update);
            var create = typeof(TerminalRegistrationAppService).GetMethod(nameof(TerminalRegistrationAppService.CreateUserAsync));
            Assert.Contains(create.GetCustomAttributes(typeof(AuthorizeAttribute), true).Cast<AuthorizeAttribute>(),
                attribute => attribute.Policy == IdentityPermissions.Users.Create);
        }

        /// <summary>新账号沿用已有用户服务，必须分配角色，且创建过程不触发设备录入。</summary>
        [Fact]
        public async Task New_Person_Uses_Existing_Account_Creation()
        {
            var fixture = new RegistrationFixture();
            await Assert.ThrowsAsync<UserFriendlyException>(() => fixture.Service.CreateUserAsync(new IdentityUserCreateDto()));
            var input = new IdentityUserCreateDto { UserName = "operator", Password = "Example123!", Email = "operator@example.com", RoleNames = new[] { "operator" } };
            fixture.Users.CreateAsync(input).Returns(new IdentityUserDto { Id = fixture.User.Id, UserName = input.UserName });
            var result = await fixture.Service.CreateUserAsync(input);
            Assert.Equal(fixture.User.Id, result.Id);
            await fixture.Users.Received(1).CreateAsync(input);
            Assert.Equal(0, fixture.Handler.RequestCount);
        }

        /// <summary>只有设备明确返回人脸成功码时才启用预留绑定。</summary>
        [Fact]
        public async Task Face_Success_Enables_Binding_After_Device_Confirmation()
        {
            var fixture = new RegistrationFixture { Response = "{\"errno\":0,\"msg\":\"录入成功\"}" };
            var result = await fixture.Service.EnrollAsync(fixture.User.Id, fixture.FaceInput);
            Assert.True(result.IsEnabled);
            Assert.Equal(101, result.FaceId);
            Assert.Equal(fixture.User.Id, result.UserId);
            Assert.True(fixture.Handler.BindingWasDisabledAtRequest);
            Assert.Contains("Face/FaceAdd", fixture.Handler.RequestUri);
            Assert.Contains("101", fixture.Handler.RequestBody);
            Assert.Equal(2, fixture.CompletedTransactions);
        }

        /// <summary>HTTP 成功但设备业务失败或缺少成功标记时，不允许启用登录。</summary>
        [Theory]
        [InlineData("{\"errno\":99,\"msg\":\"未检测到人脸\"}")]
        [InlineData("{}")]
        [InlineData("invalid-json")]
        public async Task Device_Failure_Leaves_Disabled_Reservation(string response)
        {
            var fixture = new RegistrationFixture { Response = response };
            await Assert.ThrowsAsync<UserFriendlyException>(() => fixture.Service.EnrollAsync(fixture.User.Id, fixture.FaceInput));
            Assert.Single(fixture.Bindings);
            Assert.False(fixture.Bindings[0].IsEnabled);
            Assert.Equal(1, fixture.CompletedTransactions);
        }

        /// <summary>设备连接失败时不释放编号给其他人员，也不自动重试请求。</summary>
        [Fact]
        public async Task Transport_Failure_Does_Not_Enable_Or_Retry()
        {
            var fixture = new RegistrationFixture();
            fixture.Handler.Failure = new HttpRequestException("offline");
            await Assert.ThrowsAsync<UserFriendlyException>(() => fixture.Service.EnrollAsync(fixture.User.Id, fixture.FaceInput));
            Assert.False(fixture.Bindings.Single().IsEnabled);
            Assert.Equal(1, fixture.Handler.RequestCount);
        }

        /// <summary>人脸自动分配新编号，旧客户端指定其他人员的编号也不会覆盖其特征。</summary>
        [Fact]
        public async Task Face_Number_Is_Automatically_Allocated_Without_Overwriting_Another_User()
        {
            var fixture = new RegistrationFixture();
            fixture.Bindings.Add(new UserBiometricBinding(Guid.NewGuid(), Guid.NewGuid(), null, 101, null, false));
            fixture.FaceInput.Identifier = 101;
            var result = await fixture.Service.EnrollAsync(fixture.User.Id, fixture.FaceInput);
            Assert.Equal(102, result.FaceId);
            Assert.Equal(fixture.User.Id, result.UserId);
            Assert.False(fixture.Bindings.Single(item => item.FaceId == 101).IsEnabled);
            Assert.NotEqual(fixture.User.Id, fixture.Bindings.Single(item => item.FaceId == 101).UserId);
            Assert.Contains("102", fixture.Handler.RequestBody);
        }

        /// <summary>跨租户目标人员即使具有有效 GUID，也不能被当前管理员录入。</summary>
        [Fact]
        public async Task User_From_Another_Tenant_Is_Rejected()
        {
            var fixture = new RegistrationFixture();
            fixture.Tenant.Id.Returns(Guid.NewGuid());
            await Assert.ThrowsAsync<UserFriendlyException>(() => fixture.Service.EnrollAsync(fixture.User.Id, fixture.FaceInput));
            Assert.Empty(fixture.Bindings);
            Assert.Equal(0, fixture.Handler.RequestCount);
        }

        /// <summary>指静脉必须完成六次采集且返回原编号，才允许启用绑定。</summary>
        [Theory]
        [InlineData(6, 202, true)]
        [InlineData(5, 202, false)]
        [InlineData(6, 999, false)]
        public async Task Vein_Requires_Six_Samples_And_Expected_Identifier(int samples, int returnedId, bool expectedSuccess)
        {
            var fixture = new RegistrationFixture
            {
                Response = $"{{\"success\":true,\"fingerId\":{returnedId},\"completedSamples\":{samples}}}"
            };
            var input = new EnrollUserBiometricInput { Method = "vein" };
            if (expectedSuccess)
            {
                var result = await fixture.Service.EnrollAsync(fixture.User.Id, input);
                Assert.True(result.IsEnabled);
                Assert.Equal(202, result.FingerId);
            }
            else await Assert.ThrowsAsync<UserFriendlyException>(() => fixture.Service.EnrollAsync(fixture.User.Id, input));
            Assert.Equal(expectedSuccess, fixture.Bindings.Single().IsEnabled);
            Assert.Contains("fingerId=202", fixture.Handler.RequestUri);
        }

        /// <summary>隔离服务依赖并记录预留事务和设备边界，执行真实应用服务方法。</summary>
        private sealed class RegistrationFixture
        {
            public readonly ICurrentUser CurrentUser = Substitute.For<ICurrentUser>();
            public readonly ICurrentTenant Tenant = Substitute.For<ICurrentTenant>();
            public readonly IUserAppService Users = Substitute.For<IUserAppService>();
            public readonly List<UserBiometricBinding> Bindings = new();
            public readonly IdentityUser User = new(Guid.NewGuid(), "operator", "operator@example.com");
            public readonly DeviceHandler Handler;
            public readonly TerminalRegistrationAppService Service;
            public readonly EnrollUserBiometricInput FaceInput = new() { Method = "face", ImageData = "aW1hZ2U=" };
            public int CompletedTransactions;
            public string Response { set => Handler.Response = value; }

            public RegistrationFixture()
            {
                CurrentUser.IsAuthenticated.Returns(true);
                CurrentUser.IsInRole("admin").Returns(true);
                var repository = Substitute.For<IIdentityUserRepository>();
                repository.FindAsync(User.Id, Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(User);
                var bindings = Substitute.For<IRepository<UserBiometricBinding, Guid>>();
                bindings.GetListAsync(Arg.Any<Expression<Func<UserBiometricBinding, bool>>>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
                    .Returns(call => Bindings.Where(call.Arg<Expression<Func<UserBiometricBinding, bool>>>().Compile()).ToList());
                bindings.InsertAsync(Arg.Any<UserBiometricBinding>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
                    .Returns(call => { var binding = call.Arg<UserBiometricBinding>(); Bindings.Add(binding); return binding; });
                bindings.GetAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
                    .Returns(call => Bindings.Single(item => item.Id == call.Arg<Guid>()));
                bindings.UpdateAsync(Arg.Any<UserBiometricBinding>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
                    .Returns(call => call.Arg<UserBiometricBinding>());
                var manager = Substitute.For<IUnitOfWorkManager>();
                manager.Begin(Arg.Any<AbpUnitOfWorkOptions>(), Arg.Any<bool>()).Returns(_ =>
                {
                    var work = Substitute.For<IUnitOfWork>();
                    work.CompleteAsync(Arg.Any<CancellationToken>()).Returns(_ => { CompletedTransactions++; return Task.CompletedTask; });
                    return work;
                });
                var clients = Substitute.For<IHttpClientFactory>();
                Handler = new DeviceHandler(Bindings);
                clients.CreateClient(Arg.Any<string>()).Returns(_ => new HttpClient(Handler, disposeHandler: false));
                var options = Substitute.For<IOptionsSnapshot<BiometricLoginOptions>>();
                options.Value.Returns(new BiometricLoginOptions { ServiceUrl = "http://device.local/", TimeoutSeconds = 30 });
                var biometricNumbers = Substitute.For<IBiometricNumberAllocator>();
                biometricNumbers.AllocateAsync("face").Returns(_ => Bindings.Where(item => item.FaceId.HasValue)
                    .Select(item => item.FaceId.Value).DefaultIfEmpty(100).Max() + 1);
                biometricNumbers.AllocateAsync("vein").Returns(_ => Bindings.Where(item => item.FingerId.HasValue)
                    .Select(item => item.FingerId.Value).DefaultIfEmpty(201).Max() + 1);
                Service = new TerminalRegistrationAppService(Users, Substitute.For<IIdentityRoleAppService>(), repository,
                    bindings, manager, clients, options, biometricNumbers);
                var filter = Substitute.For<IDataFilter>();
                filter.Disable<IMultiTenant>().Returns(Substitute.For<IDisposable>());
                var guid = Substitute.For<IGuidGenerator>();
                guid.Create().Returns(_ => Guid.NewGuid());
                Service.LazyServiceProvider = new AbpLazyServiceProvider(new ServiceCollection()
                    .AddSingleton(CurrentUser).AddSingleton(Tenant).AddSingleton(filter).AddSingleton(guid).BuildServiceProvider());
            }
        }

        /// <summary>记录 HTTP 请求到达时的绑定状态，模拟设备成功、业务失败或传输失败。</summary>
        private sealed class DeviceHandler : HttpMessageHandler
        {
            private readonly List<UserBiometricBinding> _bindings;
            public string Response = "{\"errno\":0}";
            public Exception Failure;
            public int RequestCount;
            public string RequestUri;
            public string RequestBody;
            public bool BindingWasDisabledAtRequest;
            public DeviceHandler(List<UserBiometricBinding> bindings) { _bindings = bindings; }
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                RequestCount++;
                RequestUri = request.RequestUri.ToString();
                RequestBody = request.Content == null ? "" : await request.Content.ReadAsStringAsync();
                BindingWasDisabledAtRequest = _bindings.Count > 0 && _bindings.All(binding => !binding.IsEnabled);
                if (Failure != null) throw Failure;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(Response, Encoding.UTF8, "application/json") };
            }
        }
    }
}
