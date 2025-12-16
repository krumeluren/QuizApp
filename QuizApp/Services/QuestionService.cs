using MongoDB.Bson;
using MongoDB.Driver;

namespace QuizApp.Services {
    public class QuestionService {
        private readonly IMongoCollection<Question> _questions;
        public QuestionService (IMongoDatabase database) {
            _questions = database.GetCollection<Question>("questions");
        }

        public async Task<List<Question>> GetAllAsync () {
            return await _questions.Find(_ => true).ToListAsync();
        }

        public async Task<List<Question>> GetRandomQuestionsByTagAsync (string tag, int count) {
            // get random samples
            var pipeline = new EmptyPipelineDefinition<Question>()
                .Match(q => q.Tags.Contains(tag))
                .Sample(count);

            return await _questions.Aggregate(pipeline).ToListAsync();
        }

        public async Task CreateAsync (Question question) {

            try {
                await _questions.InsertOneAsync(question);

            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public async Task<List<string>> GetTopTagsAsync () {
            var pipeline = new EmptyPipelineDefinition<Question>()
                .Unwind<Question, Question>(q => q.Tags)
                .Group(new BsonDocument {
            { "_id", "$Tags" },
            { "count", new BsonDocument("$sum", 1) }
                })
                .Sort(new BsonDocument("count", -1))
                .Project(new BsonDocument("_id", 1));

            var results = await _questions.Aggregate(pipeline).ToListAsync();

            //  strings from the BsonDocuments
            return results.Select(x => x["_id"].AsString).ToList();
        }

        public async Task<List<Question>> GetRandomQuestionsByTagsAsync (List<string> tags, int count) {
            if (tags == null || !tags.Any()) return new List<Question>();

            // tags array must contain ANY of the tags in the list 
            var filter = Builders<Question>.Filter.AnyIn(q => q.Tags, tags);

            var pipeline = new EmptyPipelineDefinition<Question>()
                .Match(filter)
                .Sample(count);

            return await _questions.Aggregate(pipeline).ToListAsync();
        }
    }
}
