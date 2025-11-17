using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Project.API.Extensions;
using Project.API.Middlewares;
using Project.Application.Interfaces.IDataSeedingServices;
using Project.Application.Mappers;
using Project.Application.Validators.AuthValidators;
using Project.Infrastructure.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);

#region Database
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("PrimaryDbConnection"));
});
#endregion

#region Options
builder.Services.AddCustomOptions(builder.Configuration);
#endregion

#region Custom Services
builder.Services.Register();
builder.Services.RegisterSecurityService(builder.Configuration);
builder.Services.AddCustomCors(builder.Configuration);
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

#region Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(LoginValidator).Assembly);
builder.Services.AddCustomFluentValidation();
#endregion

var app = builder.Build();

#region Database Initialization
using var scope = app.Services.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await db.Database.MigrateAsync();

var seedingService = scope.ServiceProvider.GetRequiredService<IDataSeedingService>();
await seedingService.SeedDataAsync();
#endregion

#region Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                                    $"API {description.GroupName.ToUpperInvariant()}");
        }

        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.EnableFilter();
        options.ShowExtensions();
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(CorsExtension.GetPolicyName());

#region Custom Middlewares
app.UseExceptionHandling();
app.UseRequestResponseLogging();
#endregion

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();
