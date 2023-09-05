using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGApi.RequestQueue;
using Newtonsoft.Json;

namespace IGApi.Model
{
    using static Log;
    public partial class EpicTick
    {
        //TODO: Natively Compiled Stored Proc DeleteEpicTick
        public EpicTick? SaveEpicTick(
            [NotNullAttribute] string connectionString
            )
        {
            Task.Run(() => UpsertEpicTick(connectionString, this));

            return this; //Just a formality, returned object is not used.

            async Task UpsertEpicTick(string connectionString, EpicTick EpicTick)
            {
                try
                {
                    using SqlConnection connection = new(connectionString);
                    connection.Open();

                    SqlCommand command = new("dbo.upsert_epic_tick", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramEpic = command.Parameters.AddWithValue("@epic", EpicTick.Epic); paramEpic.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter paramMidOpen = command.Parameters.AddWithValue("@mid_open", EpicTick.MidOpen); paramMidOpen.SqlDbType = SqlDbType.Decimal;
                    SqlParameter paramHigh = command.Parameters.AddWithValue("@high", EpicTick.High); paramHigh.SqlDbType = SqlDbType.Decimal;
                    SqlParameter paramLow = command.Parameters.AddWithValue("@low", EpicTick.Low); paramLow.SqlDbType = SqlDbType.Decimal;
                    SqlParameter paramChange = command.Parameters.AddWithValue("@change", EpicTick.Change); paramChange.SqlDbType = SqlDbType.Decimal;
                    SqlParameter paramChangePct = command.Parameters.AddWithValue("@change_percentage", EpicTick.ChangePct); paramChangePct.SqlDbType = SqlDbType.Decimal;
                    SqlParameter paramUpdateTime = command.Parameters.AddWithValue("@update_time", EpicTick.UpdateTime); paramUpdateTime.SqlDbType = SqlDbType.DateTime2;
                    SqlParameter paramMarketDelay = command.Parameters.AddWithValue("@market_delay", EpicTick.MarketDelay); paramMarketDelay.SqlDbType = SqlDbType.Int;
                    SqlParameter paramMarketState = command.Parameters.AddWithValue("@market_state", EpicTick.MarketState); paramMarketState.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter paramBid = command.Parameters.AddWithValue("@bid", EpicTick.Bid); paramBid.SqlDbType = SqlDbType.Decimal;
                    SqlParameter paramOffer = command.Parameters.AddWithValue("@offer", EpicTick.Offer); paramOffer.SqlDbType = SqlDbType.Decimal;

                    SqlParameter paramResult = command.Parameters.Add("@result", SqlDbType.NVarChar, 128); paramResult.Direction = ParameterDirection.Output;

                    await command.ExecuteNonQueryAsync();

                    var state = Convert.ToString(paramResult.Value);

                    await connection.CloseAsync();

                    if (state == "Added" || state == "Modified")
                        InvokeEpicTickEvent(state);
                    else
                        throw new Exception("[ERROR] \"dbo.upsert_epic_tick\" returned no result state \"Added\" or \"Modified\". Check the procedure for errors.");
                }
                catch (SqlException ex)
                {
                    foreach (var error in ex.Errors)
                    {
                        WriteError(Columns(error.ToString() ?? "ERROR NOT FOUND"));
                    }
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                    throw;
                }

                void InvokeEpicTickEvent(string state)
                {
                    using ApiDbContext apiDbContext = new();

                    var connectionString = apiDbContext.ConnectionString;
                    var apiEventHandler = apiDbContext.ApiEventHandlers.SingleOrDefault(s => s.Sender.ToLower() == nameof(EpicTick).ToLower());

                    if (apiEventHandler is not null)
                    {
                        var @delegate = apiEventHandler.Delegate;
                        var eventArgs = JsonConvert.SerializeObject(this, Formatting.None);

                        new ApiRequestQueueItem(
                            restRequest: nameof(RequestQueueEngineItem.ExecuteSqlScript),
                            parameters: $"exec {@delegate} @state='{state}', @event_arguments='{eventArgs}'",
                            executeAsap: true,
                            isRecurrent: false,
                            guid: Guid.NewGuid(),
                            parentGuid: null)
                            .SaveApiRequestQueueItem(connectionString);
                    }
                }
            }
        }

    }
}