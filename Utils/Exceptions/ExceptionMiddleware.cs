public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            BaseException e => (e.StatusCode, new { e.Message, e.ErrorCode }),
            _ => (500, new { Message = "Error interno no controlado", ErrorCode = "SERVER_ERROR" })
        };

        context.Response.StatusCode = response.Item1;
        return context.Response.WriteAsJsonAsync(response.Item2);
    }
}