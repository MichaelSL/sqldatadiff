using SqlDataDiff.DataDiff.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlDataDiff.DataDiff.Implementations
{
    public class QueryFormatter : IQueryFormatter
    {
        public string GetRowValuesString(DataRow dataRow)
        {
            var rowCols = dataRow.Table.Columns;
            List<string> values = new List<string>(dataRow.ItemArray.Length);

            for (int i = 0; i < dataRow.ItemArray.Length; i++)
            {
                if (rowCols[i].AllowDBNull && dataRow.ItemArray[i] == DBNull.Value)
                {
                    values.Add("NULL");
                }
                else
                {
                    if (rowCols[i].DataType == typeof(string))
                    {
                        values.Add($"'{dataRow.ItemArray[i]}'");
                    }
                    else
                    {
                        if (rowCols[i].DataType == typeof(DateTime))
                        {
                            var dateTime = (DateTime)dataRow.ItemArray[i];
                            values.Add($"'{dateTime.ToString("yyyy-MM-dd HH:mm:ss")}'");
                        }
                        else
                        {
                            values.Add($"{dataRow.ItemArray[i]}");
                        }
                    }
                }
            }

            return string.Join(", ", values);
        }
    }
}
