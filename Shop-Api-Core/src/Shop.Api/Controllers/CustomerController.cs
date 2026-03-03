using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Shop.Application.Customers.Commands;
using Shop.Application.Customers.Models;
using Shop.Application.Customers.Queries;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("AddCustomer")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddCustomer(
            [FromBody] CreateCustomerCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                CustomerResponse result = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetCustomer), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Email already exists."))
            {
                return StatusCode(StatusCodes.Status409Conflict, new
                {
                    status = StatusCodes.Status409Conflict,
                    message = "Email already exists."
                });
            }
        }

        [Route("GetCustomer/{id:guid}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCustomer(Guid id, CancellationToken cancellationToken)
        {
            CustomerResponse result = await _mediator.Send(new GetCustomerByIdQuery(id), cancellationToken);
            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
