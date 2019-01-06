﻿using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Mvc;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
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
        public async Task<ActionResult> Index(Index.Query query)
        {
            var result = await _mediator.Send(query);

            return View(result);
        }
    }
}