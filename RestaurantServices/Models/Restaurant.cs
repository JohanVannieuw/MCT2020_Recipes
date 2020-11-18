using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantServices.Models
{
    public class Restaurant
    {
        public Restaurant() { }  //ctor

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.String)] //controleer type in Mongo
        [BsonElement("RestaurantId")]
        public Guid RestaurantId { get; set; } = Guid.NewGuid();

        [BsonElement("RestaurantName")]
        public string Name { get; set; }

        public string Street { get; set; }

        //zelfde naam als BSON -> geen dataannotatie
        public string HouseNumber { get; set; }
        public string PostalCode { get; set; }

        [BsonElement("City")]
        [Column("City")]
        public string Main_city_name { get; set; }

        [BsonIgnoreIfNull()]
        [BsonElement("Latitude")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Lat { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("Longitude")]
        [BsonRepresentation(BsonType.Decimal128)]
        [Column("Longitude")]
        public decimal Long { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("Phone")]
        [Column("Phone")]
        public string Phone1 { get; set; }

        [BsonIgnoreIfNull]
        public string Description { get; set; }

        //navigatie props – Onbestaand in NoSQL! maar toch bruikbaar in C#-------
        [BsonIgnoreIfNull]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Review> Reviews { get; set; }
    }

}
