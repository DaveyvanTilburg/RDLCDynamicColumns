using System.IO;
using FluentAssertions;
using Xunit;

namespace RDLCDynamicColumns.UnitTests
{
    public class RDLCColumnsTest
    {
        [Fact]
        public void Test()
        {
            string reportXml = File.ReadAllText("./input.xml");

            var columns = new RDLCColumns(reportXml, "SomeTable", true, false, true);
            string result = columns.UpdatedXml();

            string expectedResult = File.ReadAllText("./result.xml");
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}