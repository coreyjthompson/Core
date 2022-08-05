using Microsoft.EntityFrameworkCore.Migrations;

namespace MEI.Core.Infrastructure.Data.Helpers
{
    public static class MigrationBuilderExtensions
    {
        public static void AddTemporalTableSupport(this MigrationBuilder builder, string schema, string tableName, string historyTableSchema)
        {
            builder.Sql(
                $@"ALTER TABLE {schema}.{tableName} ADD 
            SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
            SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
            PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);"
            );

            builder.Sql(
                $@"ALTER TABLE {schema}.{tableName} 
            SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = {historyTableSchema}.{schema}{tableName} ));"
            );
        }
    }
}