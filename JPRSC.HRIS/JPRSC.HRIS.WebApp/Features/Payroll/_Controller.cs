using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Html;
using JPRSC.HRIS.WebApp.Infrastructure.Mvc;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class PayrollController : AppController
    {
        private readonly IMediator _mediator;

        public PayrollController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AuthorizePermission(Permission.PayrollProcess)]
        [HttpPost]
        public async Task<ActionResult> AddForProcessingBatch(AddForProcessingBatch.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            var result = await _mediator.Send(command);

            return JsonCamelCase(result);
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpGet]
        public async Task<ActionResult> BankReport(BankReport.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpGet]
        public async Task<ActionResult> CashHoldReport(CashHoldReport.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpGet]
        public async Task<ActionResult> CashReport(CashReport.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpGet]
        public async Task<ActionResult> CheckReport(CheckReport.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.PayrollDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Delete.Command command)
        {
            var commandResult = await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully deleted payroll record and all associated dtrs, overtimes, and e/d records.");

            return RedirectToAction(nameof(Index));
        }

        [AuthorizePermission(Permission.PayrollDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteForProcessingBatch(DeleteForProcessingBatch.Command command)
        {
            var commandResult = await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully deleted for processing queue.");

            return RedirectToAction(nameof(ForProcessingQueue));
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpGet]
        public async Task<ActionResult> EndProcess(EndProcess.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.PayrollEndProcess)]
        [HttpPost]
        public async Task<ActionResult> EndProcessCommand(EndProcess.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            command.AppController = this;

            var result = await _mediator.Send(command);

            return JsonCamelCase(result);
        }

        [AuthorizePermission(Permission.PayrollEndProcess)]
        [HttpGet]
        public async Task<ActionResult> EndProcessQuery(EndProcess.EndProcessQuery query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }

        [AuthorizePermission(Permission.PayrollProcess)]
        [HttpGet]
        public async Task<ActionResult> ExportQueueToDTRExcel(ExportQueueToDTRExcel.Query query)
        {
            var result = await _mediator.Send(query);

            return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
        }

        [AuthorizePermission(Permission.PayrollProcess)]
        [HttpGet]
        public async Task<ActionResult> ExportQueueToEDRExcel(ExportQueueToEDRExcel.Query query)
        {
            var result = await _mediator.Send(query);

            return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpGet]
        public async Task<ActionResult> ExportToExcel(ExportToExcel.Query query)
        {
            var result = await _mediator.Send(query);

            return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpPost]
        public async Task<ActionResult> ExportToExcelPost(ExportToExcel.Query query)
        {
            var result = await _mediator.Send(query);

            return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
        }

        [AuthorizePermission(Permission.PayrollProcess)]
        [HttpGet]
        public async Task<ActionResult> ForProcessing(ForProcessing.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.PayrollProcess)]
        [HttpGet]
        public async Task<ActionResult> ForProcessingBatchSearch(ForProcessingBatchSearch.Query query)
        {
            var result = await _mediator.Send(query);

            return JsonCamelCase(result);
        }

        [AuthorizePermission(Permission.PayrollProcess)]
        [HttpGet]
        public async Task<ActionResult> ForProcessingQueue(ForProcessingQueue.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpGet]
        public async Task<ActionResult> HoldReport(HoldReport.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpGet]
        public async Task<ActionResult> Index(Index.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpGet]
        public async Task<ActionResult> PayrollReport(PayrollReport.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpGet]
        public async Task<ActionResult> PayrollSummaryReport(PayrollSummaryReport.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpGet]
        public async Task<ActionResult> PayslipReport(PayslipReport.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.PayrollProcess)]
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

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpGet]
        public async Task<ActionResult> Search(Search.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }

        [AuthorizePermission(Permission.PayrollDefault)]
        [HttpPost]
        public async Task<ActionResult> SendPayslip(SendPayslip.Command command)
        {
            var commandResult = await _mediator.Send(command);

            return Json("success");
        }
    }
}