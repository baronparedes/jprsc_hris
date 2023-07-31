using JPRSC.HRIS.Infrastructure.CSV;
using Xunit;

namespace JPRSC.HRIS.Test.Infrastructure.CSV
{
    public class CSVValidatorTest
    {
        [Fact]
        public void IsValidCSVLine_GivenValidLine_ShouldReturnTrue()
        {
            var line = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz`~!@#$%^&*()-_=+[{]};:'\",<.>/? ñÑ";

            var isValid = CSVValidator.IsValidCSVLine(line);

            Assert.True(isValid);
        }

        [Fact]
        public void IsValidCSVLine_GivenInvalidLine_ShouldReturnFalse()
        {
            var line = "PK\u0003\u0004\u0014 \u0006 \b   ! \u001d70º±\u0001  /\b  \u0013 \b\u0002[Content_Types].xml ¢\u0004\u0002(  \u0002    ";

            var isValid = CSVValidator.IsValidCSVLine(line);

            Assert.False(isValid);
        }
    }
}
