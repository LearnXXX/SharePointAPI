using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Tools
{
    class AnalysisIndexDBSize
    {
        //tb_head_index tb_body_index
        private static Dictionary<string, long> tb_index;

        private static Dictionary<string, long> InitlizeMapping()
        {
            return new Dictionary<string, long>() {
                        { "COL_ID",0},
            { "COL_SITE_URL",0},
            { "COL_FLAG",0},
            { "COL_TYPE",0},
            { "COL_NAME",0},
            { "COL_PLANID",0},
            { "COL_JOBID",0},
            { "COL_CYCLEID",0},
            { "COL_JOBTYPE",0},
            { "COL_PATH_MD5",0},
            { "COL_PARENT_PATH_MD5",0},
            { "COL_DATA_FILE_NUMBER",0},
            { "COL_DATA_FILE_OFFSET",0},
            { "COL_DATA_FILE_LENGTH",0},
            { "COL_DATA_FILE_PREFIX_NUMBER",0},
            { "COL_CRC",0},
            { "COL_BACKUP_TYPE",0},
            { "COL_BACKUP_TIME",0},
            { "COL_CREATE_TIME",0},
            { "COL_MODIFY_TIME",0},
            { "COL_RECYCLE_TIME",0},
            { "COL_AUTHOR",0},
            { "COL_PERMISSION",0},
            { "COL_RETENTION",0},
            { "COL_SUB_RETENTION",0},
            { "COL_ATTRIBUTES",0},
            { "COL_EXTRAINFO",0},
            { "COL_ITEMID",0},
            { "COL_ISSYSTEMFILE",0},
            { "COL_STUBINFO",0},
            { "COL_FILE_MD5",0},
            { "COL_SEQUENCE",0},
            { "COL_EXTENSION_1",0},
            { "COL_EXTENSION_2",0},
            { "COL_EXTENSION_3",0},
            { "COL_EXTENSION_4",0},
            { "COL_EXTENSION_5",0},
            { "COL_EXTENSION_6",0},
            { "COL_EXTENSION_7",0},
            { "COL_EXTENSION_8",0},
            { "COL_EXTENSION_9",0},
            { "COL_EXTENSION_10",0},
            { "COL_CONTENT_DATA_OFFSET",0},
            { "COL_CONTENT_DATA_FILE_NUMBER",0},
            { "COL_CONTENT_DATA_FILE_PREFIX_NUMBER",0},
            { "COL_STORAGEINFO",0},
            { "COL_HAD_OFFSET",0},
            { "COL_VERSION",0},
            { "COL_META_DATA_HEADER_OFFSET",0},
            { "COL_CONTENT_DATA_HEADER_OFFSET",0},
            { "COL_CONTENT_PAGE_SIZE",0},
            { "COL_PLATFORM_TYPE",0},
            { "COL_APP_DATA_NAME",0},
            { "COL_CONTENT_SIZE",0},
            { "COL_CREATED_BY",0},
            { "COL_CREATED_TIME",0},
            { "COL_MODIFIED_BY",0},

        };
        }
        public static void Start(string indexDbPath)
        {
            string connectionString = $"Data Source = {indexDbPath}";
            DbProviderFactory providerFactory = DbProviderFactories.GetFactory("System.Data.SQLite");
            var dbConnection = providerFactory.CreateConnection();
            dbConnection.ConnectionString = connectionString;
            dbConnection.Open();
            AnalysisTableV2(dbConnection, "tb_body_index");
            AnalysisTableV2(dbConnection, "tb_head_index");
            //AnalysisTable(dbConnection, "tb_body_index");
            //AnalysisTable(dbConnection, "tb_head_index");
        }

        private static void AnalysisTableV2(DbConnection dbConnection, string tableName)
        {
            var tableColumnMapping = InitlizetableColumnMapping(dbConnection, "tb_body_index");

            var queryString = new StringBuilder($"SELECT ");
            foreach (var column in tableColumnMapping)
            {
                queryString.Append($" length({column.Key}) as {column.Key} ,");
            }
            queryString.Length--;//remove last ,
            queryString.Append($" FROM {tableName} ");
            queryString.Append(" limit {0} offset {0}*{1}");


            long pageIndex = 0;
            long limite = 5000;
            using (var command = dbConnection.CreateCommand())
            {
                var dataTable = new DataTable();
                do
                {
                    dataTable = new DataTable();
                    command.CommandText = string.Format(queryString.ToString(), limite, pageIndex);
                    using (var dataReader = command.ExecuteReader())
                    {
                        dataTable.Load(dataReader);
                        foreach (DataRow row in dataTable.Rows)
                        {
                            Dictionary<string, long> temp = new Dictionary<string, long>();
                            foreach (var length in tableColumnMapping)
                            {
                                temp[length.Key] = length.Value + GetRow(row, length.Key);
                            }
                            tableColumnMapping = temp;
                        }
                    }
                    pageIndex++;
                } while (dataTable.Rows.Count == limite);

                Report(tableColumnMapping, tableName);
            }


        }
        private static Dictionary<string, long> InitlizetableColumnMapping(DbConnection dbConnection, string tableName)
        {
            var tableColumn = new Dictionary<string, long>();
            using (var command = dbConnection.CreateCommand())
            {
                var dataTable = new DataTable();
                command.CommandText = $"PRAGMA table_info([{tableName}])";
                using (var dataReader = command.ExecuteReader())
                {
                    dataTable.Load(dataReader);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        tableColumn[row[1].ToString()] = 0;
                    }
                }
            }
            return tableColumn;
        }
        private static void AnalysisTable2(DbConnection dbConnection, string tableName)
        {
            tb_index = InitlizeMapping();
            using (var command = dbConnection.CreateCommand())
            {
                var dataTable = new DataTable();
                command.CommandText = $"SELECT length(COL_ID) as COL_ID, length(COL_SITE_URL) as COL_SITE_URL, length(COL_FLAG) as COL_FLAG, length(COL_TYPE) as COL_TYPE, length(COL_NAME) as COL_NAME, length(COL_PLANID) as COL_PLANID, length(COL_JOBID) as COL_JOBID, length(COL_CYCLEID) as COL_CYCLEID, length(COL_JOBTYPE) as COL_JOBTYPE, length(COL_PATH_MD5) as COL_PATH_MD5, length(COL_PARENT_PATH_MD5) as COL_PARENT_PATH_MD5, length(COL_DATA_FILE_NUMBER) as COL_DATA_FILE_NUMBER, length(COL_DATA_FILE_OFFSET) as COL_DATA_FILE_OFFSET, length(COL_DATA_FILE_LENGTH) as COL_DATA_FILE_LENGTH, length(COL_DATA_FILE_PREFIX_NUMBER) as COL_DATA_FILE_PREFIX_NUMBER, length(COL_CRC) as COL_CRC, length(COL_BACKUP_TYPE) as COL_BACKUP_TYPE, length(COL_BACKUP_TIME) as COL_BACKUP_TIME, length(COL_CREATE_TIME) as COL_CREATE_TIME, length(COL_MODIFY_TIME) as COL_MODIFY_TIME, length(COL_RECYCLE_TIME) as COL_RECYCLE_TIME, length(COL_AUTHOR) as COL_AUTHOR, length(COL_PERMISSION) as COL_PERMISSION, length(COL_RETENTION) as COL_RETENTION, length(COL_SUB_RETENTION) as COL_SUB_RETENTION, length(COL_ATTRIBUTES) as COL_ATTRIBUTES, length(COL_EXTRAINFO) as COL_EXTRAINFO, length(COL_ITEMID) as COL_ITEMID, length(COL_ISSYSTEMFILE) as COL_ISSYSTEMFILE, length(COL_STUBINFO) as COL_STUBINFO, length(COL_FILE_MD5) as COL_FILE_MD5, length(COL_SEQUENCE) as COL_SEQUENCE, length(COL_EXTENSION_1) as COL_EXTENSION_1, length(COL_EXTENSION_2) as COL_EXTENSION_2, length(COL_EXTENSION_3) as COL_EXTENSION_3, length(COL_EXTENSION_4) as COL_EXTENSION_4, length(COL_EXTENSION_5) as COL_EXTENSION_5, length(COL_EXTENSION_6) as COL_EXTENSION_6, length(COL_EXTENSION_7) as COL_EXTENSION_7, length(COL_EXTENSION_8) as COL_EXTENSION_8, length(COL_EXTENSION_9) as COL_EXTENSION_9, length(COL_EXTENSION_10) as COL_EXTENSION_10, length(COL_CONTENT_DATA_OFFSET) as COL_CONTENT_DATA_OFFSET, length(COL_CONTENT_DATA_FILE_NUMBER) as COL_CONTENT_DATA_FILE_NUMBER, length(COL_CONTENT_DATA_FILE_PREFIX_NUMBER) as COL_CONTENT_DATA_FILE_PREFIX_NUMBER, length(COL_STORAGEINFO) as COL_STORAGEINFO, length(COL_HAD_OFFSET) as COL_HAD_OFFSET, length(COL_VERSION) as COL_VERSION, length(COL_META_DATA_HEADER_OFFSET) as COL_META_DATA_HEADER_OFFSET, length(COL_CONTENT_DATA_HEADER_OFFSET) as COL_CONTENT_DATA_HEADER_OFFSET, length(COL_CONTENT_PAGE_SIZE) as COL_CONTENT_PAGE_SIZE, length(COL_PLATFORM_TYPE) as COL_PLATFORM_TYPE, length(COL_APP_DATA_NAME) as COL_APP_DATA_NAME, length(COL_CONTENT_SIZE) as COL_CONTENT_SIZE, length(COL_CREATED_BY) as COL_CREATED_BY, length(COL_CREATED_TIME) as COL_CREATED_TIME, length(COL_MODIFIED_BY) as COL_MODIFIED_BY FROM {tableName} ";
                int count = 0;
                using (var dataReader = command.ExecuteReader())
                {
                    try
                    {
                        while (dataReader.Read())
                        {
                            count++;
                            Dictionary<string, long> temp = new Dictionary<string, long>();
                            foreach (var length in tb_index)
                            {
                                temp[length.Key] = length.Value + GetRow(dataReader, length.Key);
                            }
                            tb_index = temp;
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
                Report(tb_index, tableName);
            }
        }

        private static void AnalysisTable(DbConnection dbConnection, string tableName)
        {
            tb_index = InitlizeMapping();
            using (var command = dbConnection.CreateCommand())
            {
                var dataTable = new DataTable();
                command.CommandText = $"SELECT length(COL_ID) as COL_ID, length(COL_SITE_URL) as COL_SITE_URL, length(COL_FLAG) as COL_FLAG, length(COL_TYPE) as COL_TYPE, length(COL_NAME) as COL_NAME, length(COL_PLANID) as COL_PLANID, length(COL_JOBID) as COL_JOBID, length(COL_CYCLEID) as COL_CYCLEID, length(COL_JOBTYPE) as COL_JOBTYPE, length(COL_PATH_MD5) as COL_PATH_MD5, length(COL_PARENT_PATH_MD5) as COL_PARENT_PATH_MD5, length(COL_DATA_FILE_NUMBER) as COL_DATA_FILE_NUMBER, length(COL_DATA_FILE_OFFSET) as COL_DATA_FILE_OFFSET, length(COL_DATA_FILE_LENGTH) as COL_DATA_FILE_LENGTH, length(COL_DATA_FILE_PREFIX_NUMBER) as COL_DATA_FILE_PREFIX_NUMBER, length(COL_CRC) as COL_CRC, length(COL_BACKUP_TYPE) as COL_BACKUP_TYPE, length(COL_BACKUP_TIME) as COL_BACKUP_TIME, length(COL_CREATE_TIME) as COL_CREATE_TIME, length(COL_MODIFY_TIME) as COL_MODIFY_TIME, length(COL_RECYCLE_TIME) as COL_RECYCLE_TIME, length(COL_AUTHOR) as COL_AUTHOR, length(COL_PERMISSION) as COL_PERMISSION, length(COL_RETENTION) as COL_RETENTION, length(COL_SUB_RETENTION) as COL_SUB_RETENTION, length(COL_ATTRIBUTES) as COL_ATTRIBUTES, length(COL_EXTRAINFO) as COL_EXTRAINFO, length(COL_ITEMID) as COL_ITEMID, length(COL_ISSYSTEMFILE) as COL_ISSYSTEMFILE, length(COL_STUBINFO) as COL_STUBINFO, length(COL_FILE_MD5) as COL_FILE_MD5, length(COL_SEQUENCE) as COL_SEQUENCE, length(COL_EXTENSION_1) as COL_EXTENSION_1, length(COL_EXTENSION_2) as COL_EXTENSION_2, length(COL_EXTENSION_3) as COL_EXTENSION_3, length(COL_EXTENSION_4) as COL_EXTENSION_4, length(COL_EXTENSION_5) as COL_EXTENSION_5, length(COL_EXTENSION_6) as COL_EXTENSION_6, length(COL_EXTENSION_7) as COL_EXTENSION_7, length(COL_EXTENSION_8) as COL_EXTENSION_8, length(COL_EXTENSION_9) as COL_EXTENSION_9, length(COL_EXTENSION_10) as COL_EXTENSION_10, length(COL_CONTENT_DATA_OFFSET) as COL_CONTENT_DATA_OFFSET, length(COL_CONTENT_DATA_FILE_NUMBER) as COL_CONTENT_DATA_FILE_NUMBER, length(COL_CONTENT_DATA_FILE_PREFIX_NUMBER) as COL_CONTENT_DATA_FILE_PREFIX_NUMBER, length(COL_STORAGEINFO) as COL_STORAGEINFO, length(COL_HAD_OFFSET) as COL_HAD_OFFSET, length(COL_VERSION) as COL_VERSION, length(COL_META_DATA_HEADER_OFFSET) as COL_META_DATA_HEADER_OFFSET, length(COL_CONTENT_DATA_HEADER_OFFSET) as COL_CONTENT_DATA_HEADER_OFFSET, length(COL_CONTENT_PAGE_SIZE) as COL_CONTENT_PAGE_SIZE, length(COL_PLATFORM_TYPE) as COL_PLATFORM_TYPE, length(COL_APP_DATA_NAME) as COL_APP_DATA_NAME, length(COL_CONTENT_SIZE) as COL_CONTENT_SIZE, length(COL_CREATED_BY) as COL_CREATED_BY, length(COL_CREATED_TIME) as COL_CREATED_TIME, length(COL_MODIFIED_BY) as COL_MODIFIED_BY FROM {tableName} ";
                using (var dataReader = command.ExecuteReader())
                {
                    dataTable.Load(dataReader);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        Dictionary<string, long> temp = new Dictionary<string, long>();
                        foreach (var length in tb_index)
                        {
                            temp[length.Key] = length.Value + GetRow(row, length.Key);
                        }
                        tb_index = temp;
                    }
                }
                Report(tb_index, tableName);
            }
        }

        private static void Analysis_tb_body_index(DbConnection dbConnection)
        {
            using (var command = dbConnection.CreateCommand())
            {
                var dataTable = new DataTable();
                command.CommandText = "SELECT length(COL_ID) as COL_ID, length(COL_SITE_URL) as COL_SITE_URL, length(COL_FLAG) as COL_FLAG, length(COL_TYPE) as COL_TYPE, length(COL_NAME) as COL_NAME, length(COL_PLANID) as COL_PLANID, length(COL_JOBID) as COL_JOBID, length(COL_CYCLEID) as COL_CYCLEID, length(COL_JOBTYPE) as COL_JOBTYPE, length(COL_PATH_MD5) as COL_PATH_MD5, length(COL_PARENT_PATH_MD5) as COL_PARENT_PATH_MD5, length(COL_DATA_FILE_NUMBER) as COL_DATA_FILE_NUMBER, length(COL_DATA_FILE_OFFSET) as COL_DATA_FILE_OFFSET, length(COL_DATA_FILE_LENGTH) as COL_DATA_FILE_LENGTH, length(COL_DATA_FILE_PREFIX_NUMBER) as COL_DATA_FILE_PREFIX_NUMBER, length(COL_CRC) as COL_CRC, length(COL_BACKUP_TYPE) as COL_BACKUP_TYPE, length(COL_BACKUP_TIME) as COL_BACKUP_TIME, length(COL_CREATE_TIME) as COL_CREATE_TIME, length(COL_MODIFY_TIME) as COL_MODIFY_TIME, length(COL_RECYCLE_TIME) as COL_RECYCLE_TIME, length(COL_AUTHOR) as COL_AUTHOR, length(COL_PERMISSION) as COL_PERMISSION, length(COL_RETENTION) as COL_RETENTION, length(COL_SUB_RETENTION) as COL_SUB_RETENTION, length(COL_ATTRIBUTES) as COL_ATTRIBUTES, length(COL_EXTRAINFO) as COL_EXTRAINFO, length(COL_ITEMID) as COL_ITEMID, length(COL_ISSYSTEMFILE) as COL_ISSYSTEMFILE, length(COL_STUBINFO) as COL_STUBINFO, length(COL_FILE_MD5) as COL_FILE_MD5, length(COL_SEQUENCE) as COL_SEQUENCE, length(COL_EXTENSION_1) as COL_EXTENSION_1, length(COL_EXTENSION_2) as COL_EXTENSION_2, length(COL_EXTENSION_3) as COL_EXTENSION_3, length(COL_EXTENSION_4) as COL_EXTENSION_4, length(COL_EXTENSION_5) as COL_EXTENSION_5, length(COL_EXTENSION_6) as COL_EXTENSION_6, length(COL_EXTENSION_7) as COL_EXTENSION_7, length(COL_EXTENSION_8) as COL_EXTENSION_8, length(COL_EXTENSION_9) as COL_EXTENSION_9, length(COL_EXTENSION_10) as COL_EXTENSION_10, length(COL_CONTENT_DATA_OFFSET) as COL_CONTENT_DATA_OFFSET, length(COL_CONTENT_DATA_FILE_NUMBER) as COL_CONTENT_DATA_FILE_NUMBER, length(COL_CONTENT_DATA_FILE_PREFIX_NUMBER) as COL_CONTENT_DATA_FILE_PREFIX_NUMBER, length(COL_STORAGEINFO) as COL_STORAGEINFO, length(COL_HAD_OFFSET) as COL_HAD_OFFSET, length(COL_VERSION) as COL_VERSION, length(COL_META_DATA_HEADER_OFFSET) as COL_META_DATA_HEADER_OFFSET, length(COL_CONTENT_DATA_HEADER_OFFSET) as COL_CONTENT_DATA_HEADER_OFFSET, length(COL_CONTENT_PAGE_SIZE) as COL_CONTENT_PAGE_SIZE, length(COL_PLATFORM_TYPE) as COL_PLATFORM_TYPE, length(COL_APP_DATA_NAME) as COL_APP_DATA_NAME, length(COL_CONTENT_SIZE) as COL_CONTENT_SIZE, length(COL_CREATED_BY) as COL_CREATED_BY, length(COL_CREATED_TIME) as COL_CREATED_TIME, length(COL_MODIFIED_BY) as COL_MODIFIED_BY FROM tb_body_index ";
                using (var dataReader = command.ExecuteReader())
                {
                    dataTable.Load(dataReader);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        Dictionary<string, long> temp = new Dictionary<string, long>();
                        foreach (var length in tb_index)
                        {
                            temp[length.Key] = length.Value + GetRow(row, length.Key);
                        }
                        tb_index = temp;
                    }
                }
                Report(tb_index, "tb_body_index");
            }
        }
        private static void Report(Dictionary<string, long> table, string tableName)
        {
            long total = 0;
            foreach (var data in table)
            {
                total += data.Value;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{tableName}:");
            foreach (var data in table.OrderByDescending(data => data.Value).ToDictionary(data => data.Key, data => data.Value))
            {
                var percent = Math.Round(((double)(data.Value * 100)) / (double)total, 2);
                stringBuilder.AppendLine($"{data.Key}: {percent}%");
            }
            System.IO.File.WriteAllText($@"C:\Users\xluo\Desktop\{tableName}Report.txt", stringBuilder.ToString());

        }

        private static long GetRow(DbDataReader row, string columName)
        {
            if (row[columName] is DBNull)
            {
                return 0;
            }
            return (long)row[columName];
        }

        private static long GetRow(DataRow row, string columName)
        {
            if (row[columName] is DBNull)
            {
                return 0;
            }
            return (long)row[columName];
        }
    }
}
