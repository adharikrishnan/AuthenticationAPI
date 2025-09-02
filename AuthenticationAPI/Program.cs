using AuthenticationAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.SetupSwagger();
builder.Services.AddControllers();
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.SetupConfigurations(builder.Configuration);
builder.Services.SetupServices();
builder.Services.ConfigureAuthentication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();