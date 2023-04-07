using JPRSC.HRIS.Features.Loans;
using JPRSC.HRIS.Infrastructure.Mvc;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Html;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features
{
    public class LoansController : AppController
    {
        private readonly IMediator _mediator;

        public LoansController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AuthorizePermission(Permission.LoanAdd)]
        [HttpGet]
        public ActionResult Add()
        {
            return View(new Add.Command());
        }

        [AuthorizePermission(Permission.LoanAdd)]
        [HttpPost]
        public async Task<ActionResult> Add(Add.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            return Json("success");
        }

        [AuthorizePermission(Permission.LoanDefault)]
        [HttpGet]
        public async Task<ActionResult> CheckExisting(CheckExisting.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }

        [AuthorizePermission(Permission.LoanAdd)]
        [HttpGet]
        public async Task<ActionResult> Edit(Edit.Query query)
        {
            var command = await _mediator.Send(query);

            return View(command);
        }

        [AuthorizePermission(Permission.LoanAdd)]
        [HttpPost]
        public async Task<ActionResult> Edit(Edit.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            return Json("success");
        }

        [AuthorizePermission(Permission.LoanDetails)]
        [HttpGet]
        public async Task<ActionResult> GetById(GetById.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }

        [AuthorizePermission(Permission.LoanDefault)]
        [HttpGet]
        public async Task<ActionResult> Index(Index.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.LoanZeroOut)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MakeActive(MakeActive.Command command)
        {
            var commandResult = await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully activated loan.");

            return RedirectToAction(nameof(Index));
        }

        [AuthorizePermission(Permission.LoanDefault)]
        [HttpGet]
        public async Task<ActionResult> Search(Search.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }

        [AuthorizePermission(Permission.LoanZeroOut)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ZeroOut(ZeroOut.Command command)
        {
            var commandResult = await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully zeroed out loan.");

            return RedirectToAction(nameof(Index));
        }
    }
}