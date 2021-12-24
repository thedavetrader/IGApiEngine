using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static ClientSentiment? SaveClientSentiment(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] dto.endpoint.clientsentiment.ClientSentiment clientSentiment
            )
        {
            _ = apiDbContext.ClientSentiments ?? throw new DBContextNullReferenceException(nameof(apiDbContext.ClientSentiments));

            var currentClientSentiment = Task.Run(async () => await apiDbContext.ClientSentiments.FindAsync(clientSentiment.marketId)).Result;

            if (currentClientSentiment is not null)
                currentClientSentiment.MapProperties(clientSentiment);
            else
                currentClientSentiment = apiDbContext.ClientSentiments.Add(new ClientSentiment(clientSentiment)).Entity;

            return currentClientSentiment;
        }
        
      

    }
}