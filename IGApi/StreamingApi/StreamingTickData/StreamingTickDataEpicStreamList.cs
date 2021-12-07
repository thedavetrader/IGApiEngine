using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using Lightstreamer.DotNet.Client;
using Newtonsoft.Json;

namespace IGApi
{
    public sealed partial class ApiEngine
    {
        public ListExtension<RestRequestParameterEpic> EpicStreamList = new();

        /// <summary>
        /// Register new epics to EpicStreamList and removes closed ones. Only after both actions are done, notify changes to the list.
        /// </summary>
        /// <param name="epics"></param>
        public void SyncEpicStreamListItems(List<RestRequestParameterEpic> epics)
        {
            if (epics.Any())
            {
                lock (EpicStreamList)
                {
                    bool isChanged = false;

                    //  Add new open epics to list.
                    List<RestRequestParameterEpic> newEpics = epics.Where(validItem => !EpicStreamList.Where(item => item.Epic == validItem.Epic).Any()).Distinct().ToList();

                    if (newEpics.Any())
                    {
                        newEpics.ForEach(epic =>
                        {
                            if (EpicStreamPriceAvailableCheck(epic.Epic))
                            {
                                EpicStreamList.Add(epic);
                                isChanged = true;
                            }
                            else
                                epics.Remove(epic);
                        });
                    }

                    //  Remove closed epics fom list.
                    //  TODO: Only remove closed epics from list if soucres is openposition update (use reflectoin nameof)
                    EpicStreamList
                        .Where(item => !epics.Where(w => w.Epic == item.Epic).Any()).ToList()
                        .ForEach(epicStreamListItem =>
                        {
                            EpicStreamList.Remove(epicStreamListItem);
                            isChanged = true;
                        });

                    if (EpicStreamList.RemoveAll(item => !epics.Where(w => w.Epic == item.Epic).Any()) > 0)
                        isChanged = true;

                    //  Notify change once list is properly updated.
                    if (isChanged)
                        EpicStreamList.NotifyChange();
                }
            }
        }

        public void AddEpicStreamListItems(RestRequestParameterEpic epicStreamListItem)
        {
            lock (EpicStreamList)
            {
                if (EpicStreamPriceAvailableCheck(epicStreamListItem.Epic))
                    EpicStreamList.Add(epicStreamListItem);

                // TODO: Notify change is situational.
                EpicStreamList.NotifyChange();
            }
        }

        public void RemoveEpicStreamListItems(RestRequestParameterEpic epicStreamListItem)
        {
            lock (EpicStreamList)
            {
                EpicStreamList.Remove(epicStreamListItem);

                EpicStreamList.NotifyChange();
            }
        }

        public static bool EpicStreamPriceAvailableCheck(string epic)
        {
            bool StreamingPricesAvailable = true;

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
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[GetOpenPositionDetails]", "", "", ""));
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatTwoColumns, "[GetOpenPositionDetails]", "Streaming prices unavailable for:"));
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[GetOpenPositionDetails]", "", "", ""));
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[GetOpenPositionDetails]", "", "", "Epic"));
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[GetOpenPositionDetails]", "", "", new string('_', 40)));
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[GetOpenPositionDetails]", "", "", epic.ToString()));
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[GetOpenPositionDetails]", "", "", ""));
        }
    }
}
