using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlDataDiff.DataDiff.Interfaces
{
    public interface IDataDiffService
    {
        (bool, string) GetDataDiffSql(DataTable srcTable, DataTable dstTable, bool generateIdempotentScript = true);
    }
}
