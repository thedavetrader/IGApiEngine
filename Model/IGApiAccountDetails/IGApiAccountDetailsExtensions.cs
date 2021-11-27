using System.Diagnostics.CodeAnalysis;
using IGApiEngine.Common;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static async Task<int> InsertOrUpdateAsync([NotNullAttribute] this IGApiAccountDetails accountDetail)
        {
            try
            {
                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.IGApiAccountDetails ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.IGApiAccountDetails));

                iGApiDbContext.Entry(accountDetail).State =
                        iGApiDbContext.AccountBalances.Any(pk =>
                            pk.AccountId == accountDetail.accountId
                        ) ? EntityState.Modified : EntityState.Added;
                return await iGApiDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                throw;
            }
        }

        public static async Task<int> InsertOrUpdateAsync([NotNullAttribute] this List<IGApiAccountDetails> accountDetails)
        {
            try
            {
                if (accountDetails.Any())
                {
                    using IGApiDbContext iGApiDbContext = new();

                    _ = iGApiDbContext.IGApiAccountDetails ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.IGApiAccountDetails));

                    foreach (var accountDetail in accountDetails)
                    {
                        iGApiDbContext.Entry(accountDetail).State = iGApiDbContext.IGApiAccountDetails.Any(pk =>
                            pk.accountId == accountDetail.accountId
                        ) ? EntityState.Modified : EntityState.Added;
                    }

                    return await iGApiDbContext.SaveChangesAsync();
                }
                else return 0;
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}