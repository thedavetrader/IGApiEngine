using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGApi
{
    /// <summary>
    /// This object holds Epic data, that is formed such that it can be serialized and deserialized to and from JSON RestQueueParameter.
    /// </summary>
    public class RestRequestParameterEpic
    {
        public readonly string Epic;

        public RestRequestParameterEpic(string epic)
        {
            Epic = epic;
        }
    }
}
