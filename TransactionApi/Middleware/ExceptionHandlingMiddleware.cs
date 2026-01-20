using Microsoft.AspNetCore.Mvc;

namespace TransactionApi.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;

		public ExceptionHandlingMiddleware(
			RequestDelegate next,
			ILogger<ExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				LogException(context, ex);
				await WriteProblemDetailsAsync(context, ex);
			}
		}

		private void LogException(HttpContext context, Exception ex)
		{
			if (ex is ArgumentException or InvalidOperationException)
			{
				_logger.LogWarning(
					ex,
					"Business exception on {Method} {Path}, traceId={TraceId}",
					context.Request.Method,
					context.Request.Path,
					context.TraceIdentifier);
			}
			else
			{
				_logger.LogError(
					ex,
					"Unhandled exception on {Method} {Path}, traceId={TraceId}",
					context.Request.Method,
					context.Request.Path,
					context.TraceIdentifier);
			}
		}

		private static async Task WriteProblemDetailsAsync(
			HttpContext context,
			Exception ex)
		{
			var problem = ex switch
			{
				ArgumentException e => new ProblemDetails
				{
					Status = StatusCodes.Status400BadRequest,
					Title = "Validation error",
					Detail = e.Message
				},
				InvalidOperationException e => new ProblemDetails
				{
					Status = StatusCodes.Status409Conflict,
					Title = "Business rule violation",
					Detail = e.Message
				},
				_ => new ProblemDetails
				{
					Status = StatusCodes.Status500InternalServerError,
					Title = "Internal Server Error"
				}
			};

			context.Response.StatusCode = problem.Status!.Value;
			await context.Response.WriteAsJsonAsync(problem);
		}
	}
}
