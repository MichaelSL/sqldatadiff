using AutoFixture;
using SqlDataDiff.DataDiff.Implementations;
using SqlDataDiff.DataDiff.Implementations.TableSchemaValidators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SqlDataDiff.DataDiff.xUnitTests.Services
{
    public class DataDiffServiceTests
    {
        private readonly DataTestFixtureUtility dataUtility;
        private readonly AutoFixture.Fixture fixture = new Fixture();

        public DataDiffServiceTests()
        {
            dataUtility = new DataTestFixtureUtility();
        }

        [Fact(Skip = "Acceptance")]
        public void TestOnRandomData()
        {
            var dt1 = dataUtility.GetTableWithRandomTestData(new[] { typeof(string), typeof(DateTime), typeof(double) }, 15);
            var dt2 = dataUtility.GetTableWithRandomTestData(new[] { typeof(string), typeof(DateTime), typeof(double) }, 10);

            var service = new DataDiffService(new QueryFormatter(), new TableSchemaValidatorsComposite(new[] { new SamePrimaryKeysValidator() }));

            var resSql = service.GetDataDiffSql(dt1, dt2);

            Console.WriteLine(resSql.Item2);
        }
    }
}
