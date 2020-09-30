using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes_DB.Helpers
{
    public class MyMemoryCache
    {
        public MemoryCache Cache { get; set; }
        public MemoryCacheOptions Options { get; set; }
        public MyMemoryCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 100, //geen unit of measure - enkel een getal
            });
        }
    }

}
