using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantServices.Models
{
    public class Review
    {
        public Review() { } //ctor- serialisering

        [BsonId]
        public ObjectId Id { get; set; } = new ObjectId();

        [BsonElement]
        public string Subject { get; set; }

        [BsonElement]
        public string Comment { get; set; }

        [BsonElement("Quotation")]
        [BsonRepresentation(BsonType.Decimal128)]
        [Range(typeof(decimal),"0","10")]
        public decimal Quotation { get; set; }

        [BsonElement("DateOfCreation")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DateOfCreation { get; set; } = DateTime.Now;

        //navigatie property
        [BsonRepresentation(BsonType.String)]
        [BsonElement("RestaurantId")]
        public Guid RestaurantID {get; set;}

        [BsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Restaurant Restaurant { get; set; }

    }
}
