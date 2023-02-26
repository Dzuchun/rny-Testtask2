namespace rny_Testtask2.Middleware
{
    public class MyLogger
    {
        private readonly RequestDelegate _next;
        public MyLogger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();

            using StreamReader stream = new StreamReader(ctx.Request.Body);
            string? rawRequestBody = await stream.ReadToEndAsync();
            Console.Write($"--- Incoming Request ---\nMethod: {ctx.Request.Method},\nHeaders: \"{string.Join(", ", ctx.Request.Headers.ToArray())}\",\nQuery params: [{string.Join(", ", ctx.Request.Query.Select(p => $"{p.Key} = {p.Value}"))}],\nBody:\n{((rawRequestBody.Length > 1000) ? rawRequestBody.Substring(0, 1000) : rawRequestBody)}\n---==================---\n\n");
            ctx.Request.Body.Position = 0;
            await _next(ctx);
        }
    }
}
