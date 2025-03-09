using System.Collections.Generic;

namespace SadJam
{
    public class StringConvertor : StaticBehaviour
    {
        public string GetContent(IEnumerable<object> inputs)
        {
            return string.Join(' ', inputs);
        }
    }
}
