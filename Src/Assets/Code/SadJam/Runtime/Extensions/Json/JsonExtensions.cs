using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SadJam
{
    public static class JsonExtensions
    {
        public static IEnumerable<JProperty> GetAllFields(this JToken jToken)
        {
            List<JProperty> fields = new List<JProperty>();

            GetAllFields(jToken, fields);

            return fields;
        }

        private static void GetAllFields(JToken jToken, List<JProperty> list, JProperty prop = null)
        {
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    foreach (var child in jToken.Children<JProperty>())
                        GetAllFields(child, list);
                    break;
                case JTokenType.Array:
                    foreach (var child in jToken.Children())
                        GetAllFields(child, list);
                    break;
                case JTokenType.Property:
                    JProperty property = (JProperty)jToken;
                    GetAllFields(property.Value, list, property);
                    break;
                default:
                    list.Add(prop);
                    break;
            }
        }

        public static IEnumerable<JProperty> GetFields(this JToken jToken, string name)
        {
            List<JProperty> fields = new List<JProperty>();

            GetFields(jToken, name, fields);

            return fields;
        }

        private static void GetFields(JToken jToken, string name, List<JProperty> list, JProperty prop = null)
        {
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    foreach (var child in jToken.Children<JProperty>())
                        GetFields(child, name, list);
                    break;
                case JTokenType.Array:
                    foreach (var child in jToken.Children())
                        GetFields(child, name, list);
                    break;
                case JTokenType.Property:
                    JProperty property = (JProperty)jToken;
                    GetFields(property.Value, name, list, property);
                    break;
                default:
                    if (prop.Name != name) return;
                    list.Add(prop);
                    break;
            }
        }
    }
}
