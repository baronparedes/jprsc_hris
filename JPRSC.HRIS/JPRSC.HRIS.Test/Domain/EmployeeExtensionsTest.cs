using System;
using System.IO;
using Xunit;
using JPRSC.HRIS.Domain;
using System.Collections.Generic;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.Test.Domain
{
    public class EmployeeExtensionsTest
    {
        [Fact]
        public void AWOLAndResignedDateAfterPayrollPeriodFrom_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodFrom = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = ResignStatus.AWOL, DateResigned = new DateTime(2024, 1, 1) }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodFrom);

            // Assert
            Assert.Empty(resigned);
        }

        [Fact]
        public void ResignedAndResignedDateAfterPayrollPeriodFrom_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodFrom = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = ResignStatus.Resigned, DateResigned = new DateTime(2024, 1, 1) }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodFrom);

            // Assert
            Assert.Empty(resigned);
        }

        [Fact]
        public void AWOLOnly_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodFrom = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = ResignStatus.AWOL }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodFrom);

            // Assert
            Assert.Empty(resigned);
        }


        [Fact]
        public void ResignedOnly_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodFrom = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = ResignStatus.Resigned }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodFrom);

            // Assert
            Assert.Empty(resigned);
        }


        [Fact]
        public void ResignedDateAfterPayrollPeriodFromOnly_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodFrom = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { DateResigned = new DateTime(2024, 1, 1) }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodFrom);

            // Assert
            Assert.Empty(resigned);
        }

        [Fact]
        public void OtherResignStatusAndResignedDateAfterPayrollPeriodFrom_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodFrom = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = "Test", DateResigned = new DateTime(2024, 1, 1) }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodFrom);

            // Assert
            Assert.Empty(resigned);
        }

        [Fact]
        public void AWOLAndResignedDateOnPayrollPeriodFrom_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodFrom = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = ResignStatus.AWOL, DateResigned = new DateTime(2023, 1, 1) }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodFrom);

            // Assert
            Assert.Empty(resigned);
        }

        [Fact]
        public void AWOLAndResignedDateBeforePayrollPeriodFrom_ShouldBeResigned()
        {
            // Arrange
            var payrollPeriodFrom = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = ResignStatus.AWOL, DateResigned = new DateTime(2022, 1, 1) }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodFrom);

            // Assert
            Assert.Single(resigned);
        }
    }
}
