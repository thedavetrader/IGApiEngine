using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    public partial class ApiEventHandler
    {
        public void MapProperties(
            [NotNullAttribute] string sender,
            [NotNullAttribute] string @delegate,
            [NotNullAttribute] int @priority
            )
        {
            Sender = sender;
            Delegate = @delegate;
            Priority = priority;
        }
    }
}

