using Chat.Communication.ViewObjects.APIResponse;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Extensions
{
    public static class AddCustomResponseStartup
    {
        public static IMvcBuilder AddCustomResponse(this IMvcBuilder builder)
        {
            builder.ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    APIResponseVO response = new APIResponseVO
                    {
                        Success = false,
                    };
                    foreach (var (key, value) in context.ModelState)
                    {
                        response.Message = value.Errors.Select(x => x.ErrorMessage).FirstOrDefault();
                    }
                    return new BadRequestObjectResult(response);
                };
            });

            return builder;
        }
    }
}
