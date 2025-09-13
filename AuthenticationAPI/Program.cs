using AuthenticationAPI.Exceptions;
using AuthenticationAPI.Extensions;
using FluentValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes:true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLogging();
builder.Services.SetupSwagger();
builder.Services.AddControllers();
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.SetupBackgroundServices();
builder.Services.SetupConfigurations(builder.Configuration);
builder.Services.SetupServices();
builder.Services.ConfigureAuthentication();
builder.SetupSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionHandler>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();