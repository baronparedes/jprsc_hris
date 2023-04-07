using JPRSC.HRIS.Features.DailyTimeRecords;
using JPRSC.HRIS.Infrastructure.Mvc;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Html;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features
{
    [AuthorizePermission(Permission.DailyTimeRecordDefault)]
    public class DailyTimeRecordsController : AppController
    {
        private readonly IMediator _mediator;

        public DailyTimeRecordsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View(new Add.Command());
        }

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

        [HttpPost]
        public async Task<ActionResult> BulkUploadDTR(BulkUploadDTR.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            var commandResult = await _mediator.Send(command);

            return JsonCamelCase(commandResult);
        }

        [HttpPost]
        public async Task<ActionResult> BulkUploadEDR(BulkUploadEDR.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            var commandResult = await _mediator.Send(command);

            return JsonCamelCase(commandResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Delete.Command command)
        {
            var commandResult = await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully deleted daily time record.");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<ActionResult> Edit(Edit.Query query)
        {
            var command = await _mediator.Send(query);

            return View(command);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Edit.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully edited daily time record.");

            return Json("success");
        }

        [HttpGet]
        public async Task<ActionResult> Index(Index.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [HttpGet]
        public async Task<ActionResult> PayrollPeriodSelection(PayrollPeriodSelection.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }

        [HttpGet]
        public async Task<ActionResult> Search(Search.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }
    }
}