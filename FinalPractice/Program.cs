using Microsoft.OpenApi.Models;
using UPB.CoreLogic.Managers;
using UPB.CoreLogic.Services;
using UPB.FinalPractice.Middlewares;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var configurationBuilder = new ConfigurationBuilder()
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

IConfiguration Configuration = configurationBuilder.Build();
string siteTitle = Configuration.GetSection("Title").Value;
string filePath = Configuration.GetSection("FileJson").Value;

//create the logger and setup your sinks, filters and properties
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(Configuration)
    .CreateLogger();

string uri = Configuration.GetSection("URI").Value;
ProductService service = new ProductService(uri);
builder.Services.AddTransient<ProductManager>(ServiceProvider => new ProductManager(filePath, service));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = siteTitle
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
Log.Information("Environment: " + builder.Environment.EnvironmentName);
app.UseGlobalExceptionHandler(Log.Logger);
app.UseSwagger();
app.UseSwaggerUI();
//app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();
app.Run();
