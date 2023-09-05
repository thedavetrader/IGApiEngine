using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    using static Log;
    internal static partial class DtoModelExtensions
    {
        public static ApiEventHandler? SaveApiEventHandler(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] string sender,
            [NotNullAttribute] string @delegate,
            [NotNullAttribute] int @priority
            )
        {
            var currentApiEventHandler = Task.Run(async () => await apiDbContext.ApiEventHandlers.FindAsync(sender, @delegate)).Result;

            if (currentApiEventHandler is not null)
                currentApiEventHandler.MapProperties(sender, @delegate, @priority);
            else
                currentApiEventHandler = apiDbContext.ApiEventHandlers.Add(new ApiEventHandler(sender, @delegate, @priority)).Entity;

            return currentApiEventHandler;
        }
    }
}