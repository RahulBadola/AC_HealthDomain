using MongoDB.Bson.Serialization.Attributes;

namespace type_lookup_service.Model
{
    [BsonIgnoreExtraElements]
    public class Authorization
    {
        public int AuthorizationId { get; set; }

        public int? AuthorizationType { get; set; }

        public int ItemId { get; set; }

        [BsonElement("object")]
        public string Object { get; set; }

        [BsonElement("objectSid")]
        public string ObjectSid { get; set; }

        [BsonElement("operation")]
        public string Operation { get; set; }
    }

}
