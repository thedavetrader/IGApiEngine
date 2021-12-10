using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGWebApiClient;
using dto.endpoint.positions.get.otc.v2;

namespace IGApi.Model
{
    public partial class OpenPosition
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public OpenPosition()
        {
            AccountId = string.Format(Constants.InvalidEntry, nameof(OpenPosition));
            DealId = string.Format(Constants.InvalidEntry, nameof(OpenPosition));
        }

        /// <summary>
        /// For creating new accounts using OpenPositionData
        /// </summary>
        /// <param name="openPositionData"></param>
        /// <param name="accountId"></param>
        /// <param name="epic"></param>
        /// <exception cref="PrimaryKeyNullReferenceException"></exception>
        /// <exception cref="EssentialPropertyNullReferenceException"></exception>
        public OpenPosition(
            [NotNullAttribute] OpenPositionData openPositionData,
            [NotNullAttribute] string accountId,
            [NotNullAttribute] string epic
            )
        {
            MapProperties(openPositionData, accountId, epic);

            _ = AccountId ?? throw new PrimaryKeyNullReferenceException(nameof(AccountId));
            _ = Epic ?? throw new EssentialPropertyNullReferenceException(nameof(Epic));
            _ = DealId ?? throw new EssentialPropertyNullReferenceException(nameof(DealId));
            _ = Direction ?? throw new EssentialPropertyNullReferenceException(nameof(Direction));
            _ = Currency ?? throw new EssentialPropertyNullReferenceException(nameof(Currency));
        }

        public OpenPosition(
            [NotNullAttribute] LsTradeSubscriptionData lsTradeSubscriptionData,
            [NotNullAttribute] string accountId
            )
        {
            MapProperties(lsTradeSubscriptionData, accountId);

            _ = AccountId ?? throw new PrimaryKeyNullReferenceException(nameof(AccountId));
            _ = Epic ?? throw new EssentialPropertyNullReferenceException(nameof(Epic));
            _ = DealId ?? throw new EssentialPropertyNullReferenceException(nameof(DealId));
            _ = Direction ?? throw new EssentialPropertyNullReferenceException(nameof(Direction));
            _ = Currency ?? throw new EssentialPropertyNullReferenceException(nameof(Currency));
        }

    }
}
