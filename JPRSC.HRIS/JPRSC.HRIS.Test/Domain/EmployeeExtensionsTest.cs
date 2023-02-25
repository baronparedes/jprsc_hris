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
        public void AWOLAndResignedDateAfterPayrollPeriodTo_ShouldBeResigned()
        {
            // Arrange
            var payrollPeriodTo = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = ResignStatus.AWOL, DateResigned = new DateTime(2024, 1, 1) }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodTo);

            // Assert
            Assert.Single(resigned);
        }

        [Fact]
        public void ResignedAndResignedDateAfterPayrollPeriodTo_ShouldBeResigned()
        {
            // Arrange
            var payrollPeriodTo = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = ResignStatus.Resigned, DateResigned = new DateTime(2024, 1, 1) }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodTo);

            // Assert
            Assert.Single(resigned);
        }

        [Fact]
        public void AWOLOnly_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodTo = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = ResignStatus.AWOL }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodTo);

            // Assert
            Assert.Empty(resigned);
        }


        [Fact]
        public void ResignedOnly_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodTo = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = ResignStatus.Resigned }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodTo);

            // Assert
            Assert.Empty(resigned);
        }


        [Fact]
        public void ResignedDateAfterPayrollPeriodToOnly_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodTo = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { DateResigned = new DateTime(2024, 1, 1) }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodTo);

            // Assert
            Assert.Empty(resigned);
        }

        [Fact]
        public void OtherResignStatusAndResignedDateAfterPayrollPeriodTo_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodTo = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = "Test", DateResigned = new DateTime(2024, 1, 1) }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodTo);

            // Assert
            Assert.Empty(resigned);
        }

        [Fact]
        public void AWOLAndResignedDateOnPayrollPeriodTo_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodTo = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = ResignStatus.AWOL, DateResigned = new DateTime(2023, 1, 1) }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodTo);

            // Assert
            Assert.Empty(resigned);
        }

        [Fact]
        public void AWOLAndResignedDateBeforePayrollPeriodTo_ShouldNotBeResigned()
        {
            // Arrange
            var payrollPeriodTo = new DateTime(2023, 1, 1);
            var employees = new List<Employee>
            {
                new Employee { ResignStatus = ResignStatus.AWOL, DateResigned = new DateTime(2022, 1, 1) }
            };

            // Act
            var resigned = employees.Resigned(payrollPeriodTo);

            // Assert
            Assert.Empty(resigned);
        }
    }
}
