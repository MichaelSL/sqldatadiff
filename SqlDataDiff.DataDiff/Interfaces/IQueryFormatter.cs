using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlDataDiff.DataDiff.Interfaces
{
    public interface IQueryFormatter
    {
        string GetRowValuesString(DataRow dataRow);
        string GetColumnsListString(IEnumerable<DataColumn> dataColumns);
    }
}
