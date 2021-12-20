using System.Globalization;
using IGApi.Common;
using IGApi.Common.Extensions;
using IGApi.IGApi.StreamingApi.StreamingTickData.EpicStreamListItem;
using IGApi.Model;

namespace IGApi
{
    using static Log;
    public sealed partial class ApiEngine
    {
        public ObservableList<EpicStreamListItem> EpicStreamList = new();

        /// <summary>
        /// Register new epics to EpicStreamList and removes closed ones. Only after both actions are done, notify changes to the list.
        /// </summary>
        /// <param name="syncEpicStreamListItems"></param>
        public void SyncEpicStreamListItems(
            List<EpicStreamListItem> syncEpicStreamListItems,
            EpicStreamListItem.EpicStreamListItemSource epicStreamListItemSource
            )
        {
            lock (EpicStreamList)
            {
                bool isChanged = false;

                if (syncEpicStreamListItems.Any())
                {
                    //  Add new open epics to list.
                    List<EpicStreamListItem> newEpicStreamListItems =
                        syncEpicStreamListItems
                            .Where(epic => !EpicStreamList.Where(item => item.Epic == epic.Epic).Any()).Distinct().ToList();

                    if (newEpicStreamListItems.Any())
                    {
                        newEpicStreamListItems.ForEach(epic =>
                        {
                            if (EpicStreamPriceAvailableCheck(epic.Epic))
                            {
                                EpicStreamList.Add(epic);
                                isChanged = true;
                            }
                            else
                                isChanged = syncEpicStreamListItems.Remove(epic);
                        });
                    }
                }

                EpicStreamList
                    .Where(esl =>
                        !syncEpicStreamListItems.Where(sesli => sesli.Epic == esl.Epic).Any() &&
                        esl.IsSource(epicStreamListItemSource)).ToList()
                    .ForEach(epicStreamListItem =>
                    {
                        if (epicStreamListItem.multiUse)
                        {
                            epicStreamListItem.SetSource(epicStreamListItemSource, false);
                        }
                        else // Implicitly assume that if it is not multiUse, it must be used by the source caller of SyncEpicStreamListItems
                            isChanged = EpicStreamList.Remove(epicStreamListItem);
                    });

                //  Notify change once list is properly updated.
                if (isChanged)
                    EpicStreamList.NotifyChange();
            }
        }

        public void AddEpicStreamListItem(EpicStreamListItem epicStreamListItem)
        {
            lock (EpicStreamList)
            {
                if (EpicStreamPriceAvailableCheck(epicStreamListItem.Epic))
                    EpicStreamList.Add(epicStreamListItem);

                EpicStreamList.NotifyChange();
            }
        }

        public void RemoveEpicStreamListItem(EpicStreamListItem epicStreamListItem)
        {
            lock (EpicStreamList)
            {
                if (EpicStreamList.Remove(epicStreamListItem))
                    EpicStreamList.NotifyChange();
            }
        }

        public void RemoveEpicStreamListItems(IEnumerable<EpicStreamListItem> epicStreamListItems)
        {
            if (epicStreamListItems.Any())
            {
                lock (EpicStreamList)
                {
                    if (EpicStreamList.RemoveAll(r => epicStreamListItems.Any(a => a.Epic == r.Epic)) > 0)
                        EpicStreamList.NotifyChange();
                }
            }
        }

        public static bool EpicStreamPriceAvailableCheck(string epic)
        {
            bool StreamingPricesAvailable;

            IGApiDbContext iGApiDbContext = new();
            _ = iGApiDbContext.EpicDetails ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetails));
            var epicDetail = iGApiDbContext.EpicDetails.Find(epic);

            if (epicDetail is not null)
            {
                StreamingPricesAvailable = epicDetail.StreamingPricesAvailable;

                if (!StreamingPricesAvailable)
                    LogInvalidEpic(epic);
            }
            else StreamingPricesAvailable = true;   // assume instrument can stream. Otherwise a new openposition could not be written to db when EpicDetail is not available yet.

            return StreamingPricesAvailable;
        }

        private static void LogInvalidEpic(string epic)
        {
            WriteLog();
            WriteLog(Messages("Streaming prices unavailable for:"));
            WriteLog(Messages(""));
            WriteLog(Messages("Epic"));
            WriteLog(Messages(new string('_', 40)));
            WriteLog(Messages(epic.ToString()));
            WriteLog(Messages(""));
        }
    }
}
