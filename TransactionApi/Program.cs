using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TransactionApi.Configuration;
using TransactionApi.Extensions;
using TransactionApi.Middleware;
using TransactionApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<TransactionApiSettings>(builder.Configuration.GetSection("TransactionApi"));

builder.Services.AddDbContext(builder.Configuration);

builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();

app.ApplyMigrations();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        var problem = error switch
        {
            ArgumentException ex => new ProblemDetails
            {
                Status = 400,
                Title = "Validation error",
                Detail = ex.Message
            },
            InvalidOperationException ex => new ProblemDetails
            {
                Status = 409,
                Title = "Business rule violation",
                Detail = ex.Message
            },
            _ => new ProblemDetails
            {
                Status = 500,
                Title = "Internal Server Error"
            }
        };

        context.Response.StatusCode = problem.Status ?? 500;
        await context.Response.WriteAsJsonAsync(problem);
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.Run();
