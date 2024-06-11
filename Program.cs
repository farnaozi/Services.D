using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Services.D.Core.Enums;
using Services.D.Core.Events;
using Services.D.Core.Handlers;
using Services.D.Core.Helpers;
using Services.D.Core.Interfaces;
using Services.D.Core.Models;
using Services.D.Shared;

var builder = WebApplication.CreateBuilder(args);

#region *** ConfigureServices
var appSettings = builder.Configuration.GetSection(nameof(AppSettings));
builder.Services.Configure<AppSettings>(appSettings);

var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).ToJWTSettings();
builder.Services.ConfigureJwtSettings(jwtSettings);
builder.Services.AddJWTAuthentication(jwtSettings);

builder.Services.ConfigureRepositories();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service D", Version = "v1" });
});

builder.Services.AddHttpClient();
#endregion

var app = builder.Build();

#region *** Configure
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service D v1"));
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<ServiceCDEvent, ServiceCDEventHandler>(ExchangeTypes.Fanout);
#endregion

app.Run();
