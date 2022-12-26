using JPRSC.HRIS.Features.AuditTrails;
using JPRSC.HRIS.Infrastructure.Mvc;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.AuditTrails
{
    [AuthorizePermission(Permission.AuditTrailDefault)]
    public class AuditTrailsController : AppController
    {
        private readonly IMediator _mediator;

        public AuditTrailsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Index.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [HttpGet]
        public async Task<ActionResult> Search(Search.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }
    }
}