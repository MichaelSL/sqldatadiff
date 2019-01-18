using SqlDataDiff.DataDiff.Extensions;
using SqlDataDiff.DataDiff.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SqlDataDiff.DataDiff.Implementations
{
    public class DataDiffService : IDataDiffService
    {
        private readonly IQueryFormatter queryFormatter;
        private readonly ITableSchemaValidator tableSchemaValidator;

        public DataDiffService(IQueryFormatter queryFormatter, ITableSchemaValidator tableSchemaValidator)
        {
            this.queryFormatter = queryFormatter ?? throw new ArgumentNullException(nameof(queryFormatter));
            this.tableSchemaValidator = tableSchemaValidator ?? throw new ArgumentNullException(nameof(tableSchemaValidator));
        }

        public (bool, string) GetDataDiffSql(DataTable srcTable, DataTable dstTable, bool generateIdempotentScript = true)
        {
            var (validationResult, errors) = tableSchemaValidator.Validate(srcTable, dstTable);
            if (!validationResult)
            {
                return (validationResult, string.Join("\n", errors));
            }

            List<object> processedDestinationRowKeys = new List<object>();
            string diffScript = "";

            //first pass UPDATE + INSERT
            for (int i = 0; i < srcTable.Rows.Count; i++)
            {
                string rowUpdateSql = "";

                var keyColumn = srcTable.PrimaryKey.Single();
                var keyValue = srcTable.Rows[i][keyColumn];

                var destinationRow = FindRowByKey(dstTable, keyValue);
                if (destinationRow == null)
                {
                    rowUpdateSql += GetInsertSql(srcTable, srcTable.Rows[i]) + Environment.NewLine;
                }
                else
                {
                    if (generateIdempotentScript)
                    {
                        rowUpdateSql += GetUpdateSql(srcTable, srcTable.Rows[i], keyValue, generateIdempotentScript) + Environment.NewLine;
                    }
                    processedDestinationRowKeys.Add(keyValue);
                }

                diffScript += rowUpdateSql;
            }

            //second pass DELETE
            for (int i = 0; i < dstTable.Rows.Count; i++)
            {
                var keyColumn = dstTable.PrimaryKey.Single();
                var keyValue = dstTable.Rows[i][keyColumn];

                if (!processedDestinationRowKeys.Contains(keyValue))
                {
                    diffScript += GetDeleteSql(dstTable, keyValue) + Environment.NewLine + Environment.NewLine;
                }
            }

            return (true, diffScript);
        }

        private string GetDeleteSql(DataTable table, object keyValue)
        {
            return $"DELETE FROM {table.TableName} WHERE {table.TableName}.{table.PrimaryKey.Single().ColumnName} = {keyValue};";
        }

        private string GetUpdateSql(DataTable table, DataRow row, object keyValue, bool generateIdempotentScript)
        {
            string resSql = "";
            if (generateIdempotentScript)
            {
                resSql += GetDeleteSql(table, keyValue) + Environment.NewLine;
            }
            resSql += GetInsertSql(table, row);
            return resSql;
        }

        private string GetInsertSql(DataTable table, DataRow row)
        {
            return $"INSERT INTO {table.TableName}({queryFormatter.GetColumnsListString(table.Columns.ToList())}) VALUES ({queryFormatter.GetRowValuesString(row)});";
        }

        private object FindRowByKey(DataTable table, object key)
        {
            var keyColumn = table.PrimaryKey.Single();
            foreach (DataRow row in table.Rows)
            {
                if (row[keyColumn].Equals(key))
                    return row;
            }

            return null;
        }
    }
}
