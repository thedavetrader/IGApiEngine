using System.Diagnostics.CodeAnalysis;
using IGApiEngine.Common;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static async Task<int> InsertOrUpdateAsync([NotNullAttribute] this IGRestApiQueue iGRestApiQueue)
        {
            try
            {
                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.IGRestApiQueue ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.IGRestApiQueue));

                iGApiDbContext.Entry(iGRestApiQueue).State =
                        iGApiDbContext.IGRestApiQueue.Any(pk =>
                            pk.Id == iGRestApiQueue.Id
                        ) ? EntityState.Modified : EntityState.Added;
                return await iGApiDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                throw;
            }
        }

        public static async Task<int> InsertOrUpdateAsync([NotNullAttribute] this List<IGRestApiQueue> iGRestApiQueue)
        {
            try
            {
                if (iGRestApiQueue.Any())
                {
                    using IGApiDbContext iGApiDbContext = new();

                    _ = iGApiDbContext.IGRestApiQueue ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.IGRestApiQueue));

                    foreach (var IGRestApiQueueItem in iGRestApiQueue)
                    {
                        iGApiDbContext.Entry(IGRestApiQueueItem).State = iGApiDbContext.IGRestApiQueue.Any(pk =>
                            pk.Id == IGRestApiQueueItem.Id
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