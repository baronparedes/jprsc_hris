using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Html;
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
        public async Task<ActionResult> BankReport(BankReport.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Delete.Command command)
        {
            var commandResult = await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully deleted payroll record.");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<ActionResult> EndProcess(EndProcess.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [HttpGet]
        public async Task<ActionResult> EndProcessQuery(EndProcess.EndProcessQuery query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }

        [HttpPost]
        public async Task<ActionResult> EndProcessCommand(EndProcess.Command query)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            var result = await _mediator.Send(query);

            return JsonCamelCase(result);
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
        public async Task<ActionResult> PayrollReport(PayrollReport.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [HttpGet]
        public async Task<ActionResult> PayslipReport(PayslipReport.Query query)
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