using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using MimeKit.Cryptography;
using System.Net;
using System.Text.Json;

namespace BlogWebApp.ExceptionHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IUrlHelperFactory urlHelperFactory)
        {
            _logger = logger;
            _urlHelperFactory = urlHelperFactory;
        }
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            System.Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, exception.Message);

            var details = new ProblemDetails() { 
                Detail = $"Error   {exception.Message}",
                Instance= "Error",
                Status= (int)HttpStatusCode.InternalServerError,
                Title="Exception Error",
                Type= "Server Error"
            };

            var response = JsonSerializer.Serialize(details);
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(response, cancellationToken);
            return true;
            // Redirect to an error page with the problem 

        }
    }
}
