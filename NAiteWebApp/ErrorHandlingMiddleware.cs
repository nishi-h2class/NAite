using System.Net;

namespace NAiteWebApp
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // エラーメッセージをHttpContext.Itemsに保存
                context.Items["ErrorMessage"] = ex.Message;

                // ステータスコードを500に設定
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // リダイレクトを行う
                context.Response.Redirect("/error/500");
            }
        }
    }
}
