using Microsoft.AspNetCore.Mvc;
using TransactionApi.Models.Dto;
using TransactionApi.Services;

namespace TransactionApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }

		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] Guid id, CancellationToken cancellationToken)
		{
			var res = await _service.GetAsync(id, cancellationToken);
			if (res == null)
			{
				return NotFound(new ProblemDetails
				{
					Status = StatusCodes.Status404NotFound,
					Title = "Not found",
					Detail = $"Entity with id '{id}' was not found"
				});
			}

			return Ok(res);
		}

		[HttpPost]
        public async Task<IActionResult> Post([FromBody] TransactionCreateRequest transaction, CancellationToken cancellationToken)
        {
            var res = await _service.CreateAsync(transaction, cancellationToken);
             return Ok(res);
        }
    }
}
