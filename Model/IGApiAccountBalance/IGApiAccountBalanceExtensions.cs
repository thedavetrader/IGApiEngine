using System.Diagnostics.CodeAnalysis;
using IGApiEngine.Common;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static async Task<int> InsertOrUpdateAsync([NotNullAttribute] this IGApiAccountBalance accountBalance)
        {
            try
            {
                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.AccountBalances ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.AccountBalances));

                iGApiDbContext.Entry(accountBalance).State =
                        iGApiDbContext.AccountBalances.Any(pk =>
                            pk.AccountId == accountBalance.AccountId
                        ) ? EntityState.Modified : EntityState.Added;
                return await iGApiDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                throw;
            }
        }

        public static async Task<int> InsertOrUpdateAsync([NotNullAttribute] this List<IGApiAccountBalance> accountBalances)
        {
            try
            {
                if (accountBalances.Any())
                {
                    using IGApiDbContext iGApiDbContext = new();

                    _ = iGApiDbContext.AccountBalances ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.AccountBalances));

                    foreach (var accountBalance in accountBalances)
                    {
                        iGApiDbContext.Entry(accountBalance).State = iGApiDbContext.AccountBalances.Any(pk =>
                            pk.AccountId == accountBalance.AccountId
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