using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static ClientSentiment? SaveClientSentiment(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.clientsentiment.ClientSentiment clientSentiment
            )
        {
            _ = iGApiDbContext.ClientSentiments ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ClientSentiments));

            var currentClientSentiment = Task.Run(async () => await iGApiDbContext.ClientSentiments.FindAsync(clientSentiment.marketId)).Result;

            if (currentClientSentiment is not null)
                currentClientSentiment.MapProperties(clientSentiment);
            else
                currentClientSentiment = iGApiDbContext.ClientSentiments.Add(new ClientSentiment(clientSentiment)).Entity;

            return currentClientSentiment;
        }
        
      

    }
}