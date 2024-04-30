using System.Net;
using System.Text.Json;
using ChatApp.API.Errors;

namespace ChatApp.API.Middlewares
{
    public class ExecptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExecptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExecptionMiddleware(RequestDelegate next, ILogger<ExecptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // if our context is good go to next 
                await _next(context);
            }
            catch (Exception ex)
            {
                // log the ex into terminal or output window
                _logger.LogError(ex, ex.Message);
                // make return response json
                context.Response.ContentType = "application/json";
                // make our status code at thi response is 500
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment() 
                    ? new ApiExecption(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiExecption(context.Response.StatusCode, ex.Message, "Internal Server Error");

                var options = new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                // return string json 
                string json = JsonSerializer.Serialize(response, options);
                
                await context.Response.WriteAsync(json);
            }
        }
    }
}