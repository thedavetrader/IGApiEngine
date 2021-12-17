namespace IGApi.Common.Extensions
{
    public class ObservableList<T> : List<T>
    {
        public event EventHandler? ListChanged;

        public void NotifyChange()
        {
            ListChanged?.Invoke(this, new EventArgs());
        }
    }
}