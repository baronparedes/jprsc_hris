using JPRSC.HRIS.Infrastructure.CSV;
using Xunit;

namespace JPRSC.HRIS.Test.Infrastructure.CSV
{
    public class CSVValidatorTest
    {
        [Fact]
        public void IsValidCSVLine_GivenValidChars_ShouldReturnTrue()
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

        [Fact]
        public void IsValidCSVLine_GivenValidHeader_ShouldReturnTrue()
        {
            var line = "Employee Code,Last Name,First Name,Days Worked,Hours Worked,Minutes Late,Minutes Undertime,ND_SHF8,ROT,ND_LHF8,OT_SHRDF8-1,OT_LHRDF8-1,ND_LHRDF8-1,ND_LHRDX8-1,ND_LHX8-1,ND_RDF8-1,ND_RDX8-1,ND_SHRDF8-1,DOD,ND_SHRDX8-1,ND_SHX8-1,SHDODOT,LHDOD,LHDODOT,ADJ_PAY,EXC_DOD,HOLIDAY,ND_LHX8,ND_LHRDX8,ND_LHRDF8,ND_LHF8-1,NDX8,NDF8,ND_RDX8,ND_RDF8,ND_SHX8,ND_SHRDX8,ND_SHRDF8,ND_SHF8-1,ND_B2020,OT_LHX8,OT_LHRDX8,OT_LHRDF8,OT_LHF8,OT_RDX8,OT_RDF8,OT_SHF8M,OT_SHX8,OT_SHRDX8,OT_SHRDF8,OT_SHF8,UWLH_B2020,OT_LHRDF8B,DOD_NO_COLA,OT_LHLHF8,OT_LHLHX8,ND_LHLHF8,ND_LHLHX8,ND_LHLHF8-1,ND_LHLHX8-1";

            var isValid = CSVValidator.IsValidCSVLine(line);

            Assert.True(isValid);
        }

        [Fact]
        public void IsValidCSVLine_GivenValidLine_ShouldReturnTrue()
        {
            var line = "52022,BARRAMEDA,RYAN,12,,62,,,28,,,,,,,,,,,,,,,,,,8,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,";

            var isValid = CSVValidator.IsValidCSVLine(line);

            Assert.True(isValid);
        }
    }
}
