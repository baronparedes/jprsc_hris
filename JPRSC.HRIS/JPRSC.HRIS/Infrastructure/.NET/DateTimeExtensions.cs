namespace System
{
    public static class DateTimeExtensions
    {
        public static object AsSqlParameterValue(this DateTime? d)
        {
            if (!d.HasValue || d.Value == DateTime.MinValue)
            {
                return DBNull.Value;
            }
            else
            {
                return d.Value;
            }
        }
    }
}