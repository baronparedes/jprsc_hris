using JPRSC.HRIS.Features.Reports;
using JPRSC.HRIS.Infrastructure.Mvc;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.Reports
{
    [AuthorizePermission(Permission.ReportsDefault)]
    public class ReportsController : AppController
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Generate2316(Generate2316.Query query)
        {
            if (!ModelState.IsValid) return Content($"Error: {ModelState.GetAllErrors().First()}");

            var result = await _mediator.Send(query);

            return View("2316Report", result);
        }

        [HttpGet]
        public async Task<ActionResult> GenerateAlphalist(GenerateAlphalist.Query query)
        {
            if (!ModelState.IsValid) return Content($"Error: {ModelState.GetAllErrors().First()}");

            var result = await _mediator.Send(query);

            if (query.Destination == "Excel" || query.Destination == "CSV")
            {
                return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
            }
            else
            {
                return View("AlphalistReport", result);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GenerateMasterlist(GenerateMasterlist.Query query)
        {
            var result = await _mediator.Send(query);

            if (query.Destination == "Excel")
            {
                return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
            }
            else
            {
                return View("MasterlistReport", result);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GenerateEarningsDeductions(GenerateEarningsDeductions.Query query)
        {
            if (!ModelState.IsValid) return Content($"Error: {ModelState.GetAllErrors().First()}");

            var result = await _mediator.Send(query);

            if (query.Destination == "Excel")
            {
                return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
            }
            else
            {
                return View("EarningsDeductionsReport", result);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GeneratePagIbig(GeneratePagIbig.Query query)
        {
            var result = await _mediator.Send(query);

            if (query.Destination == "Excel")
            {
                return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
            }
            else
            {
                return View("PagIbigReport", result);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GeneratePHIC(GeneratePHIC.Query query)
        {
            var result = await _mediator.Send(query);

            if (query.Destination == "Excel")
            {
                return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
            }
            else
            {
                return View("PHICReport", result);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GenerateSSS(GenerateSSS.Query query)
        {
            var result = await _mediator.Send(query);

            if (query.Destination == "Excel")
            {
                return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
            }
            else
            {
                return View("SSSReport", result);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GenerateLoanLedger(GenerateLoanLedger.Query query)
        {
            var result = await _mediator.Send(query);

            if (query.Destination == "Excel")
            {
                return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
            }
            else
            {
                return View("LoanLedgerReport", result);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GenerateSingleLoanType(GenerateSingleLoanType.Query query)
        {
            var result = await _mediator.Send(query);

            if (query.Destination == "Excel")
            {
                return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
            }
            else
            {
                return View("SingleLoanTypeReport", result);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GenerateThirteenthMonth(GenerateThirteenthMonth.Query query)
        {
            if (!ModelState.IsValid) return Content($"Error: {ModelState.GetAllErrors().First()}");

            var result = await _mediator.Send(query);

            if (query.Destination == "Excel")
            {
                return File(result.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, result.Filename);
            }
            else
            {
                return View("ThirteenthMonthReport", result);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Index(Index.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }
    }
}