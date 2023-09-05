using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    using static Log;
    public partial class ApiRequestQueueItem
    {
        public ApiRequestQueueItem? SaveApiRequestQueueItem(
            [NotNullAttribute] string connectionString
            )
        {
            Task.Run(() => PushApiRequestQueueItem(connectionString, this));

            return this; //Just a formality, returned object is not used.

            static async Task PushApiRequestQueueItem(string connectionString, ApiRequestQueueItem apiRequestQueueItem)
            {
                try
                {
                    using SqlConnection connection = new(connectionString);
                    connection.Open();

                    SqlCommand command = new("dbo.push_api_request_queue_item", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramGuid = command.Parameters.AddWithValue("@guid", apiRequestQueueItem.Guid); paramGuid.SqlDbType = SqlDbType.UniqueIdentifier;
                    SqlParameter paramParentGuid = command.Parameters.AddWithValue("@parent_guid", apiRequestQueueItem.ParentGuid); paramParentGuid.SqlDbType = SqlDbType.UniqueIdentifier;
                    SqlParameter paramTimestamp = command.Parameters.AddWithValue("@timestamp", apiRequestQueueItem.Timestamp); paramTimestamp.SqlDbType = SqlDbType.DateTime2;
                    SqlParameter paramRequest = command.Parameters.AddWithValue("@request", apiRequestQueueItem.Request); paramRequest.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter paramParameter = command.Parameters.AddWithValue("@parameter", apiRequestQueueItem.Parameters); paramParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter paramExecuteAsap = command.Parameters.AddWithValue("@execute_asap", apiRequestQueueItem.ExecuteAsap); paramExecuteAsap.SqlDbType = SqlDbType.Bit;
                    SqlParameter paramIsRecurrent = command.Parameters.AddWithValue("@is_recurrent", apiRequestQueueItem.IsRecurrent); paramIsRecurrent.SqlDbType = SqlDbType.Bit;

                    await command.ExecuteNonQueryAsync();

                    await connection.CloseAsync();
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                    throw;
                }
            }
        }
    }
}