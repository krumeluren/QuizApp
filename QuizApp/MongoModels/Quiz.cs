using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


[BsonIgnoreExtraElements]
public class Question {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string Text { get; set; }

    public List<string> Tags { get; set; } = new List<string>();

    public List<Answer> Answers { get; set; } = new List<Answer>();
}

public class Answer {
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}


[BsonIgnoreExtraElements]
public class Quiz {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string Title { get; set; }

    public List<string> Tags { get; set; } = new List<string>();

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> QuestionIds { get; set; } = new List<string>();
}