using AutoFixture;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SqlDataDiff.DataDiff.xUnitTests
{
    internal class DataTestFixtureUtility
    {
        internal const string DEFAULT_DATATABLE_NAME = "TestTable";
        internal readonly Fixture fixture = new Fixture();

        internal DataTable GetDataTableWithColumns(DataColumn[] dataColumns)
        {
            var dt = new DataTable(DEFAULT_DATATABLE_NAME);
            dt.Columns.AddRange(dataColumns);
            return dt;
        }

        internal DataTable GetDataTableWithKeys(DataColumn[] keyColumns)
        {
            var dt = new DataTable(DEFAULT_DATATABLE_NAME);
            dt.Columns.AddRange(keyColumns);
            dt.PrimaryKey = keyColumns;
            return dt;
        }

        internal DataTable GetTableWithRandomTestData(IEnumerable<Type> columnTypes, int rowsCount)
        {
            var dt = new DataTable(DEFAULT_DATATABLE_NAME);

            var keyColumn = new DataColumn("Id", typeof(int));

            List<DataColumn> columns = new List<DataColumn>();
            foreach (var t in columnTypes)
            {
                var c = new DataColumn($"Col{t.Name}", t);
                c.AllowDBNull = true;
                columns.Add(c);
            }

            dt.Columns.AddRange(new[] { keyColumn }.Union(columns).ToArray());
            dt.PrimaryKey = new[] { keyColumn };

            var rnd = new Random();
            var pkShift = rnd.Next(rowsCount / 2);

            for (int i = 0; i < rowsCount; i++)
            {
                var key = (object)(i + pkShift);

                var vals = columnTypes.Select(t => new AutoFixture.Kernel.SpecimenContext(fixture).Resolve(t));

                dt.Rows.Add(new[] { key }.Union(vals).ToArray());
            }

            return dt;
        }

        internal DataTable GetTableWithTestData(Type keyDataType, IEnumerable<Type> columnTypes, IEnumerable<IEnumerable<object>> data)
        {
            var dt = new DataTable(DEFAULT_DATATABLE_NAME);

            var keyColumn = new DataColumn("Id", keyDataType);

            List<DataColumn> columns = new List<DataColumn>();
            foreach (var t in columnTypes)
            {
                var c = new DataColumn($"Col{t.Name}", t);
                c.AllowDBNull = true;
                columns.Add(c);
            }

            dt.Columns.AddRange(new[] { keyColumn }.Union(columns).ToArray());
            dt.PrimaryKey = new[] { keyColumn };

            foreach (var row in data)
            {
                dt.Rows.Add(row.ToArray());
            }

            return dt;
        }
    }
}
