using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes_DB.Models
{
    public class ExceptionMessageUser
    {


        public string Error { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
        public string ErrorRoute { get; set; }
    }

}
