using JPRSC.HRIS.Features.Clients;
using JPRSC.HRIS.Infrastructure.Mvc;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Html;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features
{
    public class ClientsController : AppController
    {
        private readonly IMediator _mediator;

        public ClientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AuthorizePermission(Permission.ClientAdd)]
        [HttpGet]
        public ActionResult Add()
        {
            return View(new Add.Command());
        }

        [AuthorizePermission(Permission.ClientAdd)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(Add.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully added client {command.Name}.");

            return Json("success");
        }

        [AuthorizePermission(Permission.ClientDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Delete.Command command)
        {
            var commandResult = await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully deleted client {commandResult.Name}.");

            return RedirectToAction(nameof(Index));
        }

        [AuthorizePermission(Permission.ClientDefault)]
        [HttpGet]
        public async Task<ActionResult> Details(Details.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.ClientEdit)]
        [HttpGet]
        public async Task<ActionResult> Edit(Edit.Query query)
        {
            var command = await _mediator.Send(query);

            return View(command);
        }

        [AuthorizePermission(Permission.ClientEdit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Edit.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully edited client {command.Name}.");

            return Json("success");
        }

        [AuthorizePermission(Permission.ClientDefault)]
        [HttpGet]
        public async Task<ActionResult> Index(Index.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.ClientDefault)]
        [HttpGet]
        public async Task<ActionResult> Search(Search.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }
    }
}