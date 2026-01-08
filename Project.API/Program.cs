using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Project.API.Extensions;
using Project.API.Middlewares;
using Project.Application.Common.Mappers;
using Project.Application.Features.Auth.Commands.Login;
using Project.Application.Features.Auth.Validators;

var builder = WebApplication.CreateBuilder(args);

#region Database
builder.Services.AddCustomDb(builder.Configuration);
#endregion

#region Options
builder.Services.AddCustomOptions(builder.Configuration);
#endregion

#region Custom Services
builder.Services.Register();
builder.Services.AddCustomHangfire(builder.Configuration);
builder.Services.RegisterSecurityService(builder.Configuration);
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.AddCustomRateLimit(builder.Configuration);
builder.Services.AddCustomApiVersioning();
builder.Services.AddCustomSwagger();
#endregion

#region Framework Services
builder.Services.AddCustomControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
#endregion

#region AutoMapper Services
builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);
#endregion

#region MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));
#endregion

#region Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(LoginRequestValidator).Assembly);
builder.Services.AddCustomFluentValidation();
#endregion

var app = builder.Build();

#region Database Initialization
await app.UseDatabaseInitialization();
#endregion

#region Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwaggerUI();
}

app.UseHangfireDashboard("/hangfire");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(CorsExtension.GetPolicyName());

app.UseRateLimiter();

#region Custom Middlewares
app.UseExceptionHandling();
app.UseRequestResponseLogging();
#endregion

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();
