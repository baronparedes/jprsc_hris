using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Html;
using JPRSC.HRIS.WebApp.Infrastructure.Mvc;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.EarningDeductionRecords
{
    [AuthorizePermission(Permission.EarningDeductionRecordDefault)]
    public class DailyTimeRecordsController : AppController
    {
        private readonly IMediator _mediator;

        public DailyTimeRecordsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> BulkUpload(BulkUpload.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            var commandResult = await _mediator.Send(command);

            return JsonCamelCase(commandResult);
        }
    }
}