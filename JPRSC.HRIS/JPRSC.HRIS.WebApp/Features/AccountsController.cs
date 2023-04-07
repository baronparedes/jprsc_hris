using JPRSC.HRIS.Features.Accounts;
using JPRSC.HRIS.Infrastructure.Mvc;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Html;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features
{
    public class AccountsController : AppController
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AuthorizePermission(Permission.AccountDefault)]
        public async Task<ActionResult> Add()
        {
            var command = await _mediator.Send(new Add.Query());

            return View(command);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permission.AccountDefault)]
        public async Task<ActionResult> Add(Add.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully added user {command.Name}.");

            return Json("success");
        }

        [HttpGet]
        [AuthorizePermission(Permission.AccountDefault)]
        public async Task<ActionResult> ChangePassword(ChangePassword.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return View(queryResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permission.AccountDefault)]
        public async Task<ActionResult> ChangePassword(ChangePassword.Command command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            var commandResult = await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully changed password for user {commandResult.Name}.");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permission.AccountDefault)]
        public async Task<ActionResult> Delete(Delete.Command command)
        {
            var commandResult = await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully deleted user {commandResult.Name}.");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AuthorizePermission(Permission.AccountDefault)]
        public async Task<ActionResult> Edit(Edit.Query query)
        {
            var command = await _mediator.Send(query);

            return View(command);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permission.AccountDefault)]
        public async Task<ActionResult> Edit(Edit.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully edited user {command.Name}.");

            return Json("success");
        }

        [HttpGet]
        [AuthorizePermission(Permission.AccountEditOwn)]
        public async Task<ActionResult> EditOwn(EditOwn.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return View(queryResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permission.AccountEditOwn)]
        public async Task<ActionResult> EditOwn(EditOwn.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully edited account.");

            return Json("success");
        }

        [HttpGet]
        [AuthorizePermission(Permission.AccountDefault)]
        public async Task<ActionResult> Index(Index.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [HttpGet]
        [AuthorizePermission(Permission.AccountDefault)]
        public async Task<ActionResult> Search(Search.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }
    }
}