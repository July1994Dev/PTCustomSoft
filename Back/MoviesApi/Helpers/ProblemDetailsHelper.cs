using Microsoft.AspNetCore.Mvc;

namespace MoviesApi.Helpers
{
    public static class ProblemDetailsHelper
    {
        public static ProblemDetails CreateProblemDetails(HttpContext httpContext, int statusCode, string title, string detail)
        {
            return new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };
        }
    }
}
