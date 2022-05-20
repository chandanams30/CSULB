using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.TemplateModels
{
    public class TemplateStore<T>
        where T:class
    {
        public T GetDeserializedTemplate(string jsonString)
        {
            T template = JsonConvert.DeserializeObject<T>(jsonString);
            return template;
        }

        public string SetSerializedJSONString(object model)
        {
            string jsonString = string.Empty;
            jsonString= JsonConvert.SerializeObject(model);
            return jsonString;
        }
    }
}
