using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Producer.Databases;
using Producer.Hubs;
using Producer.Mappers;
using Producer.Services;
using Publish;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ISettingConfigurationService, SettingConfigurationService>();
builder.Services.AddAutoMapper(typeof(SettingConfigurationMapper));
builder.Services.AddControllers();
builder.Services.Configure<DatabaseSetting>(builder.Configuration.GetSection("Database"));
builder.Services.AddSingleton<IDatabaseSetting>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSetting>>().Value;
});

// Add Swagger
builder.Services.AddSwaggerGen();

// Add EventBus
builder.Services.AddSingleton<IConnectedQService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<ConnectedQService>>();
    var factory = new ConnectionFactory()
    {
        HostName = builder.Configuration["Database:HostName"]
    };

    if (!string.IsNullOrWhiteSpace(builder.Configuration["Database:UserName"]))
    {
        factory.UserName = builder.Configuration["Database:UserName"];
    }

    if (!string.IsNullOrWhiteSpace(builder.Configuration["Database:Password"]))
    {
        factory.UserName = builder.Configuration["Database:Password"];
    }

    var retryCount = 5;
    string retryCountString = builder.Configuration["Publisher:RetryCount"];

    if (!string.IsNullOrWhiteSpace(retryCountString))
    {
        retryCount = int.Parse(retryCountString);
    }

    return new ConnectedQService(factory, retryCount, logger);
});

builder.Services.AddSingleton<PublishQService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()
               .WithOrigins("http://localhost:5012");
    });
});

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Enable middleware to serve generated Swagger as a JSON endpoint.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapHub<ConfigurationHub>("/ConfigurationHub");

app.MapControllers();

app.Run();
