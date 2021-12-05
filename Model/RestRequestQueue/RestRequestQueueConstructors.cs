using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    public partial class RestRequestQueue
    {
        public RestRequestQueue(
            [NotNullAttribute] string restRequest,
            string? parameters,
            [NotNullAttribute] bool executeAsap,
            [NotNullAttribute] bool isRecurrent
        )
        {
            RestRequest = restRequest;
            Parameters = parameters;
            ExecuteAsap = executeAsap;
            IsRecurrent = isRecurrent;
        }
    }
}