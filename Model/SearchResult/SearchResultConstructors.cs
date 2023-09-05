using System.Diagnostics.CodeAnalysis;
using dto.endpoint.accountactivity.transaction;
using IGApi.Common;

namespace IGApi.Model
{
    public partial class SearchResult
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public
        SearchResult()
        {
            Epic = string.Format(Constants.InvalidEntry, nameof(SearchResult));
        }

        /// <summary>
        /// For creating new accounts using TransactionData
        /// </summary>
        /// <param name="SearchResult"></param>
        /// <param name="epic"></param>
        /// <exception cref="PrimaryKeyNullReferenceException"></exception>
        /// <exception cref="EssentialPropertyNullReferenceException"></exception>
        public
        SearchResult(
            [NotNullAttribute] string epic
            )
        {
            Epic = epic;
        }
    }
}
