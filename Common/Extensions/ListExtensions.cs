namespace IGApi.Common.Extensions
{
    using static Log;
    internal static partial class Extensions
    {
        public static IEnumerable<List<T>> Split<T>(this List<T> list, int nSize)
        {
            for (int i = 0; i < list.Count; i += nSize)
            {
                yield return list.GetRange(i, Math.Min(nSize, list.Count - i));
            }
        }
    }
}
