using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dto.endpoint.accountbalance;
using IGApiEngine.Common;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static async Task<int> InsertOrUpdateAsync([NotNullAttribute] this IGApiStreamingAccountData streamingAccountData)
        {
            try
            {
                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.IGApiStreamingAccountData ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.IGApiStreamingAccountData));

                iGApiDbContext.Entry(streamingAccountData).State =
                    iGApiDbContext.IGApiStreamingAccountData.Any(pk =>
                        pk.AccountId == streamingAccountData.AccountId
                    ) ? EntityState.Modified : EntityState.Added;
                return await iGApiDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                throw;
            }
        }

        public static async Task<int> InsertOrUpdateAsync([NotNullAttribute] this List<IGApiStreamingAccountData> streamingAccountData)
        {
            try
            {
                if (streamingAccountData.Any())
                {
                    using IGApiDbContext iGApiDbContext = new();

                    _ = iGApiDbContext.IGApiStreamingAccountData ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.IGApiStreamingAccountData));

                    foreach (var accountDetail in streamingAccountData)
                    {
                        iGApiDbContext.Entry(accountDetail).State = iGApiDbContext.IGApiStreamingAccountData.Any(pk =>
                            pk.AccountId == accountDetail.AccountId
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