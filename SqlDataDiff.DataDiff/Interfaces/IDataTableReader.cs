using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlDataDiff.DataDiff.Interfaces
{
    public interface IDataTableReader
    {
        DataTable GetDataTable(string connectionString, string tableName);
    }
}
