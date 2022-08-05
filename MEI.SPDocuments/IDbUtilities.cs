using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MEI.SPDocuments
{
    public interface IDbUtilities
    {
        /// <summary>
        ///     Converts a database <see cref="object" /> <paramref name="value" /> to its appropriate CLR type.
        /// </summary>
        /// <typeparam name="T">The CLR type to convert to.</typeparam>
        /// <param name="value">The database object value.</param>
        /// <returns>A value of type T.</returns>
        T FromDbValue<T>(object value);

        /// <summary>
        ///     Converts a CLR <paramref name="value" /> to an appropriate database object value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The CLR value.</param>
        /// <returns>A database object value.</returns>
        /// <remarks>
        ///     This specifically checks whether a nullable value has a value. If it does, then it will return its value,
        ///     else it will return a DBNull value.
        /// </remarks>
        object ToDbValue<T>(T? value)
            where T : struct;

        /// <summary>
        ///     Converts a CLR <paramref name="value" /> to an appropriate database object value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The CLR value.</param>
        /// <returns>A database object value</returns>
        /// <remarks>
        ///     This specifically checks whether a non-nullable value is nothing. If it is, then it will return a DBNull value,
        ///     else it will return its value.
        /// </remarks>
        object ToDbValue<T>(T value);

        /// <summary>
        ///     Builds a <see cref="SqlDataReader" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="SqlDataReader" /> object.</returns>
        /// <remarks>
        ///     Will return an open <see cref="SqlDataReader" />, so be sure to close is when finished. It is set to automatically
        ///     close the connection when the <see cref="SqlDataReader" /> is closed.
        /// </remarks>
        SqlDataReader ExecuteDataReader(string sql,
                                        IList<SqlParameter> parameters,
                                        string connectionString,
                                        CommandType cmdType);

        /// <summary>
        ///     Builds a <see cref="SqlDataReader" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameter">The parameter to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="SqlDataReader" /> object.</returns>
        /// <remarks>
        ///     Will return an open <see cref="SqlDataReader" />, so be sure to close is when finished. It is set to automatically
        ///     close the connection when the <see cref="SqlDataReader" /> is closed.
        /// </remarks>
        SqlDataReader ExecuteDataReader(string sql,
                                        SqlParameter parameter,
                                        string connectionString,
                                        CommandType cmdType);

        /// <summary>
        ///     Builds a <see cref="SqlDataReader" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="SqlDataReader" /> object.</returns>
        /// <remarks>
        ///     Will return an open <see cref="SqlDataReader" />, so be sure to close is when finished. It is set to automatically
        ///     close the connection when the <see cref="SqlDataReader" /> is closed.
        /// </remarks>
        SqlDataReader ExecuteDataReader(string sql, string connectionString, CommandType cmdType);

        /// <summary>
        ///     Gets an array of strings built from a command using the specified parameters and containing the text from the
        ///     column with the specified <paramref name="columnName" />.
        /// </summary>
        /// <typeparam name="T">The data type of the column.</typeparam>
        /// <param name="sql">The command text.</param>
        /// <param name="columnName">Name of the column to retrieve the value from.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>An array of strings.</returns>
        string[] GetColumnValues<T>(string sql,
                                    string columnName,
                                    IList<SqlParameter> parameters,
                                    string connectionString,
                                    CommandType cmdType);

        /// <summary>
        ///     Gets an array of strings built from a command using the specified parameter and containing the text from the column
        ///     with the specified <paramref name="columnName" />.
        /// </summary>
        /// <typeparam name="T">The data type of the column.</typeparam>
        /// <param name="sql">The command text.</param>
        /// <param name="columnName">Name of the column to retrieve the value from.</param>
        /// <param name="parameter">The parameter to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>An array of string.</returns>
        string[] GetColumnValues<T>(string sql,
                                    string columnName,
                                    SqlParameter parameter,
                                    string connectionString,
                                    CommandType cmdType);

        /// <summary>
        ///     Creates a <see cref="DataSet" /> from the command text using the specified
        ///     <paramref name="parameters" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="DataSet" />.</returns>
        DataSet GetDataSet(string sql,
                           IList<SqlParameter> parameters,
                           string connectionString,
                           CommandType cmdType);

        /// <summary>
        ///     Creates a <see cref="DataTable" /> from the command text using the specified
        ///     <paramref name="parameter" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameter">The parameter to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="DataTable" />.</returns>
        DataTable GetDataTable(string sql, SqlParameter parameter, string connectionString, CommandType cmdType);

        /// <summary>
        ///     Creates a <see cref="DataTable" /> from the command text using the specified
        ///     <paramref name="parameters" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="DataTable" />.</returns>
        DataTable GetDataTable(string sql,
                               IList<SqlParameter> parameters,
                               string connectionString,
                               CommandType cmdType);

        /// <summary>
        ///     Creates a <see cref="DataTable" /> from the command text using the specified <paramref name="parameters" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="DataTable" />.</returns>
        IList<DataTable> GetDataTables(string sql,
                                       IList<SqlParameter> parameters,
                                       string connectionString,
                                       CommandType cmdType);

        /// <summary>
        ///     Executes a Transact-SQL statement against the connection using the specified <paramref name="parameters" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>An <see cref="int" /> set to the number of rows affected.</returns>
        int ExecuteNonQuery(string sql,
                            IList<SqlParameter> parameters,
                            string connectionString,
                            CommandType cmdType);
    }

    /// <summary>
    ///     A utility class for common database procedures.
    /// </summary>
    internal class DbUtilities
        : IDbUtilities
    {
        /// <summary>
        ///     Converts a database <see cref="object" /> <paramref name="value" /> to its appropriate CLR type.
        /// </summary>
        /// <typeparam name="T">The CLR type to convert to.</typeparam>
        /// <param name="value">The database object value.</param>
        /// <returns>A value of type T.</returns>
        public T FromDbValue<T>(object value)
        {
            if (value.GetType() == typeof(T))
            {
                return (T)value;
            }

            return default;
        }

        /// <summary>
        ///     Converts a CLR <paramref name="value" /> to an appropriate database object value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The CLR value.</param>
        /// <returns>A database object value.</returns>
        /// <remarks>
        ///     This specifically checks whether a nullable value has a value. If it does, then it will return its value,
        ///     else it will return a DBNull value.
        /// </remarks>
        public object ToDbValue<T>(T? value)
            where T : struct
        {
            if (!value.HasValue)
            {
                return DBNull.Value;
            }

            return value.Value;
        }

        /// <summary>
        ///     Converts a CLR <paramref name="value" /> to an appropriate database object value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The CLR value.</param>
        /// <returns>A database object value</returns>
        /// <remarks>
        ///     This specifically checks whether a non-nullable value is nothing. If it is, then it will return a DBNull value,
        ///     else it will return its value.
        /// </remarks>
        public object ToDbValue<T>(T value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            return value;
        }

        /// <summary>
        ///     Builds a <see cref="SqlDataReader" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="SqlDataReader" /> object.</returns>
        /// <remarks>
        ///     Will return an open <see cref="SqlDataReader" />, so be sure to close is when finished. It is set to automatically
        ///     close the connection when the <see cref="SqlDataReader" /> is closed.
        /// </remarks>
        public SqlDataReader ExecuteDataReader(string sql,
                                               IList<SqlParameter> parameters,
                                               string connectionString,
                                               CommandType cmdType)
        {
            Preconditions.CheckNotNullOrEmpty("sql", sql);
            Preconditions.CheckNotNullOrEmpty("connectionString", connectionString);

            var conn = new SqlConnection(connectionString);

            conn.Open();

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = cmdType;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        /// <summary>
        ///     Builds a <see cref="SqlDataReader" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameter">The parameter to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="SqlDataReader" /> object.</returns>
        /// <remarks>
        ///     Will return an open <see cref="SqlDataReader" />, so be sure to close is when finished. It is set to automatically
        ///     close the connection when the <see cref="SqlDataReader" /> is closed.
        /// </remarks>
        public SqlDataReader ExecuteDataReader(string sql,
                                               SqlParameter parameter,
                                               string connectionString,
                                               CommandType cmdType)
        {
            var parameters = new List<SqlParameter>
                             {
                                 parameter
                             };

            return ExecuteDataReader(sql, parameters, connectionString, cmdType);
        }

        /// <summary>
        ///     Builds a <see cref="SqlDataReader" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="SqlDataReader" /> object.</returns>
        /// <remarks>
        ///     Will return an open <see cref="SqlDataReader" />, so be sure to close is when finished. It is set to automatically
        ///     close the connection when the <see cref="SqlDataReader" /> is closed.
        /// </remarks>
        public SqlDataReader ExecuteDataReader(string sql,
                                               string connectionString,
                                               CommandType cmdType)
        {
            return ExecuteDataReader(sql, (IList<SqlParameter>)null, connectionString, cmdType);
        }

        /// <summary>
        ///     Gets an array of strings built from a command using the specified parameters and containing the text from the
        ///     column with the specified <paramref name="columnName" />.
        /// </summary>
        /// <typeparam name="T">The data type of the column.</typeparam>
        /// <param name="sql">The command text.</param>
        /// <param name="columnName">Name of the column to retrieve the value from.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>An array of strings.</returns>
        public string[] GetColumnValues<T>(string sql,
                                           string columnName,
                                           IList<SqlParameter> parameters,
                                           string connectionString,
                                           CommandType cmdType)
        {
            Preconditions.CheckNotNullOrEmpty("columnName", columnName);

            var values = new ArrayList();

            using (SqlDataReader rdr = ExecuteDataReader(sql, parameters, connectionString, cmdType))
            {
                while (rdr.Read())
                {
                    values.Add(FromDbValue<T>(rdr[columnName]).ToString());
                }
            }

            return (string[])values.ToArray(typeof(string));
        }

        /// <summary>
        ///     Gets an array of strings built from a command using the specified parameter and containing the text from the column
        ///     with the specified <paramref name="columnName" />.
        /// </summary>
        /// <typeparam name="T">The data type of the column.</typeparam>
        /// <param name="sql">The command text.</param>
        /// <param name="columnName">Name of the column to retrieve the value from.</param>
        /// <param name="parameter">The parameter to us in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>An array of string.</returns>
        public string[] GetColumnValues<T>(string sql,
                                           string columnName,
                                           SqlParameter parameter,
                                           string connectionString,
                                           CommandType cmdType)
        {
            var parameters = new List<SqlParameter>
                             {
                                 parameter
                             };

            return GetColumnValues<T>(sql, columnName, parameters, connectionString, cmdType);
        }

        /// <summary>
        ///     Creates a <see cref="DataSet" /> from the command text using the specified <paramref name="parameters" />.
        /// </summary>
        /// <param name="sql">The commandText.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="DataSet" />.</returns>
        public DataSet GetDataSet(string sql,
                                  IList<SqlParameter> parameters,
                                  string connectionString,
                                  CommandType cmdType)
        {
            Preconditions.CheckNotNullOrEmpty("sql", sql);
            Preconditions.CheckNotNullOrEmpty("connectionString", connectionString);

            var ds = new DataSet();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = cmdType;

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }

            return ds;
        }

        /// <summary>
        ///     Creates a <see cref="DataTable" /> from the command text using the specified
        ///     <paramref name="parameter" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameter">The parameter to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="DataTable" />.</returns>
        public DataTable GetDataTable(string sql, SqlParameter parameter, string connectionString, CommandType cmdType)
        {
            var parameters = new List<SqlParameter>
                             {
                                 parameter
                             };

            return GetDataTable(sql, parameters, connectionString, cmdType);
        }

        /// <summary>
        ///     Creates a <see cref="DataTable" /> from the command text using the specified
        ///     <paramref name="parameters" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="DataTable" />.</returns>
        public DataTable GetDataTable(string sql,
                                      IList<SqlParameter> parameters,
                                      string connectionString,
                                      CommandType cmdType)
        {
            DataSet ds = GetDataSet(sql, parameters, connectionString, cmdType);

            if (ds?.Tables.Count > 0)
            {
                return ds.Tables[0];
            }

            return null;
        }

        /// <summary>
        ///     Creates a <see cref="DataTable" /> from the command text using the specified <paramref name="parameters" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>A <see cref="DataTable" />.</returns>
        public IList<DataTable> GetDataTables(string sql,
                                                   IList<SqlParameter> parameters,
                                                   string connectionString,
                                                   CommandType cmdType)
        {
            DataSet ds = GetDataSet(sql, parameters, connectionString, cmdType);
            var dataTables = new List<DataTable>();

            foreach (DataTable table in ds.Tables)
            {
                dataTables.Add(table);
            }

            return dataTables;
        }

        /// <summary>
        ///     Executes a Transact-SQL statement against the connection. using the specified <paramref name="parameters" />.
        /// </summary>
        /// <param name="sql">The command text.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <param name="connectionString"></param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns>An <see cref="int" /> set to the number of rows affected.</returns>
        public int ExecuteNonQuery(string sql,
                                   IList<SqlParameter> parameters,
                                   string connectionString,
                                   CommandType cmdType)
        {
            Preconditions.CheckNotNullOrEmpty("sql", sql);
            Preconditions.CheckNotNullOrEmpty("connectionString", connectionString);

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = cmdType;

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
