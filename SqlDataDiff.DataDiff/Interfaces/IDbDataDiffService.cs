using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlDataDiff.DataDiff.Interfaces
{
    public interface IDbDataDiffService
    {
        (bool, bool, string) IsDatabasesDataIdentical(IDbConnection dbSourceConnection, IDbConnection dbTargetConnection);
    }
}
