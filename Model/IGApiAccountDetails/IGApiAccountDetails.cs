using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dto.endpoint.accountbalance;

namespace IGApi.Model
{
    public class IGApiAccountDetails : dto.endpoint.accountbalance.AccountDetails
    {
        /// <summary>
        /// This constructor allows to iniitialize IGApiAccountDetails basesd on the session AccountDetails from IGWebApiClient.dto.
        /// </summary>
        /// <param name="accountBalance"></param>
        public IGApiAccountDetails(dto.endpoint.auth.session.AccountDetails accountDetails) : base()
        {
            accountId = accountDetails.accountId;
            accountName = accountDetails.accountName;
            preferred = accountDetails.preferred;
            accountType = accountDetails.accountType;
        }

        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        //[Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public IGApiAccountDetails() : base() { }
    }
}
