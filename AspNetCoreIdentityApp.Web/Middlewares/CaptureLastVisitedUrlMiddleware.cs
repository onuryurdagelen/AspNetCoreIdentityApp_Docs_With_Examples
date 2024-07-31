namespace AspNetCoreIdentityApp.Web.Middlewares
{
    public class CaptureLastVisitedUrlMiddleware
    {
        private readonly RequestDelegate _next;

        public CaptureLastVisitedUrlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.HasValue && !context.Request.Path.Value.Contains("Client/Home/Login"))
            {
                context.Session.SetString("LastVisitedUrl", context.Request.Path + context.Request.QueryString);
            }

            await _next(context);
        }
    }
}
