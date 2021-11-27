using IGWebApiClient.Common;

namespace IGApi.Common
{
    public class SmartDispatcher : PropertyEventDispatcher
    {
        private static PropertyEventDispatcher instance = new SmartDispatcher();

        public static PropertyEventDispatcher getInstance()
        {
            return instance;
        }

        public void addEventMessage(string message)
        {
            //UpdateDebugMessage(message);
        }

        public void BeginInvoke(Action a)
        {
            a();
        }
    }
}