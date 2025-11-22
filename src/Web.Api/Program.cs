using System.Reflection;
using Application;
using Infrastructure;
using Serilog;
using Serilog.Extensions.Logging;
using Web.Api;
using Web.Api.Configuration;
using Web.Api.Endpoints;
using Web.Api.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting application");

var builder = WebApplication.CreateBuilder(args);

// Init logger
builder.AddLoggerConfig();

var appLogger = new SerilogLoggerFactory(Log.Logger).CreateLogger<Program>();

// Add services to the container
builder.Services.AddOptionsConfig(builder.Configuration, appLogger, builder);
builder.Services.AddApplication().AddInfrastructure(builder.Configuration);

builder.LogApplicationInfo(appLogger);

builder.Services.AddEndpoints(Assembly.GetAssembly(typeof(Program)));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Vue dev server default port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Use CORS before other middleware
app.UseCors("AllowVueApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
    app.UseExceptionHandler();
}

app.MapCustomHealthChecks();

app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();

await app.RunAsync();

Log.CloseAndFlush();

// For integration tests purpose only
namespace Web.Api
{
    public partial class Program()
    {
    }
}