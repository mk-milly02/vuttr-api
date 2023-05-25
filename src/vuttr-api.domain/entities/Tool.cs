using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace vuttr_api.domain.entities;

public class Tool
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Link { get; set; }
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}