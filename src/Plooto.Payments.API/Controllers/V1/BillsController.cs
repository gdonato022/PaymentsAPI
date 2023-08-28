using MediatR;
using Microsoft.AspNetCore.Mvc;
using Plooto.Payments.API.Controllers.Common;
using Plooto.Payments.Application.UseCases.Bill.Commands;
using Plooto.Payments.Application.UseCases.Bill.Queries;
using Plooto.Payments.Domain.Entities;

namespace Plooto.Payments.API.Controllers.V1
{
    /// <summary>
    /// Bill's controller
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{api-version:apiVersion}/bills")]
    public class BillsController : ApiControllerBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mediator"></param>
        public BillsController(ILogger<BillsController> logger, IMediator mediator) : base(logger, mediator)
        {
        }

        /// <summary>
        /// Get a list of bills
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Bill>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Bill>>> GetBills([FromQuery] GetBillsQuery request)
        {
            var result = await _mediator.Send(request);
            
            return Ok(result);
        }

        /// <summary>
        /// Create a new bill
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateBillCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(response);
        }
    }
}
