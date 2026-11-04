# Graph Report - services  (2026-09-09)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 1710 nodes · 2643 edges · 126 communities (111 shown, 8 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 102 edges (avg confidence: 0.81)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `712e9f04`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Community 0
- Community 1
- Community 2
- Community 3
- Community 4
- Community 5
- Community 6
- Community 7
- Community 8
- Community 9
- Community 10
- Community 11
- Community 12
- Community 13
- Community 14
- Community 15
- Community 16
- Community 17
- Community 18
- Community 19
- Community 20
- Community 21
- Community 22
- Community 23
- Community 24
- Community 25
- Community 26
- Community 27
- Community 28
- Community 29
- Community 30
- Community 31
- Community 32
- Community 33
- Community 34
- Community 35
- Community 36
- Community 37
- Community 38
- Community 39
- Community 40
- Community 41
- Community 42
- Community 43
- Community 44
- Community 45
- Community 46
- Community 47
- Community 48
- Community 49
- Community 50
- Community 51
- Community 52
- Community 53
- Community 54
- Community 55
- Community 56
- Community 57
- Community 58
- Community 59
- Community 60
- Community 61
- Community 62
- Community 63
- Community 64
- Community 65
- Community 66
- Community 67
- Community 68
- Community 69
- Community 70
- Community 71
- Community 72
- Community 73
- Community 74
- Community 75
- Community 76
- Community 77
- Community 78
- Community 79
- Community 80
- Community 81
- Community 82
- Community 83
- Community 84
- Community 85
- Community 86
- Community 87
- Community 88
- Community 89
- Community 90
- Community 91
- Community 92
- Community 93
- Community 94
- Community 95
- Community 96
- Community 97
- Community 98
- Community 99
- Community 100
- Community 101
- Community 102
- Community 103
- Community 104
- Community 105
- Community 106
- Community 107
- Community 108
- Community 109
- Community 110
- Community 111
- Community 112
- Community 113
- Community 114
- Community 115
- Community 116
- Community 117
- Community 118

## God Nodes (most connected - your core abstractions)
1. `PagingClientListOutput` - 64 edges
2. `UpdataBasicDataInput` - 46 edges
3. `AbpProDbContext` - 43 edges
4. `GetAuditLogPageListOutput` - 25 edges
5. `ApiResourceOutput` - 24 edges
6. `Lion.AbpPro` - 24 edges
7. `Lion.AbpPro.IdentityServers.Clients.Dtos` - 23 edges
8. `PagingAuditLogListInput` - 19 edges
9. `AbpProDbMigrationService` - 19 edges
10. `LoginOutput` - 18 edges

## Surprising Connections (you probably didn't know these)
- `Login` --references--> `IAccountAppService`  [EXTRACTED]
  host/Lion.AbpPro.HttpApi.Host/Pages/Login.cshtml.cs → src/Lion.AbpPro.Application.Contracts/Users/IAccountAppService.cs
- `Login` --references--> `JwtOptions`  [EXTRACTED]
  host/Lion.AbpPro.HttpApi.Host/Pages/Login.cshtml.cs → src/Lion.AbpPro.Application.Contracts/ConfigurationOptions/JwtOptions.cs
- `IdentityServerClientAppService` --implements--> `IIdentityServerClientAppService`  [EXTRACTED]
  src/Lion.AbpPro.Application/IdentityServers/Clients/IdentityServerClientAppService.cs → src/Lion.AbpPro.Application.Contracts/IdentityServers/Clients/IIdentityServerClientAppService.cs
- `PagingClientListOutput` --references--> `ClientCorsOriginOutput`  [EXTRACTED]
  src/Lion.AbpPro.Application.Contracts/IdentityServers/Clients/Dtos/PagingClientListOutput.cs → src/Lion.AbpPro.Application.Contracts/IdentityServers/Clients/Dtos/ClientCorsOriginOutput.cs
- `PagingClientListOutput` --references--> `ClientGrantTypeOutput`  [EXTRACTED]
  src/Lion.AbpPro.Application.Contracts/IdentityServers/Clients/Dtos/PagingClientListOutput.cs → src/Lion.AbpPro.Application.Contracts/IdentityServers/Clients/Dtos/ClientGrantTypeOutput.cs

## Import Cycles
- None detected.

## Communities (126 total, 8 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.06
Nodes (41): Lion.AbpPro.Tenants.Dtos, Lion.AbpPro.Controllers.Tenants, Lion.AbpPro.Tenants, IAbpTenantAppService, ITenantAppService, FindTenantByNameInput, Name, PagingTenantInput (+33 more)

### Community 1 - "Community 1"
Cohesion: 0.05
Nodes (46): IdentityUserUpdateDto, IIdentityUserAppService, IIdentityUserRepository, Guid, LockUserInput, Locked, UserId, PagingUserListInput (+38 more)

### Community 2 - "Community 2"
Cohesion: 0.06
Nodes (36): AddCorsInput, ClientId, Origin, AddRedirectUriInput, ClientId, Uri, CreateClientInput, AllowedGrantTypes (+28 more)

### Community 3 - "Community 3"
Cohesion: 0.06
Nodes (42): IdentityRoleUpdateDto, IIdentityRoleAppService, IIdentityRoleRepository, PagingBase, PagingApiRseourceListInput, Filter, PagingApiScopeListInput, Filter (+34 more)

### Community 4 - "Community 4"
Cohesion: 0.05
Nodes (40): Lion.AbpPro.IdentityServer, Lion.AbpPro.IdentityServers.IdentityResources, Lion.AbpPro.IdentityServers.IdentityResources.Dtos, CreateIdentityResourceInput, Description, DisplayName, Emphasize, Enabled (+32 more)

### Community 5 - "Community 5"
Cohesion: 0.07
Nodes (31): AbpProDomainService, IIdentityResourceRepository, ApiScope, ApiScope, Authorize, FromSelector, IdInput, List (+23 more)

### Community 6 - "Community 6"
Cohesion: 0.05
Nodes (29): Lion.AbpPro.Data, Lion.AbpPro.DbMigrator, IDataSeeder, IHostApplicationLifetime, IHostedService, ITenantRepository, AbpBackgroundJobOptions, ServiceConfigurationContext (+21 more)

### Community 7 - "Community 7"
Cohesion: 0.04
Nodes (51): Guid, List, PagingClientListOutput, AbsoluteRefreshTokenLifetime, AccessTokenLifetime, AccessTokenType, AllowAccessTokensViaBrowser, AllowedCorsOrigins (+43 more)

### Community 8 - "Community 8"
Cohesion: 0.07
Nodes (32): Lion.AbpPro.Controllers.Systems, Lion.AbpPro.Roles.Dtos, Lion.AbpPro.Roles, IPermissionAppService, PermissionGrantInfoDto, PermissionGroupDto, GetPermissionInput, ProviderKey (+24 more)

### Community 9 - "Community 9"
Cohesion: 0.05
Nodes (43): UpdataBasicDataInput, AbsoluteRefreshTokenLifetime, AccessTokenLifetime, AccessTokenType, AllowAccessTokensViaBrowser, AllowedGrantTypes, AllowedIdentityTokenSigningAlgorithms, AllowOfflineAccess (+35 more)

### Community 10 - "Community 10"
Cohesion: 0.13
Nodes (14): Authorize, Client, IdInput, List, PagedResultDto, Task, IdentityServerClientAppService, CancellationToken (+6 more)

### Community 11 - "Community 11"
Cohesion: 0.06
Nodes (36): AbpDbContext, BackgroundJobRecord, DbSet, FeatureValue, IAuditLoggingDbContext, IBackgroundJobsDbContext, IdentityClaimType, IdentityLinkUser (+28 more)

### Community 12 - "Community 12"
Cohesion: 0.07
Nodes (26): Microsoft.AspNetCore.DataProtection.StackExchangeRedis ($(MicrosoftVersion)), NetEscapades.Configuration.Yaml (3.1.0), Serilog.AspNetCore ($(SerilogAspNetCoreVersion)), Serilog.Extensions.Logging ($(SerilogExtensionsLoggingVersion)), Serilog.Sinks.Async ($(SerilogSinksAsyncVersion)), Serilog.Sinks.Console ($(SerilogSinksConsoleVersion)), Serilog.Sinks.File ($(SerilogSinksFileVersion)), Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic ($(AbpPackageVersion)) (+18 more)

### Community 13 - "Community 13"
Cohesion: 0.15
Nodes (14): IGuidGenerator, IIdentityResourceDataSeeder, IPermissionDataSeeder, ApiResource, Client, DataSeedContext, IApiResourceRepository, IApiScopeRepository (+6 more)

### Community 14 - "Community 14"
Cohesion: 0.10
Nodes (21): DateTime, Guid, GetAuditLogPageListOutput, ApplicationName, BrowserInfo, ClientId, ClientIpAddress, ClientName (+13 more)

### Community 15 - "Community 15"
Cohesion: 0.17
Nodes (14): Guid, List, LoginOutput, Id, Name, Roles, Token, UserName (+6 more)

### Community 16 - "Community 16"
Cohesion: 0.12
Nodes (10): ApplicationService, Lion.AbpPro.Localization, Lion.AbpPro.MultiTenancy, IPermissionDefinitionContext, PermissionDefinitionProvider, AbpProAppService, LocalizableString, AbpProPermissionDefinitionProvider (+2 more)

### Community 17 - "Community 17"
Cohesion: 0.11
Nodes (17): Guid, List, ApiResourceOutput, AllowedAccessTokenSigningAlgorithms, Description, DisplayName, Enabled, Name (+9 more)

### Community 18 - "Community 18"
Cohesion: 0.18
Nodes (10): AbpAuditingOptions, AbpBundlingOptions, AbpBackgroundJobOptions, AbpDistributedCacheOptions, AbpLocalizationOptions, AbpVirtualFileSystemOptions, ApplicationInitializationContext, IConfiguration (+2 more)

### Community 19 - "Community 19"
Cohesion: 0.13
Nodes (11): Lion.AbpPro.Data.Seeds, IDataSeedContributor, IdentityRoleManager, DataSeedContext, ISettingManager, Task, AbpSettingDataSeedContributor, DataSeedContext (+3 more)

### Community 20 - "Community 20"
Cohesion: 0.14
Nodes (12): CanComplete, Cells, CheckAppService, CheckManager, Flag, ScheduledDemoJob, IDisposable, PlanManager (+4 more)

### Community 21 - "Community 21"
Cohesion: 0.13
Nodes (9): Lion.AbpPro, DefaultBrandingProvider, AbpProHttpApiHostConsts, AbpProBrandingProvider, AppName, Lion.AbpPro.Pages.Monitor, HttpClientNameConsts, AbpProConsts (+1 more)

### Community 22 - "Community 22"
Cohesion: 0.17
Nodes (10): Lion.AbpPro.ElasticSearchs.Providers, ISingletonDependency, IElasticClient, ElasticsearchBasicService, Client, IConfiguration, IElasticClient, ElasticsearchProvider (+2 more)

### Community 23 - "Community 23"
Cohesion: 0.28
Nodes (7): EnumSchemaFilter, HiddenAbpDefaultApiFilter, AbpBackgroundJobOptions, AbpDistributedCacheOptions, IConfiguration, ServiceConfigurationContext, AbpProHttpApiHostModule

### Community 24 - "Community 24"
Cohesion: 0.13
Nodes (11): AllocationRulesOptions, Enable, XBigToSmall, YBigToSmall, ZBigToSmall, ERPOptions, Enable, Server (+3 more)

### Community 25 - "Community 25"
Cohesion: 0.13
Nodes (15): HttpStatusCode, DateTime, PagingAuditLogListInput, ApplicationName, CorrelationId, EndTime, HasException, HttpMethod (+7 more)

### Community 26 - "Community 26"
Cohesion: 0.24
Nodes (8): List, Task, ISettingAppService, HttpPost, List, SwaggerOperation, Task, SettingController

### Community 27 - "Community 27"
Cohesion: 0.24
Nodes (9): Lion.AbpPro.Controllers.IdentityServers, FromSelector, HttpPost, IdInput, List, PagedResultDto, SwaggerOperation, Task (+1 more)

### Community 28 - "Community 28"
Cohesion: 0.15
Nodes (7): Lion.AbpPro.IdentityServers.ApiResources.Dtos, Lion.AbpPro.IdentityServers.ApiResources, Guid, ApiResourcePropertyOutput, ApiResourceId, Key, Value

### Community 29 - "Community 29"
Cohesion: 0.14
Nodes (10): ILogger, Login, Monitor, IHostEnvironment, PageModel, JwtOptions, Audience, ExpirationTime (+2 more)

### Community 30 - "Community 30"
Cohesion: 0.16
Nodes (12): IAbpLazyServiceProvider, IApplicationBuilder, IServiceProvider, IStringLocalizer, IStringLocalizerFactory, Type, LocalizationHelper, L (+4 more)

### Community 31 - "Community 31"
Cohesion: 0.22
Nodes (9): IdentityUser&gt;, IHttpClientFactory, IHttpContextAccessor, Guid, ICurrentTenant, IdentityUserManager, List, Task (+1 more)

### Community 32 - "Community 32"
Cohesion: 0.17
Nodes (8): AbpPageModel, Lion.AbpPro.Pages, ICurrentUser, IndexModel, Volo.Abp.Users, ICurrentUser, Welcome, IndexModel

### Community 33 - "Community 33"
Cohesion: 0.21
Nodes (7): AbpProController, Lion.AbpPro.ElasticSearchs, Lion.AbpPro.ElasticSearchs.Dto, IApplicationService, ILionAbpProLogAppService, ElasticSearchApplicationAutoMapperProfile, LionAbpProLogController

### Community 34 - "Community 34"
Cohesion: 0.19
Nodes (8): ElasticsearchSinkOptions, HttpContext, IConfiguration, Task, SerilogToEsExtensions, HttpRequest, HttpResponse, IDiagnosticContext

### Community 35 - "Community 35"
Cohesion: 0.15
Nodes (10): Microsoft.AspNetCore.Http.Abstractions ($(MicrosoftAspNetCoreHttpAbstractionsVersion)), Microsoft.Extensions.FileProviders.Embedded ($(MicrosoftVersion)), Volo.Abp.AuditLogging.Domain.Shared ($(AbpPackageVersion)), Volo.Abp.BackgroundJobs.Domain.Shared ($(AbpPackageVersion)), Volo.Abp.FeatureManagement.Domain.Shared ($(AbpPackageVersion)), Volo.Abp.Identity.Domain.Shared ($(AbpPackageVersion)), Volo.Abp.IdentityServer.Domain.Shared ($(AbpPackageVersion)), Volo.Abp.PermissionManagement.Domain.Shared ($(AbpPackageVersion)) (+2 more)

### Community 36 - "Community 36"
Cohesion: 0.17
Nodes (11): DateTime, PagingElasticSearchLogOutput, CreationTime, Level, Message, CustomePagedResultDto, Task, CustomePagedResultDto (+3 more)

### Community 37 - "Community 37"
Cohesion: 0.36
Nodes (7): ApiResource, CancellationToken, Guid, IApiResourceRepository, List, Task, IdenityServerApiResourceManager

### Community 38 - "Community 38"
Cohesion: 0.31
Nodes (7): HttpPost, IdInput, List, PagedResultDto, SwaggerOperation, Task, ApiResourceController

### Community 39 - "Community 39"
Cohesion: 0.18
Nodes (7): AbpExceptionLocalizationOptions, AbpLocalizationOptions, AbpVirtualFileSystemOptions, ServiceConfigurationContext, AbpProDomainSharedModule, OneTimeRunner, AbpProGlobalFeatureConfigurator

### Community 40 - "Community 40"
Cohesion: 0.17
Nodes (9): CookieOptions, Task, IValidatableObject, IEnumerable, LoginInput, Name, Password, ValidationContext (+1 more)

### Community 41 - "Community 41"
Cohesion: 0.17
Nodes (8): Lion.AbpPro.IdentityServers, Lion.AbpPro.Users.Mappers, Lion.AbpPro.AuditLogs.Mappers, Profile, AbpProApplicationAutoMapperProfile, AuditLogApplicationAutoMapperProfile, IdentityServerApplicationAutoMapperProfile, UserApplicationAutoMapperProfile

### Community 42 - "Community 42"
Cohesion: 0.20
Nodes (6): Lion.AbpPro.Jobs, ITransientDependency, Task, IRecurringJob, Task, TestJob

### Community 43 - "Community 43"
Cohesion: 0.17
Nodes (10): Volo.Abp.AuditLogging.Domain ($(AbpPackageVersion)), Volo.Abp.BackgroundJobs.Domain ($(AbpPackageVersion)), Volo.Abp.Emailing ($(AbpPackageVersion)), Volo.Abp.FeatureManagement.Domain ($(AbpPackageVersion)), Volo.Abp.Identity.Domain ($(AbpPackageVersion)), Volo.Abp.IdentityServer.Domain ($(AbpPackageVersion)), Volo.Abp.PermissionManagement.Domain.Identity ($(AbpPackageVersion)), Volo.Abp.PermissionManagement.Domain.IdentityServer ($(AbpPackageVersion)) (+2 more)

### Community 44 - "Community 44"
Cohesion: 0.17
Nodes (11): Microsoft.NET.Sdk.Web, Microsoft.AspNetCore.DataProtection.StackExchangeRedis ($(MicrosoftVersion)), Serilog.AspNetCore ($(SerilogAspNetCoreVersion)), Serilog.Sinks.Async ($(SerilogSinksAsyncVersion)), Volo.Abp.Account.Application ($(AbpPackageVersion)), Volo.Abp.Account.HttpApi ($(AbpPackageVersion)), Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic ($(AbpPackageVersion)), Volo.Abp.AspNetCore.Serilog ($(AbpPackageVersion)) (+3 more)

### Community 45 - "Community 45"
Cohesion: 0.17
Nodes (11): Microsoft.EntityFrameworkCore.Tools ($(MicrosoftVersion)), Volo.Abp.AuditLogging.EntityFrameworkCore ($(AbpPackageVersion)), Volo.Abp.BackgroundJobs.EntityFrameworkCore ($(AbpPackageVersion)), Volo.Abp.EntityFrameworkCore.MySQL ($(AbpPackageVersion)), Volo.Abp.FeatureManagement.EntityFrameworkCore ($(AbpPackageVersion)), Volo.Abp.Identity.EntityFrameworkCore ($(AbpPackageVersion)), Volo.Abp.IdentityServer.EntityFrameworkCore ($(AbpPackageVersion)), Volo.Abp.PermissionManagement.EntityFrameworkCore ($(AbpPackageVersion)) (+3 more)

### Community 46 - "Community 46"
Cohesion: 0.18
Nodes (10): Guid, UpdateCreateApiScopeInput, Description, DisplayName, Emphasize, Enabled, Id, Name (+2 more)

### Community 47 - "Community 47"
Cohesion: 0.29
Nodes (7): ApiResource, Authorize, IdInput, List, PagedResultDto, Task, ApiResourceAppService

### Community 48 - "Community 48"
Cohesion: 0.22
Nodes (6): AbpProAppService, Lion.AbpPro.AuditLogs, IAuditLogRepository, AuditLogAppService, IAuditLogAppService, AuditLogController

### Community 49 - "Community 49"
Cohesion: 0.20
Nodes (6): Lion.AbpPro.Settings.Dtos, Lion.AbpPro.Settings, Dictionary, UpdateSettingInput, Values, Volo.Abp.Settings

### Community 50 - "Community 50"
Cohesion: 0.18
Nodes (6): Lion.AbpPro.Extensions.Filters, DashboardContext, ApplicationInitializationContext, ICurrentUser, CustomHangfireAuthorizeFilter, IDashboardAuthorizationFilter

### Community 51 - "Community 51"
Cohesion: 0.18
Nodes (5): Lion.AbpPro.EntityFrameworkCore, IEfCoreDbContext, ModelBuilder, AbpProDbContextModelCreatingExtensions, IAbpProDbContext

### Community 52 - "Community 52"
Cohesion: 0.18
Nodes (10): DomainService, IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, AbpProDomainService, DistributedEventBus, ObjectMapper (+2 more)

### Community 53 - "Community 53"
Cohesion: 0.25
Nodes (7): ISettingDefinitionManager, ISettingManager, IStringLocalizer, IStringLocalizerFactory, List, Task, SettingAppService

### Community 54 - "Community 54"
Cohesion: 0.18
Nodes (10): Guid, LoginStsOutput, avatar, email, family_name, given_name, name, preferred_username (+2 more)

### Community 55 - "Community 55"
Cohesion: 0.20
Nodes (6): AbpAutoMapperOptions, AbpModule, ServiceConfigurationContext, AbpProApplicationModule, ServiceConfigurationContext, AbpProHttpApiClientModule

### Community 56 - "Community 56"
Cohesion: 0.36
Nodes (3): Lion.AbpPro.Users.Dtos, Lion.AbpPro.ConfigurationOptions, Lion.AbpPro.Users

### Community 57 - "Community 57"
Cohesion: 0.20
Nodes (10): EntityDto, Guid, PagingApiScopeListOutput, Description, DisplayName, Emphasize, Enabled, Name (+2 more)

### Community 58 - "Community 58"
Cohesion: 0.20
Nodes (8): Volo.Abp.Account.Application.Contracts ($(AbpPackageVersion)), Volo.Abp.FeatureManagement.Application.Contracts ($(AbpPackageVersion)), Volo.Abp.Identity.Application.Contracts ($(AbpPackageVersion)), Volo.Abp.ObjectExtending ($(AbpPackageVersion)), Volo.Abp.PermissionManagement.Application.Contracts ($(AbpPackageVersion)), Volo.Abp.SettingManagement.Application.Contracts ($(AbpPackageVersion)), Volo.Abp.TenantManagement.Application.Contracts ($(AbpPackageVersion)), Volo.Abp.AspNetCore.Mvc.Contracts ($(AbpPackageVersion))

### Community 59 - "Community 59"
Cohesion: 0.20
Nodes (8): NEST ($(NESTVersion)), Volo.Abp.FeatureManagement.Application ($(AbpPackageVersion)), Volo.Abp.Identity.Application ($(AbpPackageVersion)), Volo.Abp.PermissionManagement.Application ($(AbpPackageVersion)), Volo.Abp.SettingManagement.Application ($(AbpPackageVersion)), Volo.Abp.TenantManagement.Application ($(AbpPackageVersion)), Volo.Abp.Account.Application ($(AbpPackageVersion)), Volo.Abp.AspNetCore.Mvc.Contracts ($(AbpPackageVersion))

### Community 60 - "Community 60"
Cohesion: 0.20
Nodes (9): AGVOptions, CTUInTaskType, CTUOutTaskType, Enable, FloorEmptyTaskType, FloorTaskType, LiftEmptyTaskType, LiftTaskType (+1 more)

### Community 61 - "Community 61"
Cohesion: 0.20
Nodes (10): List, UpdateApiResourceInput, AllowedAccessTokenSigningAlgorithms, ApiScopes, Description, DisplayName, Enabled, Name (+2 more)

### Community 62 - "Community 62"
Cohesion: 0.29
Nodes (5): IdInput, List, PagedResultDto, Task, IApiResourceAppService

### Community 63 - "Community 63"
Cohesion: 0.27
Nodes (6): FromSelector, IdInput, List, PagedResultDto, Task, IApiScopeAppService

### Community 64 - "Community 64"
Cohesion: 0.33
Nodes (3): Lion.AbpPro.IdentityServers.Clients.Dtos, Lion.AbpPro.IdentityServers.Clients, Lion.AbpPro.Permissions

### Community 65 - "Community 65"
Cohesion: 0.22
Nodes (7): Volo.Abp.Account.HttpApi.Client ($(AbpPackageVersion)), Volo.Abp.FeatureManagement.HttpApi.Client ($(AbpPackageVersion)), Volo.Abp.Identity.HttpApi.Client ($(AbpPackageVersion)), Volo.Abp.PermissionManagement.HttpApi.Client ($(AbpPackageVersion)), Volo.Abp.SettingManagement.HttpApi.Client ($(AbpPackageVersion)), Volo.Abp.TenantManagement.HttpApi.Client ($(AbpPackageVersion)), Microsoft.NET.Sdk

### Community 66 - "Community 66"
Cohesion: 0.22
Nodes (7): Swashbuckle.AspNetCore.Annotations ($(SwashbuckleVersion)), Volo.Abp.FeatureManagement.HttpApi ($(AbpPackageVersion)), Volo.Abp.Identity.HttpApi ($(AbpPackageVersion)), Volo.Abp.PermissionManagement.HttpApi ($(AbpPackageVersion)), Volo.Abp.SettingManagement.HttpApi ($(AbpPackageVersion)), Volo.Abp.TenantManagement.HttpApi ($(AbpPackageVersion)), Volo.Abp.Account.HttpApi ($(AbpPackageVersion))

### Community 67 - "Community 67"
Cohesion: 0.25
Nodes (5): IConfigurationRoot, IDesignTimeDbContextFactory, OneTimeRunner, AbpProEfCoreEntityExtensionMappings, AbpProMigrationsDbContextFactory

### Community 68 - "Community 68"
Cohesion: 0.31
Nodes (5): ISettingDefinitionContext, SettingDefinition, SettingDefinitionProvider, LocalizableString, AbpProSettingDefinitionProvider

### Community 69 - "Community 69"
Cohesion: 0.22
Nodes (8): DateTime, Guid, ApiResourceSecretOutput, ApiResourceId, Description, Expiration, Type, Value

### Community 70 - "Community 70"
Cohesion: 0.29
Nodes (5): AbpController, ActionResult, Lion.AbpPro.Controllers, HomeController, AbpProController

### Community 71 - "Community 71"
Cohesion: 0.29
Nodes (3): Lion.AbpPro.Extensions, IHostBuilder, Program

### Community 72 - "Community 72"
Cohesion: 0.25
Nodes (6): Lion.AbpPro.Extensions.Hangfire, ElectStateContext, JobRetryLastFilter, RetryCount, IElectStateFilter, JobFilterAttribute

### Community 74 - "Community 74"
Cohesion: 0.25
Nodes (5): IApplicationBuilder, IServiceCollection, Startup, ILoggerFactory, IWebHostEnvironment

### Community 75 - "Community 75"
Cohesion: 0.25
Nodes (7): AccountResource, IHtmlLocalizer, Microsoft.AspNetCore.Mvc.Localization, Volo.Abp.Account.Localization, ISettingProvider, Volo.Abp.Account.Settings, Volo.Abp.Account.Web.Pages.Account.LoginModel

### Community 76 - "Community 76"
Cohesion: 0.25
Nodes (4): ServiceConfigurationContext, AbpProApplicationContractsModule, OneTimeRunner, AbpProDtoExtensions

### Community 77 - "Community 77"
Cohesion: 0.25
Nodes (6): PagedResultDto, Task, HttpPost, PagedResultDto, SwaggerOperation, Task

### Community 78 - "Community 78"
Cohesion: 0.25
Nodes (8): CreateApiResourceInput, AllowedAccessTokenSigningAlgorithms, Description, DisplayName, Enabled, Name, Secret, ShowInDiscoveryDocument

### Community 79 - "Community 79"
Cohesion: 0.25
Nodes (8): CreateApiScopeInput, Description, DisplayName, Emphasize, Enabled, Name, Required, ShowInDiscoveryDocument

### Community 80 - "Community 80"
Cohesion: 0.29
Nodes (5): AbpMultiTenancyOptions, IEmailSender, NullEmailSender, ServiceConfigurationContext, AbpProDomainModule

### Community 81 - "Community 81"
Cohesion: 0.38
Nodes (5): ApplyStateContext, AutoDeleteAfterSuccessAttributer, IApplyStateFilter, IWriteOnlyTransaction, TimeSpan

### Community 82 - "Community 82"
Cohesion: 0.38
Nodes (4): CookiePolicyOptions, HttpContext, IServiceCollection, SameSiteCookiesServiceCollectionExtensions

### Community 84 - "Community 84"
Cohesion: 0.29
Nodes (6): dependencies, @abp/aspnetcore.mvc.ui.theme.basic, name, private, version, @abp/aspnetcore.mvc.ui.theme.basic

### Community 85 - "Community 85"
Cohesion: 0.29
Nodes (6): AccountResource, IHtmlLocalizer, Microsoft.AspNetCore.Mvc.Localization, Volo.Abp.Account.Localization, route:/Account/LoggedOut, Volo.Abp.Account.Web.Pages.Account.LoggedOutModel

### Community 86 - "Community 86"
Cohesion: 0.29
Nodes (6): DateTime, ClientSecretOutput, Description, Expiration, Type, Value

### Community 87 - "Community 87"
Cohesion: 0.29
Nodes (6): Serilog.Extensions.Logging ($(SerilogExtensionsLoggingVersion)), Serilog.Sinks.Async ($(SerilogSinksAsyncVersion)), Serilog.Sinks.Console ($(SerilogSinksConsoleVersion)), Serilog.Sinks.File ($(SerilogSinksFileVersion)), Volo.Abp.Autofac ($(AbpPackageVersion)), net6.0

### Community 88 - "Community 88"
Cohesion: 0.40
Nodes (3): AbpDbContextOptions, ServiceConfigurationContext, AbpProEntityFrameworkCoreModule

### Community 89 - "Community 89"
Cohesion: 0.33
Nodes (4): Lion.AbpPro.Localization.Extensions, Enum, MemberInfo, EnumLocalicationExtension

### Community 90 - "Community 90"
Cohesion: 0.40
Nodes (3): IHostBuilder, Program, LoggerConfiguration

### Community 91 - "Community 91"
Cohesion: 0.33
Nodes (3): IApplicationBuilder, IServiceCollection, Startup

### Community 92 - "Community 92"
Cohesion: 0.33
Nodes (5): Guid, ClientClaimOutput, ClientId, Type, Value

### Community 93 - "Community 93"
Cohesion: 0.33
Nodes (5): Guid, ClientPropertyOutput, ClientId, Key, Value

### Community 94 - "Community 94"
Cohesion: 0.33
Nodes (6): AbpProPermissions, ApiResource, Client, IdentityResources, IdentityServer, SystemManagement

### Community 95 - "Community 95"
Cohesion: 0.33
Nodes (6): SettingItemOutput, Description, DisplayName, Name, Type, Value

### Community 96 - "Community 96"
Cohesion: 0.33
Nodes (5): DateTime, PagingElasticSearchLogDto, CreationTime, Level, Message

### Community 97 - "Community 97"
Cohesion: 0.33
Nodes (5): CustomePagedResultDto, IConfiguration, List, Task, LionAbpProLogAppService

### Community 98 - "Community 98"
Cohesion: 0.33
Nodes (6): AbpProSettings, Cell, ControlType, Group, Other, Verification

### Community 100 - "Community 100"
Cohesion: 0.40
Nodes (3): AbpLocalizationOptions, ServiceConfigurationContext, AbpProHttpApiModule

### Community 101 - "Community 101"
Cohesion: 0.40
Nodes (4): AuditLog, List, PagedResultDto, Task

### Community 102 - "Community 102"
Cohesion: 0.40
Nodes (4): WCSOptions, Enable, Server, WCSSimulation

### Community 103 - "Community 103"
Cohesion: 0.40
Nodes (5): DateTime, PagingElasticSearchLogInput, EndCreationTime, Filter, StartCreationTime

### Community 104 - "Community 104"
Cohesion: 0.40
Nodes (4): Guid, ClientCorsOriginOutput, ClientId, Origin

### Community 105 - "Community 105"
Cohesion: 0.40
Nodes (4): Guid, ClientGrantTypeOutput, ClientId, GrantType

### Community 106 - "Community 106"
Cohesion: 0.40
Nodes (4): Guid, ClientIdPRestrictionOutput, ClientId, Provider

### Community 107 - "Community 107"
Cohesion: 0.40
Nodes (4): Guid, ClientPostLogoutRedirectUriOutput, ClientId, PostLogoutRedirectUri

### Community 108 - "Community 108"
Cohesion: 0.40
Nodes (4): Guid, ClientRedirectUriOutput, ClientId, RedirectUri

### Community 109 - "Community 109"
Cohesion: 0.40
Nodes (4): Guid, ClientScopeOutput, ClientId, Scope

### Community 110 - "Community 110"
Cohesion: 0.40
Nodes (5): List, SettingOutput, Group, GroupDisplayName, SettingItemOutput

### Community 111 - "Community 111"
Cohesion: 0.40
Nodes (4): Guid, ApiResourceClaimOutput, ApiResourceId, Type

### Community 114 - "Community 114"
Cohesion: 0.50
Nodes (3): ICurrentUser, Volo.Abp.Users, Lion.AbpPro.Pages.Welcome

### Community 115 - "Community 115"
Cohesion: 0.50
Nodes (3): MesOptions, Enable, Server

## Knowledge Gaps
- **490 isolated node(s):** `MultiTenancyConsts`, `AbpProHttpApiHostConsts`, `HttpClientNameConsts`, `AbpProConsts`, `AbpProDomainErrorCodes` (+485 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 844 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **8 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `AccountAppService` connect `Community 31` to `Community 56`, `Community 48`, `Community 29`, `Community 15`?**
  _High betweenness centrality (0.170) - this node is a cross-community bridge._
- **Why does `Lion.AbpPro.Localization` connect `Community 16` to `Community 100`, `Community 68`, `Community 70`, `Community 39`, `Community 74`, `Community 49`?**
  _High betweenness centrality (0.144) - this node is a cross-community bridge._
- **Why does `Lion.AbpPro.Permissions` connect `Community 64` to `Community 33`, `Community 4`, `Community 38`, `Community 48`, `Community 16`, `Community 49`, `Community 83`, `Community 56`, `Community 27`, `Community 28`?**
  _High betweenness centrality (0.124) - this node is a cross-community bridge._
- **What connects `MultiTenancyConsts`, `AbpProHttpApiHostConsts`, `HttpClientNameConsts` to the rest of the system?**
  _490 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.057902973395931145 - nodes in this community are weakly interconnected._
- **Should `Community 1` be split into smaller, more focused modules?**
  _Cohesion score 0.05134575569358178 - nodes in this community are weakly interconnected._
- **Should `Community 2` be split into smaller, more focused modules?**
  _Cohesion score 0.05926251097453907 - nodes in this community are weakly interconnected._