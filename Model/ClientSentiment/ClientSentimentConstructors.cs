using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    public partial class ClientSentiment
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public ClientSentiment()
        { MarketId = string.Format(Constants.InvalidEntry, nameof(ClientSentiment)); }

        /// <summary>
        /// For creating new accounts using dto.endpoint.clientsentiment.ClientSentiment
        /// </summary>
        /// <param name="clientSentiment"></param>
        /// <exception cref="PrimaryKeyNullReferenceException"></exception>
        public ClientSentiment(
            [NotNullAttribute] dto.endpoint.clientsentiment.ClientSentiment clientSentiment
            )
        {
            MapProperties(clientSentiment);

            _ = MarketId ?? throw new PrimaryKeyNullReferenceException(nameof(MarketId));
        }
    }
}