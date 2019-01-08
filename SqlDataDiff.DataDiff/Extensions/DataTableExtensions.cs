using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlDataDiff.DataDiff.Extensions
{
    public static class DataTableExtensions
    {
        public static IList<DataColumn> ToList(this DataColumnCollection dataColumnCollection)
        {
            List<DataColumn> columnList = new List<DataColumn>(dataColumnCollection.Count);
            foreach (DataColumn item in dataColumnCollection)
            {
                columnList.Add(item);
            }
            return columnList;
        }
    }
}
