using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    public partial class ApiEventHandler
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public ApiEventHandler()
        { 
            Sender = string.Format(Constants.InvalidEntry, nameof(ApiEventHandler)); 
            Delegate = string.Format(Constants.InvalidEntry, nameof(ApiEventHandler)); 
        }

        /// <summary>
        /// For creating new ApiEventHandlers using dto.endpoint.auth.session.
        /// </summary>
        /// <param name="ApiEventHandlerDetails"></param>
        /// <param name="ApiEventHandlerBalance"></param>
        public ApiEventHandler(
            [NotNullAttribute] string sender,
            [NotNullAttribute] string @delegate,
            [NotNullAttribute] int @priority
            )
        {
            MapProperties(sender, @delegate, @priority);
            _ = Sender ?? throw new PrimaryKeyNullReferenceException(nameof(Sender));
            _ = Delegate ?? throw new PrimaryKeyNullReferenceException(nameof(Delegate));
        }
    }
}

