using Microsoft.AspNetCore.Mvc;

namespace Project.API.Extensions
{
    public static class FluentValidationExtension
    {
        public static IServiceCollection AddCustomFluentValidation(this IServiceCollection services)
        {
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = (ActionContext context) =>
                    {
                        Dictionary<string, string[]> errors = context.ModelState
                            .Where(e => e.Value?.Errors.Count > 0)
                            .ToDictionary(
                                kvp => char.ToLowerInvariant(kvp.Key[0]) + kvp.Key.Substring(1),
                                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                            );

                        var response = new
                        {
                            success = false,
                            message = "One or more validation errors occurred.",
                            errors,
                            error = new
                            {
                                code = "VALIDATION_ERROR",
                                type = "ValidatorException"
                            },
                            timestamp = DateTime.UtcNow
                        };

                        return new BadRequestObjectResult(response);
                    };
                });

            return services;
        }
    }
}
