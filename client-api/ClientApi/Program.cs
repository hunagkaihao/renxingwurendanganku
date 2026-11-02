using ClientApi;
using ClientApi.FaceRecognition;
using ClientApi.Configuration;
using ClientApi.NetApi;
using ClientApi.Vein;

var builder = WebApplication.CreateBuilder(args);
builder.UseYamlSettings();
ServerUrl.Initialize(builder.Configuration);
try
{
    bool veinEnable = builder.Configuration.GetValue<bool>("Vein:Enable");
    if (veinEnable)
    {
        Console.WriteLine("指静脉开始初始化");
        //获取并验证指静脉连接配置，避免使用代码中的串口默认值。
        VeinHelper.Configure(builder.Configuration.GetSection("Vein").Get<VeinConnectionSettings>() ?? new());
        VeinHelper.VeinInt();//指静脉初始化
    }
}
catch (Exception e)
{
    Console.WriteLine("指静脉初始化异常: " + e.Message);
    Logger.Singleton.Error("指静脉初始化异常: " + e.Message);
    //throw;
}
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<FaceRecognitionService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<FaceRecognitionService>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // 加载当前应用的 XML 文档，展示接口、参数及模型字段的中文注释。
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ClientApi.xml"), includeControllerXmlComments: true);
    // 引用类型字段通过 allOf 保留字段说明，例如人脸响应中的 Data。
    options.UseAllOfToExtendReferenceSchemas();
});
const string DefaultCorsPolicyName = "Default";
bool allowAnyOrigin = builder.Configuration.GetValue("Cors:AllowAnyOrigin", true);
string[] corsOrigins = (builder.Configuration["CorsOrigins"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
//跨域策略配置
builder.Services.AddCors(options =>
{
    options.AddPolicy(DefaultCorsPolicyName, builder =>
    {
        builder.AllowAnyHeader().AllowAnyMethod();
        if (allowAnyOrigin) builder.AllowAnyOrigin();
        else builder.WithOrigins(corsOrigins);
    });
});
var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//app.Urls.Add("http://localhost:5032");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors(DefaultCorsPolicyName);//跨域设置
app.UseAuthorization();

app.MapControllers();

app.Run();
