using JPRSC.HRIS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace JPRSC.HRIS.Test.Infrastructure.Data
{
    public class TableBuilderTest
    {
        [Fact]
        public void BuildTable_ShouldBuildTable_01()
        {
            var items = new List<Sample>
            {
                new Sample { Name = "John", Date = new DateTime(2023, 2, 6), Number = 9 },
                new Sample { Name = "Jane", Date = null, Number = 5 }
            };
            var sampleBuilder = new SampleBuilder();
            var lines = sampleBuilder.Build(items);

            Assert.Equal(4, lines.Count);

            var firstLine = lines[0];
            Assert.Equal(3, firstLine.Count);
            Assert.Equal("Name", firstLine[0]);
            Assert.Equal("Date", firstLine[1]);
            Assert.Equal("Number", firstLine[2]);

            var secondLine = lines[1];
            Assert.Equal(3, secondLine.Count);
            Assert.Equal("John", secondLine[0]);
            Assert.Equal("2/6/2023 12:00:00 AM", secondLine[1]);
            Assert.Equal("9", secondLine[2]);
            
            var thirdLine = lines[2];
            Assert.Equal(3, thirdLine.Count);
            Assert.Equal("Jane", thirdLine[0]);
            Assert.Equal("", thirdLine[1]);
            Assert.Equal("5", thirdLine[2]);

            var fourthLine = lines[3];
            Assert.Equal(3, fourthLine.Count);
            Assert.Equal("", fourthLine[0]);
            Assert.Equal("", fourthLine[1]);
            Assert.Equal("14", fourthLine[2]);
        }
    }

    internal class Sample
    {
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public int Number { get; set; }
    }

    internal class SampleBuilder : TableBuilder<Sample>
    {
        public SampleBuilder()
        {
            Column("Name", item => item.Name);
            Column("Date", item => item.Date.HasValue ? item.Date.Value.ToString() : String.Empty);
            Column("Number", item => item.Number.ToString(), items => items.Sum(i => i.Number).ToString());
        }
    }
}
