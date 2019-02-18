using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlDataDiff.DataDiff.Interfaces
{
    public interface IDataDiffService
    {
        (bool, string, string) GetDataDiffSql(DataTable srcTable, DataTable targetTable, bool generateIdempotentScript = true);
        (bool, bool, string) TablesDataIdentical(DataTable srcTable, DataTable targetTable);
    }
}
