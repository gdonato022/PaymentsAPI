using MediatR;
using Microsoft.AspNetCore.Mvc;
using Plooto.Payments.API.Controllers.Common;
using Plooto.Payments.Application.UseCases.Payment.Commands;
using Plooto.Payments.Application.UseCases.Payment.Queries;
using Plooto.Payments.Domain.Entities;

namespace Plooto.Payments.API.Controllers.V1
{
    /// <summary>
    /// Payment's controller
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{api-version:apiVersion}/payments")]
    public class PaymentsController : ApiControllerBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mediator"></param>
        public PaymentsController(ILogger<PaymentsController> logger, IMediator mediator) : base(logger, mediator)
        {
        }

        /// <summary>
        /// Get a list of payments by bill
        /// </summary>
        /// <returns></returns>
        [HttpGet("bills/{billId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Payment>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Payment>>> GetBills([FromRoute] int billId)
        {
            var result = await _mediator.Send(new GetPaymentsByBillQuery { BillId = billId });

            return Ok(result);
        }

        /// <summary>
        /// Create a new payment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreatePaymentCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(response);
        }
    }
}
