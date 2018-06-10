using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Html;
using JPRSC.HRIS.WebApp.Infrastructure.Mvc;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.PayPercentages
{
    [AuthorizePermission(Permission.PayPercentageDefault)]
    public class PayPercentagesController : AppController
    {
        private readonly IMediator _mediator;

        public PayPercentagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View(new Add.Command());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(Add.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully added pay percentage.");

            return Json("success");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Delete.Command command)
        {
            var commandResult = await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully deleted pay percentage.");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<ActionResult> Edit(Edit.Query query)
        {
            var command = await _mediator.Send(query);

            return View(command);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Edit.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            //NotificationHelper.CreateSuccessNotification(this, $"Successfully edited pay percentage {command.Name}.");

            return Json("success");
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