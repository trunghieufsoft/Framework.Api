using Newtonsoft.Json.Linq;
using System.Linq;

namespace Common.Core.Extensions
{
    public static class JvalueExtensition
    {
        public static int[] GetPositionValue(this JToken jvalue, string keyGetValue, string keySplit)
        {
            return jvalue.SelectToken(keyGetValue).ToString().Split(keySplit).Select(int.Parse).ToArray();
        }
    }
}
