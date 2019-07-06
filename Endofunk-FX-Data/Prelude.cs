using System;
using System.Data;

namespace Endofunk.FX.Data {
  public static partial class Prelude {

    #region IDataRecord Conversion
    public static int ToInt(this IDataReader record, string sqlColumnName) => Convert.ToInt32(record[sqlColumnName]);
    public static long ToLong(this IDataReader record, string sqlColumnName) => Convert.ToInt64(record[sqlColumnName]);
    public static string ToString(this IDataReader record, string sqlColumnName) => Convert.ToString(record[sqlColumnName]);
    public static bool ToBoolean(this IDataReader record, string sqlColumnName) => Convert.ToBoolean(record[sqlColumnName]);
    public static double ToSingle(this IDataReader record, string sqlColumnName) => Convert.ToSingle(record[sqlColumnName]);
    public static double ToDouble(this IDataReader record, string sqlColumnName) => Convert.ToDouble(record[sqlColumnName]);
    public static DateTime toDateTime(this IDataReader record, string sqlColumnName) => ((long)Convert.ToInt32(record[sqlColumnName])).UnixTimeSecondsToDateTime();
    #endregion

  }
}
