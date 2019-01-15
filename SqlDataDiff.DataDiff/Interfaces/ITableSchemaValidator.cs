using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlDataDiff.DataDiff.Interfaces
{
    public interface ITableSchemaValidator
    {
        (bool, IEnumerable<string>) Validate(DataTable srcTable, DataTable dstTable);
    }
}
