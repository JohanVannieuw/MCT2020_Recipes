using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes_DB.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ExceptionMessageUser
    {
        public string Error { get; set; }
        public string Message { get; set; }
                public string ErrorRoute { get; set; } = null;
    }
}
