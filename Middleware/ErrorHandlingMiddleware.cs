using System.Net;

namespace smart_receipt_web.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Handle 401 Unauthorized
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    // If it's an AJAX request, return JSON
                    if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"success\":false,\"message\":\"Oturum süreniz doldu. Lütfen tekrar giriş yapın.\"}");
                    }
                    else
                    {
                        // Redirect to login page
                        context.Response.Redirect("/Auth/Login");
                    }
                }
                // Handle 403 Forbidden
                else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"success\":false,\"message\":\"Bu işlem için yetkiniz yok.\"}");
                    }
                    else
                    {
                        context.Response.Redirect("/Home/Error");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"success\":false,\"message\":\"Bir hata oluştu. Lütfen tekrar deneyin.\"}");
            }
            else
            {
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}

