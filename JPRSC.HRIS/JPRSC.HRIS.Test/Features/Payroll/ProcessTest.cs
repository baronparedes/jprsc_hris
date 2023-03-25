using JPRSC.HRIS.Features.Payroll;
using JPRSC.HRIS.Models;
using System.Collections.Generic;
using Xunit;

namespace JPRSC.HRIS.Test.Features.Payroll
{
    public class ProcessTest
    {
        [Fact]
        public void GetMatchingSSSRecord_WithNoOffset_ShouldGetBaseRecord()
        {
            // Arrange
            var deductionBasis = 15m;
            var sssRecords = new List<SSSRecord>
            {
                new SSSRecord { Id = 1, Range1 = 0, Range1End = 9},
                new SSSRecord { Id = 2, Range1 = 10, Range1End = 19 },
                new SSSRecord { Id = 3, Range1 = 20, Range1End = 29 }
            };
            var client = new Client { SSSRangeOffset = 0 };

            // Act
            var matchingSSSRecord = Process.CommandHandler.GetMatchingSSSRecord(deductionBasis, sssRecords, client);

            // Assert
            Assert.Equal(2, matchingSSSRecord.Id);
        }

        [Fact]
        public void GetMatchingSSSRecord_WithPositiveOffset_ShouldGetLowerRecord()
        {
            // Arrange
            var deductionBasis = 15m;
            var sssRecords = new List<SSSRecord>
            {
                new SSSRecord { Id = 1, Range1 = 0, Range1End = 9},
                new SSSRecord { Id = 2, Range1 = 10, Range1End = 19 },
                new SSSRecord { Id = 3, Range1 = 20, Range1End = 29 }
            };
            var client = new Client { SSSRangeOffset = 1 };

            // Act
            var matchingSSSRecord = Process.CommandHandler.GetMatchingSSSRecord(deductionBasis, sssRecords, client);

            // Assert
            Assert.Equal(1, matchingSSSRecord.Id);
        }

        [Fact]
        public void GetMatchingSSSRecord_WithNegativeOffset_ShouldGetHigherRecord()
        {
            // Arrange
            var deductionBasis = 15m;
            var sssRecords = new List<SSSRecord>
            {
                new SSSRecord { Id = 1, Range1 = 0, Range1End = 9},
                new SSSRecord { Id = 2, Range1 = 10, Range1End = 19 },
                new SSSRecord { Id = 3, Range1 = 20, Range1End = 29 }
            };
            var client = new Client { SSSRangeOffset = -1 };

            // Act
            var matchingSSSRecord = Process.CommandHandler.GetMatchingSSSRecord(deductionBasis, sssRecords, client);

            // Assert
            Assert.Equal(3, matchingSSSRecord.Id);
        }
    }
}
