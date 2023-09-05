namespace IGApi.Common.Extensions
{
    using System.Data.SqlTypes;
    using System.Runtime.CompilerServices;
    using static Log;
    internal static partial class Extensions
    {
        public static decimal? TryParseSqlDecimal(this decimal? value, string? source = "N/A", [CallerMemberName] string? caller = null)
        {
            try
            {
                if (value is not null)
                    return ((decimal)value).TryParseSqlDecimal(source, caller);

                return null;
            }
            catch (OverflowException)
            {
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static decimal TryParseSqlDecimal(this decimal value, string? source = "N/A", [CallerMemberName] string? caller = null)
        {
            try
            {
                SqlDecimal sqlMinStepDistance = SqlDecimal.ConvertToPrecScale(new SqlDecimal((decimal)value), 38, 19);

                if (sqlMinStepDistance.Value < decimal.MaxValue)
                    return ((decimal)sqlMinStepDistance.ToDouble());
                else
                    throw new Exception("Impossible error. Value could be parsed but has exceeded decimal.MaxValue.");
            }
            catch (OverflowException ex)
            {
                Log.WriteException(new OverflowException($"Parse failed \"{source}\". Caller = \"{caller}\" value = \"{value}\""));
                throw ex;
            }
        }
    }
}
