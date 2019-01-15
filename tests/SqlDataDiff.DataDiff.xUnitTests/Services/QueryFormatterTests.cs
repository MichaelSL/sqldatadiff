using FluentAssertions;
using SqlDataDiff.DataDiff.Extensions;
using SqlDataDiff.DataDiff.Implementations;
using SqlDataDiff.DataDiff.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xunit;

namespace SqlDataDiff.DataDiff.xUnitTests.Services
{
    public class QueryFormatterTests
    {
        private readonly DataTestFixtureUtility dataUtility;

        public QueryFormatterTests()
        {
            dataUtility = new DataTestFixtureUtility();
        }

        private DataTable GetDataTable(DataColumn[] dataColumns) => dataUtility.GetDataTableWithColumns(dataColumns);

        #region GetRowValuesString
        [Fact]
        public void GetRowValuesString_IntegerValue_ShouldNotBeWrappedByQuotes()
        {
            var data = 1;
            var dataColumns = new DataColumn[] { new DataColumn("Col1", typeof(int)) };
            var dataTable = GetDataTable(dataColumns);
            dataTable.Rows.Add(data);

            IQueryFormatter queryFormatter = new QueryFormatter();
            queryFormatter.GetRowValuesString(dataTable.Rows[0]).Should().BeEquivalentTo($"{data}");
            queryFormatter.GetRowValuesString(dataTable.Rows[0]).Should().NotContain("'");
        }

        [Fact]
        public void GetRowValuesString_DoubleValue_ShouldNotBeWrappedByQuotes()
        {
            var data = 1.1;
            var dataColumns = new DataColumn[] { new DataColumn("Col1", typeof(double)) };
            var dataTable = GetDataTable(dataColumns);
            dataTable.Rows.Add(data);

            IQueryFormatter queryFormatter = new QueryFormatter();
            queryFormatter.GetRowValuesString(dataTable.Rows[0]).Should().BeEquivalentTo($"{data}");
            queryFormatter.GetRowValuesString(dataTable.Rows[0]).Should().NotContain("'");
        }

        [Fact]
        public void GetRowValuesString_DecimalValue_ShouldNotBeWrappedByQuotes()
        {
            var data = 1.1m;
            var dataColumns = new DataColumn[] { new DataColumn("Col1", typeof(decimal)) };
            var dataTable = GetDataTable(dataColumns);
            dataTable.Rows.Add(data);

            IQueryFormatter queryFormatter = new QueryFormatter();
            queryFormatter.GetRowValuesString(dataTable.Rows[0]).Should().BeEquivalentTo($"{data}");
            queryFormatter.GetRowValuesString(dataTable.Rows[0]).Should().NotContain("'");
        }

        [Fact]
        public void GetRowValuesString_StringValue_ShouldBeWrappedByQuotes()
        {
            var data = "foo";
            var dataColumns = new DataColumn[] { new DataColumn("Col1", typeof(string)) };
            var dataTable = GetDataTable(dataColumns);
            dataTable.Rows.Add(data);

            IQueryFormatter queryFormatter = new QueryFormatter();
            queryFormatter.GetRowValuesString(dataTable.Rows[0]).Should().BeEquivalentTo($"'{data}'");
        }

        [Fact]
        public void GetRowValuesString_DateTimeValue_ShouldBeWrappedByQuotes()
        {
            var data = DateTime.Now;
            var dataColumns = new DataColumn[] { new DataColumn("Col1", typeof(DateTime)) };
            var dataTable = GetDataTable(dataColumns);
            dataTable.Rows.Add(data);

            IQueryFormatter queryFormatter = new QueryFormatter();
            queryFormatter.GetRowValuesString(dataTable.Rows[0]).Should().StartWith("'");
            queryFormatter.GetRowValuesString(dataTable.Rows[0]).Should().EndWith("'");
        }

        [Theory]
        [MemberData(nameof(GetDatesTestData))]
        public void GetRowValuesString_DateTimeValue_ShouldHaveSqlFormatting(DateTime dateTime, string expectedFormat)
        {
            var dataColumns = new DataColumn[] { new DataColumn("Col1", typeof(DateTime)) };
            var dataTable = GetDataTable(dataColumns);
            dataTable.Rows.Add(dateTime);

            IQueryFormatter queryFormatter = new QueryFormatter();
            queryFormatter.GetRowValuesString(dataTable.Rows[0]).Should().BeEquivalentTo($"'{expectedFormat}'");
        }

        [Fact]
        public void GetRowValuesString_ShouldProduceNull_IfColumnIsNullableAndValueIsNull()
        {
            var data = new object[] { null, null, null };
            var col1 = new DataColumn("Col1", typeof(string));
            col1.AllowDBNull = true;
            var col2 = new DataColumn("Col2", typeof(DateTime));
            col2.AllowDBNull = true;
            var col3 = new DataColumn("Col3", typeof(int));
            col3.AllowDBNull = true;
            var dataColumns = new DataColumn[] { col1, col2, col3 };
            var dataTable = GetDataTable(dataColumns);
            dataTable.Rows.Add(data);

            IQueryFormatter queryFormatter = new QueryFormatter();
            queryFormatter.GetRowValuesString(dataTable.Rows[0]).Should().BeEquivalentTo($"NULL, NULL, NULL");
        }

        public static IEnumerable<object[]> GetDatesTestData()
        {
            var allData = new List<object[]>
            {
                new object[] { new DateTime(2019, 1, 1), "2019-01-01 00:00:00" },
                new object[] { new DateTime(2019, 10, 1), "2019-10-01 00:00:00" },
                new object[] { new DateTime(2019, 1, 10), "2019-01-10 00:00:00" },
                new object[] { new DateTime(2019, 1, 1, 7, 7, 10), "2019-01-01 07:07:10" },
            };

            return allData;
        }
        #endregion

        #region GetColumnsListString
        [Fact]
        public void ColumnListString_ShouldReturnColumnsList()
        {
            var col1 = new DataColumn("Col1", typeof(string));
            col1.AllowDBNull = true;
            var col2 = new DataColumn("Col2", typeof(DateTime));
            col2.AllowDBNull = true;
            var col3 = new DataColumn("Col3", typeof(int));
            col3.AllowDBNull = true;
            var dataColumns = new DataColumn[] { col1, col2, col3 };
            var dataTable = GetDataTable(dataColumns);

            IQueryFormatter queryFormatter = new QueryFormatter();
            queryFormatter.GetColumnsListString(dataTable.Columns.ToList()).Should().BeEquivalentTo("Col1, Col2, Col3");
        }
        #endregion
    }
}
