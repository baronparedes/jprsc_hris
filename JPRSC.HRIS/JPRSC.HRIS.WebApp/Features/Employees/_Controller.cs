using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Html;
using JPRSC.HRIS.WebApp.Infrastructure.Mvc;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.Employees
{
    public class EmployeesController : AppController
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AuthorizePermission(Permission.EmployeeAdd)]
        [HttpGet]
        public async Task<ActionResult> Add()
        {
            var command = await _mediator.Send(new Add.Query());

            return View(command);
        }

        [AuthorizePermission(Permission.EmployeeAdd)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(Add.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully added employee {command.FirstName} {command.LastName}");

            return Json("success");
        }

        [AuthorizePermission(Permission.EmployeeDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Delete.Command command)
        {
            var commandResult = await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully deleted employee {commandResult.FirstName} {commandResult.LastName}.");

            return RedirectToAction(nameof(Index));
        }

        [AuthorizePermission(Permission.EmployeeDefault)]
        [HttpGet]
        public async Task<ActionResult> Details(Details.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.EmployeeEdit)]
        [HttpGet]
        public async Task<ActionResult> Edit(Edit.Query query)
        {
            var command = await _mediator.Send(query);

            return View(command);
        }

        [AuthorizePermission(Permission.EmployeeEdit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Edit.Command command)
        {
            if (!ModelState.IsValid)
            {
                return JsonValidationError();
            }

            await _mediator.Send(command);

            NotificationHelper.CreateSuccessNotification(this, $"Successfully edited employee {command.FirstName} {command.LastName}.");

            return Json("success");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetByClientId(GetByClientId.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }

        [AuthorizePermission(Permission.EmployeeDefault)]
        [HttpGet]
        public async Task<ActionResult> Index(Index.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }

        [AuthorizePermission(Permission.EmployeeDefault)]
        [HttpGet]
        public async Task<ActionResult> Search(Search.Query query)
        {
            var queryResult = await _mediator.Send(query);

            return JsonCamelCase(queryResult);
        }
    }
}