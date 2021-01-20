using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Courses.Common
{
    public class ServiceMessage
    {
        public string ReplyRoutingKey { get; set; }
        
        public string Body { get; set; }
        
        public Dictionary<string, string> Parameters;

        public ServiceMessage()
        {
            Parameters = new Dictionary<string, string>();
        }
    }
}   