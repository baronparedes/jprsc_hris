using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Mvc;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.SystemSettings
{
    [AuthorizePermission(Permission.SystemSettingsDefault)]
    public class SystemSettingsController : AppController
    {
        private readonly IMediator _mediator;

        public SystemSettingsController(IMediator mediator)
        {
            _mediator = mediator;
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

            return Json("success");
        }
    }
}