namespace BlogWebApp.HelperClass
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;

        //public RedirectMiddleware(RequestDelegate next)
        //{
        //    _next = next;
        //}

        //public async Task Invoke(HttpContext context)
        //{
        //    if (context.Request.Path == "/Account/%2F")
        //    {
        //        // Redirect to another action
        //        context.Response.Redirect("/"); 
        //        return;
        //    }

        //    // If the request doesn't match the pattern, continue to the next middleware
        //    await _next(context);
        //}
    }

    //public static class RedirectMiddlewareExtensions
    //{
    //    public static IApplicationBuilder UseRedirectMiddleware(this IApplicationBuilder builder)
    //    {
    //        return builder.UseMiddleware
    //         (typeof(RedirectMiddleware));
    //    }
    //}
}
