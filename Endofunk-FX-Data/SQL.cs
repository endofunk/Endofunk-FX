using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Data.SqlClient;
using Endofunk.FX;
using static Endofunk.FX.Prelude;
using System.IO;


namespace Endofunk.FX.Data {
  using DBInstance = Tuple<IDbConnection, IDbCommand>;
  public static class SQL {
    #region New SQLite Database Instance
    /// <summary>
    /// Creates a new SQLite Database file at the specified path.
    /// </summary>
    public static Result<bool> CreateSQLiteDB(string dbFilePath) => Try(() => {
      dbFilePath.DeleteFileIfExists().Match(
        success: (success) => { }, // NOP
        failed: e => throw new IOException(e.SourceException.Message));
      SQLiteConnection.CreateFile(dbFilePath);
      return true;
    });
    #endregion

    #region Connectivity
    /// <summary>
    /// Connect to SQLite instance using a specified connectionString.
    /// </summary>
    public static Result<DBInstance> ConnectToSQLite(string connectionString) => Try(() => {
      var connection = new SQLiteConnection(connectionString);
      connection.Open();
      var command = connection.CreateCommand();
      return new DBInstance(connection, command);
    });


    /// <summary>
    /// Connect to SqlClient instance using a specified connectionString.
    /// </summary>
    public static Result<DBInstance> ConnectToSqlClient(string connectionString) => Try(() => {
      var connection = new SqlConnection(connectionString);
      connection.Open();
      var command = connection.CreateCommand();
      return new DBInstance(connection, command);
    });
    #endregion

    #region Execute Queries
    /// <summary>
    /// Executes the non query.
    /// </summary>
    public static Func<DBInstance, Result<int>> ExecuteNonQuery() => db => Try(() => {
      var (connection, command) = db;
      var result = command.ExecuteNonQuery();
      db.DisposeAndClose();
      return result;
    });

    /// <summary>
    /// Executes the reader.
    /// </summary>
    public static Func<DBInstance, Result<List<T>>> ExecuteReader<T>(Func<IDataReader, T> transform) => db => Try(() => {
      var (connection, command) = db;
      var reader = command.ExecuteReader();
      var result = reader.ToType(transform);
      reader.Dispose();
      db.DisposeAndClose();
      return result;
    });

    /// <summary>
    /// Executes the scalar.
    /// </summary>
    public static Func<DBInstance, Result<T>> ExecuteScalar<T>(Func<object, T> transform) => db => Try(() => {
      var (connection, command) = db;
      var result = transform(command.ExecuteScalar());
      db.DisposeAndClose();
      return result;
    });
    #endregion

    #region Bind SQL or Stored Procedure
    public static Func<DBInstance, Result<DBInstance>> BindSQL(string script) => db => Try(() => {
      var (connection, command) = db;
      command.CommandText = script;
      return new DBInstance(connection, command);
    });

    public static Func<DBInstance, Result<DBInstance>> BindProcedure(string script) => db => Try(() => {
      var (connection, command) = db;
      command.CommandText = script;
      command.CommandType = CommandType.StoredProcedure;
      return new DBInstance(connection, command);
    });
    #endregion

    #region Sql Parameters tied to Values
    public static Func<DBInstance, Result<DBInstance>> Parameters(Action<IDataParameterCollection> fn) => db => Try(() => {
      var (connection, command) = db;
      fn(command.Parameters);
      return new DBInstance(connection, command);
    });
    #endregion

    #region SqlExtension Methods
    public static void DisposeAndClose(this (IDbConnection connection, IDbCommand command) db) {
      db.command.Dispose();
      db.connection.Close();
    }

    public static void DisposeAndClose(this DBInstance db) {
      var (connection, command) = db;
      command.Dispose();
      connection.Close();
    }

    public static List<T> ToType<T>(this IDataReader reader, Func<IDataReader, T> fn) {
      var result = new List<T>();
      while (reader.Read()) {
        result.Add(fn(reader));
      }
      return result;
    }
    #endregion

  }
}
