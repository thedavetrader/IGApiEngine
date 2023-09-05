namespace IGApi.RequestQueue
{
    [AttributeUsage(AttributeTargets.All)]
    public class RequestTypeAttribute : Attribute
    {
        private readonly bool _isRestRequest;
        private readonly bool _isTradingRequest;

        public RequestTypeAttribute(bool isRestRequest, bool isTradingRequest)
        {
            this._isRestRequest = isRestRequest;
            this._isTradingRequest = isTradingRequest;
        }

        public virtual bool IsRestRequest
        {
            get { return _isRestRequest; }
        }

        public virtual bool IsTradingRequest
        {
            get { return _isTradingRequest; }
        }
    }
}