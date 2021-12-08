namespace IGApi.Common
{
    public class ObservableList<T> : List<T>
    {

        public event EventHandler? ListChanged;

        public void NotifyChange()
        {
            ListChanged?.Invoke(this, new EventArgs());
        }

        //TODO:     ZEROPRIO Remove. Just reference to method hiding example.
        //public new void Add(T item) // "new" to avoid compiler-warnings, because we're hiding a method from base-class
        //{
        //    if (null != ListChanged)
        //    {
        //        ListChanged(this, null);
        //    }
        //    base.Add(item);
        //}
    }

}
