using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.IGApi.StreamingApi.StreamingTickData.EpicStreamListItem
{
    /// <summary>
    /// This object holds Epic data, that is formed such that it can be serialized and deserialized to and from JSON RestQueueParameter.
    /// </summary>
    public class EpicStreamListItem : IEqualityComparer<EpicStreamListItem>
    {
        public readonly string Epic;

        private bool _sourceOpenPositions;

        private bool _sourceWorkingOrders;

        private bool _sourceCustomTracked;

        public enum EpicStreamListItemSource
        {
            OpenPositions,
            WorkingOrders,
            CustomTracked,
            None
        }

        public bool multiUse
        {
            get
            {
                return Utility.CountTrue(
                    _sourceOpenPositions,
                    _sourceWorkingOrders,
                    _sourceCustomTracked) > 1;
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
                case EpicStreamListItemSource.OpenPositions: _sourceOpenPositions = isInUse; break;
                case EpicStreamListItemSource.WorkingOrders: _sourceWorkingOrders = isInUse; break;
                case EpicStreamListItemSource.CustomTracked: _sourceCustomTracked = isInUse; break;
            }
        }

        public bool IsSource(EpicStreamListItemSource epicStreamListItemSource)
        {
            return
                epicStreamListItemSource == EpicStreamListItemSource.OpenPositions && _sourceOpenPositions ||
                epicStreamListItemSource == EpicStreamListItemSource.WorkingOrders && _sourceWorkingOrders ||
                epicStreamListItemSource == EpicStreamListItemSource.CustomTracked && _sourceCustomTracked;
        }

        public bool Equals(EpicStreamListItem? x, EpicStreamListItem? y)
        {
            if (x is null && y is null)
                return true;
            else if (x is null || y is null)
                return false;
            else
                return x.Epic == y.Epic;
        }

        public int GetHashCode([DisallowNull] EpicStreamListItem obj)
        {
            return obj.Epic.GetHashCode();
        }
    }
}
