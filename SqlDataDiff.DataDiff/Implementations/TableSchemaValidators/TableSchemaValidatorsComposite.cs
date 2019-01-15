using SqlDataDiff.DataDiff.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlDataDiff.DataDiff.Implementations.TableSchemaValidators
{
    public class TableSchemaValidatorsComposite : ITableSchemaValidator
    {
        private readonly IEnumerable<ITableSchemaValidator> validators;

        public TableSchemaValidatorsComposite(IEnumerable<ITableSchemaValidator> validators)
        {
            this.validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        public (bool, IEnumerable<string>) Validate(DataTable srcTable, DataTable dstTable)
        {
            (bool, List<string>) result = (true, new List<string>());
            foreach (var item in validators)
            {
                (bool, IEnumerable<string>) validationResult = item.Validate(srcTable, dstTable);
                result.Item1 &= validationResult.Item1;
                result.Item2.AddRange(validationResult.Item2 ?? new string[] { });
            }
            return result;
        }
    }
}
