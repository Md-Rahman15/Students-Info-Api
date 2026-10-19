using System.Net;

namespace Students_Info_Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExceptionMiddleware> logger;

        public GlobalExceptionMiddleware(RequestDelegate next,ILogger<GlobalExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext) 
        {
            try
            {
                await next(httpContext);

               
            }
            catch (Exception Ex) 
            {

                var traceid = httpContext.TraceIdentifier;
                logger.LogError(Ex, "An unexpected error occurred. TraceId: {TraceId}", traceid);

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var errorRespone = new
                {
                    errorMessage = "Something went wrong. We are looking to resolve it" + "Trace id : ",
                     traceid = traceid,
                };
            
                await httpContext.Response.WriteAsJsonAsync(errorRespone);
               
            }
        
        
        }


    }
}
