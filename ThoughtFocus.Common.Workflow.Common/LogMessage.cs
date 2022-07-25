using Newtonsoft.Json;

namespace OptimaJet.Common
{
    public class LogMessage
    {
        public object json { get; set; }
        public string message { get; set; }

        public override string ToString()
        {
            if (json != null)
            {
                return message + " " + JsonConvert.SerializeObject(json, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Serialize });
            }
            else
            {
                return message;
            }
        }
    }
}
