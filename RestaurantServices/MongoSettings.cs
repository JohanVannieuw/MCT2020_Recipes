using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantServices
{
    public interface IMongoSettings
    {
        string ConnectionString { get; set; }
        string ConnectionStringHost { get; set; }
        string DatabaseName { get; set; }
    }

    public class MongoSettings : IMongoSettings
    {
        public string ConnectionString { get; set; }
        public string ConnectionStringHost { get; set; }

        public string DatabaseName { get; set; }

    }

}
