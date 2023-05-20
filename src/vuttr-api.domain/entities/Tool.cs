using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace vuttr_api.domain.entities;

public class Tool
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Link { get; set; }
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}