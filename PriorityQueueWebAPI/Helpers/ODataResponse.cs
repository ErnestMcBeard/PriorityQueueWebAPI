using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELPriorityQueue.Models
{
    public class ODataResponse<T>
    {
        [JsonProperty("odata.metadata")]
        public string Metadata { get; set; }
        public List<T> Value { get; set; }
    }
}
