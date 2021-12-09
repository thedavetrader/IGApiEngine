using IGApi.Common;

namespace IGApi
{
    /// <summary>
    /// This object holds Epic data, that is formed such that it can be serialized and deserialized to and from JSON RestQueueParameter.
    /// </summary>
    public class EpicStreamListItem
    {
        public readonly string Epic;

        private bool _sourceOpenPositions;

        private bool _sourceWorkingOrders;

        public enum EpicStreamListItemSource {
            SourceOpenPositions,
            SourceWorkingOrders,
            None
        }

        public bool multiUse
        {
            get
            {
                return Utility.CountTrue(
                    _sourceOpenPositions,
                    _sourceWorkingOrders) > 1;
            }
        }

        public EpicStreamListItem(
            string epic,
            EpicStreamListItemSource epicStreamListItemSource
            )
        {
            Epic = epic;
            SetSource(epicStreamListItemSource);
        }

        /// <summary>
        /// Sets the source of the EpicStreamListItem.
        /// </summary>
        /// <param name="epicStreamListItemSource"></param>
        /// <param name="isInUse"></param>
        public void SetSource(EpicStreamListItemSource epicStreamListItemSource, bool isInUse = true)
        {
            switch (epicStreamListItemSource)
            {
                case EpicStreamListItemSource.SourceOpenPositions: _sourceOpenPositions = isInUse; break;
                case EpicStreamListItemSource.SourceWorkingOrders: _sourceWorkingOrders = isInUse; break;
            }
        }

        public bool IsSource(EpicStreamListItemSource epicStreamListItemSource)
        {
            return 
                epicStreamListItemSource == EpicStreamListItemSource.SourceOpenPositions && _sourceOpenPositions ||
                epicStreamListItemSource == EpicStreamListItemSource.SourceWorkingOrders && _sourceWorkingOrders;
        }
    }
}
