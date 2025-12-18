using MongoDB.Bson;

public class Answer {
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}
