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

        public async Task CreateAsync (Question question) {
            await _questions.InsertOneAsync(question);
        }
    }
}
