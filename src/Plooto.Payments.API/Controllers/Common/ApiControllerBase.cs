using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Plooto.Payments.API.Controllers.Common
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected readonly ILogger _logger;
        protected readonly IMediator _mediator;

        public ApiControllerBase(ILogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
    }
}
