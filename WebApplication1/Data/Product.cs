using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace WebApplication1.Data
{
    public class Product
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("price")]
        public double Price { get; set; }
    }
}
