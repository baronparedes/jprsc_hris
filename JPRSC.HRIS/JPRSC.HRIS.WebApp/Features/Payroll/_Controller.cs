using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Mvc;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    [AuthorizePermission(Permission.PayrollDefault)]
    public class PayrollController : AppController
    {
        private readonly IMediator _mediator;

        public PayrollController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Index.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> Process(Process.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            return Json("success");
        }

        [HttpGet]
        public async Task<ActionResult> Search(Search.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }
    }
}