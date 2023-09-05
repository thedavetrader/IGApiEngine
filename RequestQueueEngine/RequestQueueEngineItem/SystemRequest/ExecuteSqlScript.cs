using System.Data;
using IGApi.Common;
using IGApi.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? ExecuteSqlScriptCompleted;

        [RequestType(isRestRequest: false, isTradingRequest: false)]
        public void ExecuteSqlScript()
        {
            string? request = "[NO_SCRIPT]";
            try
            {
                request = ApiRequestQueueItem.Parameters;

                using ApiDbContext apiDbContext = new();

                using SqlConnection connection = new(apiDbContext.Database.GetDbConnection().ConnectionString);

                connection.Open();

                using var command = connection.CreateCommand();

                connection.InfoMessage += (object sender, SqlInfoMessageEventArgs e) => { WriteInformational(Columns(e.Message)); };

                command.CommandText = request;
                command.CommandType = CommandType.Text;

                apiDbContext.Database.OpenConnection();

                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (SqlException ex)
            {
                WriteError(Columns("[SQL_SCRIPT_ERROR] " + ex.Message + Environment.NewLine + Environment.NewLine + "[SCRIPT]" + Environment.NewLine + request));
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(ExecuteSqlScriptCompleted);
            }
        }
    }
}