using JPRSC.HRIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Domain
{
    internal static class EmployeeExtensions
    {
        internal static List<Employee> Resigned(this IEnumerable<Employee> employees, DateTime payrollPeriodFrom)
        {
            return employees
                .Where(e => (e.ResignStatus == ResignStatus.AWOL || e.ResignStatus == ResignStatus.Resigned) && e.DateResigned.HasValue && e.DateResigned.Value.Date < payrollPeriodFrom.Date)
                .ToList();
        }
    }
}
