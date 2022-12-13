using AutoMapper;
using AutoMapper.QueryableExtensions;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.Loans
{
    public class Index
    {
        public class Query : IRequest<QueryResult>
        {
        }

        public class QueryResult
        {
            public IList<Client> Clients { get; set; } = new List<Client>();
            public IList<SelectListItem> LoanTypesList { get; set; } = new List<SelectListItem>();
            public string NextTransactionNumber { get; set; }

            public class Client
            {
                public string Code { get; set; }
                public int? CurrentPayrollPeriod { get; set; }
                public CutOffPeriod? CutOffPeriod { get; set; }
                public int? DaysPerWeek { get; set; }
                public string Description { get; set; }
                public int? HoursPerDay { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
                public int? NumberOfHoursInADay { get; set; }
                public int? NumberOfPayrollPeriodsAMonth { get; set; }
                public int? NumberOfWorkingDaysForThisPayrollPeriod { get; set; }
                public PayrollCode? PayrollCode { get; set; }
                public DateTime? PayrollPeriodFrom { get; set; }
                public Month? PayrollPeriodMonth { get; set; }
                public DateTime? PayrollPeriodTo { get; set; }
                public TaxTable? TaxTable { get; set; }
                public bool? ZeroBasic { get; set; }
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Client, QueryResult.Client>().ForAllOtherMembers(opts => opts.Ignore());
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                var clients = await _db.Clients
                    .AsNoTracking()
                    .Where(c => !c.DeletedOn.HasValue)
                    .OrderBy(c => c.Code)
                    .ProjectTo<QueryResult.Client>(_mapper)
                    .ToListAsync();

                return new QueryResult
                {
                    Clients = clients,
                    LoanTypesList = await GetLoanTypesList(),
                    NextTransactionNumber = await GetNextTransactionNumber()
                };
            }

            private async Task<IList<SelectListItem>> GetLoanTypesList()
            {
                var loanTypes = await _db.LoanTypes.AsNoTracking().Where(c => !c.DeletedOn.HasValue).ToListAsync();

                return loanTypes
                    .Select(c => new SelectListItem
                    {
                        Text = c.Code,
                        Value = c.Id.ToString()
                    })
                    .ToList();
            }

            private async Task<string> GetNextTransactionNumber()
            {
                var transactionNumbers = await _db
                    .Loans
                    .AsNoTracking()
                    .Where(l => !l.DeletedOn.HasValue && l.TransactionNumber != null)
                    .Select(l => l.TransactionNumber)
                    .ToListAsync();

                if (!transactionNumbers.Any()) return "0001";

                var maxTransactionNumber = transactionNumbers
                    .ConvertAll(Convert.ToInt32)
                    .Max();

                return (maxTransactionNumber + 1).ToString(maxTransactionNumber + 1 < 1000 ? "D4" : null);
            }
        }
    }
}