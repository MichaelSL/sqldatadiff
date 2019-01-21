using AutoFixture;
using FluentAssertions;
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

        [Fact]
        public void TestOnRandomData()
        {
            var rowsList1 = new List<List<object>>();
            var row1 = new List<object> { 1, "string1", new DateTime(2019, 01, 19) };
            rowsList1.Add(row1);

            var rowsList2 = new List<List<object>>();
            var row2 = new List<object> { 2, "string2", new DateTime(2019, 01, 19) };
            rowsList2.Add(row1);
            rowsList2.Add(row2);

            var dt1 = dataUtility.GetTableWithTestData(typeof(int), new List<Type>() { typeof(string), typeof(DateTime) }, rowsList1);
            var dt2 = dataUtility.GetTableWithTestData(typeof(int), new List<Type>() { typeof(string), typeof(DateTime) }, rowsList2);

            var service = new DataDiffService(new QueryFormatter(), new TableSchemaValidatorsComposite(new[] { new SamePrimaryKeysValidator() }));

            var (sucess, resSql) = service.GetDataDiffSql(dt1, dt2, false);

            var exp = "INSERT INTO TestTable(Id, ColString, ColDateTime) VALUES (2, 'string2', '2019-01-19 00:00:00');\r\n";

            sucess.Should().BeTrue();
            resSql.Should().Be(exp);
        }
    }
}
