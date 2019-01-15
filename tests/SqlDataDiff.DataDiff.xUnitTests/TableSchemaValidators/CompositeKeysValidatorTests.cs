using FluentAssertions;
using SqlDataDiff.DataDiff.Implementations.TableSchemaValidators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Xunit;

namespace SqlDataDiff.DataDiff.xUnitTests.TableSchemaValidators
{
    public class CompositeKeysValidatorTests
    {
        private readonly DataTestFixtureUtility dataUtility;

        public CompositeKeysValidatorTests()
        {
            dataUtility = new DataTestFixtureUtility();
        }

        [Fact]
        public void ShouldReturnTrue_WhenThereIsOnlyOneKeyColumn()
        {
            var col1 = new DataColumn("Col1", typeof(string));
            col1.AllowDBNull = true;
            var dataColumns = new DataColumn[] { col1 };
            var anotherColumns = dataColumns.Select(src => new DataColumn(src.ColumnName, src.DataType))
                .ToArray();

            var tableSrc = dataUtility.GetDataTableWithKeys(dataColumns);
            var tableDst = dataUtility.GetDataTableWithKeys(anotherColumns);

            CompositeKeysValidator samePrimaryKeysValidator = new CompositeKeysValidator();
            var (res, error) = samePrimaryKeysValidator.Validate(tableSrc, tableDst);

            res.Should().BeTrue();
            error.Should().BeNull();
        }

        [Fact]
        public void ShouldReturnFalse_WhenThereAreMultipleKeyColumns()
        {
            var col1 = new DataColumn("Col1", typeof(string));
            col1.AllowDBNull = true;
            var col2 = new DataColumn("Col2", typeof(DateTime));
            col2.AllowDBNull = true;
            var col3 = new DataColumn("Col3", typeof(int));
            col3.AllowDBNull = true;
            var dataColumns = new DataColumn[] { col1, col2, col3 };
            var anotherColumns = dataColumns.Select(src => new DataColumn(src.ColumnName, src.DataType))
                .ToArray();

            var tableSrc = dataUtility.GetDataTableWithKeys(dataColumns);
            var tableDst = dataUtility.GetDataTableWithKeys(anotherColumns);

            CompositeKeysValidator samePrimaryKeysValidator = new CompositeKeysValidator();
            var (res, error) = samePrimaryKeysValidator.Validate(tableSrc, tableDst);

            res.Should().BeFalse();
            error.Should().NotBeNullOrEmpty();
        }
    }
}
