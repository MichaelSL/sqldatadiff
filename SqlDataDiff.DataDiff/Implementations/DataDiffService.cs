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

        public (bool, string, string) GetDataDiffSql(DataTable srcTable, DataTable targetTable, bool generateIdempotentScript = true)
        {
            var (validationResult, errors) = tableSchemaValidator.Validate(srcTable, targetTable);
            if (!validationResult)
            {
                return (validationResult, null, string.Join("\n", errors));
            }

            List<object> processedDestinationRowKeys = new List<object>();
            StringBuilder diffScript = new StringBuilder();

            //first pass UPDATE + INSERT
            for (int i = 0; i < targetTable.Rows.Count; i++)
            {
                string rowUpdateSql = "";

                var keyColumn = targetTable.PrimaryKey.Single();
                var keyValue = targetTable.Rows[i][keyColumn];

                var destinationRow = FindRowByKey(srcTable, keyValue);
                if (destinationRow == null)
                {
                    rowUpdateSql += GetInsertSql(targetTable, targetTable.Rows[i]) + Environment.NewLine;
                }
                else
                {
                    if (!RowsEqual(targetTable.Rows[i], destinationRow))
                    {
                        if (generateIdempotentScript)
                        {
                            rowUpdateSql += GetUpdateSql(targetTable, targetTable.Rows[i], keyValue, generateIdempotentScript) + Environment.NewLine;
                        }
                    }
                    processedDestinationRowKeys.Add(keyValue);
                }

                diffScript.Append(rowUpdateSql);
            }

            //second pass DELETE
            for (int i = 0; i < srcTable.Rows.Count; i++)
            {
                var keyColumn = srcTable.PrimaryKey.Single();
                var keyValue = srcTable.Rows[i][keyColumn];

                if (!processedDestinationRowKeys.Contains(keyValue))
                {
                    diffScript.Append(GetDeleteSql(srcTable, keyValue) + Environment.NewLine + Environment.NewLine);
                }
            }

            return (true, diffScript.ToString(), null);
        }

        public (bool, bool, string) TablesDataIdentical(DataTable srcTable, DataTable targetTable)
        {
            var (validationResult, errors) = tableSchemaValidator.Validate(srcTable, targetTable);
            if (!validationResult)
            {
                return (validationResult, false, string.Join("\n", errors));
            }

            List<object> processedDestinationRowKeys = new List<object>();

            //first pass by rows in target table
            for (int i = 0; i < targetTable.Rows.Count; i++)
            {
                var keyColumn = targetTable.PrimaryKey.Single();
                var keyValue = targetTable.Rows[i][keyColumn];

                var destinationRow = FindRowByKey(srcTable, keyValue);
                if (destinationRow == null)
                {
                    return (true, false, null);
                }
                else
                {
                    //check if data is the same here
                    if (!RowsEqual(targetTable.Rows[i], destinationRow))
                    {
                        return (true, false, null);
                    }
                    processedDestinationRowKeys.Add(keyValue);
                }
            }

            //second pass check if we have not processed rows in source table
            for (int i = 0; i < srcTable.Rows.Count; i++)
            {
                var keyColumn = srcTable.PrimaryKey.Single();
                var keyValue = srcTable.Rows[i][keyColumn];

                if (!processedDestinationRowKeys.Contains(keyValue))
                {
                    return (true, false, null);
                }
            }

            return (true, true, null);
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

        private DataRow FindRowByKey(DataTable table, object key)
        {
            var keyColumn = table.PrimaryKey.Single();
            foreach (DataRow row in table.Rows)
            {
                if (row[keyColumn].Equals(key))
                    return row;
            }

            return null;
        }

        private bool RowsEqual(DataRow row1, DataRow row2)
        {
            return row1.ItemArray.SequenceEqual(row2.ItemArray);
        }
    }
}
