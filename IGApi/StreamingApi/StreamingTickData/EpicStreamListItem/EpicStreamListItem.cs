using IGApi.Common;

namespace IGApi
{
    /// <summary>
    /// This object holds Epic data, that is formed such that it can be serialized and deserialized to and from JSON RestQueueParameter.
    /// </summary>
    public class EpicStreamListItem
    {
        public readonly string Epic;

        public readonly bool UsedByOpenPositions;

        public readonly bool usedByWorkingOrders;

        public bool multiUse
        {
            get
            {
                return Utility.CountTrue(
                    UsedByOpenPositions,
                    usedByWorkingOrders) > 1;
            }
        }

        public EpicStreamListItem(
            string epic,
            bool usedByOpenPositions = false,
            bool usedByWorkingOrders = false
            )
        {
            Epic = epic;
            UsedByOpenPositions = usedByOpenPositions;
        }
    }
}
