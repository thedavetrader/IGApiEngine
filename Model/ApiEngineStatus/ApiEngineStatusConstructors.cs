using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using dto.endpoint.positions.get.otc.v2;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    public partial class ApiEngineStatus
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        public ApiEngineStatus()
        {
            MapProperties();
        }
    }
}