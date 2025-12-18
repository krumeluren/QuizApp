using MongoDB.Driver;

namespace QuizApp.Services {
    public class QuizService {
        private readonly IMongoCollection<Quiz> _quizzes;
        private readonly IMongoCollection<Question> _questions;
        public QuizService (IMongoDatabase db) {
            _questions = db.GetCollection<Question>("questions");
            _quizzes = db.GetCollection<Quiz>("quizzes");
        }

        public async Task<List<Quiz>> GetAllQuizzesAsync () {
            // sorted by newest first
            return await _quizzes.Find(_ => true)
                                 .SortByDescending(q => q.Id)
                                 .ToListAsync();
        }

        public async Task<QuizWithQuestions> GetQuizWithQuestionsAsync (string quizId) {
            var quiz = await _quizzes.Find(q => q.Id == quizId).FirstOrDefaultAsync();
            if (quiz == null) return null;

            var filter = Builders<Question>.Filter.In(q => q.Id, quiz.QuestionIds);
            var questions = await _questions.Find(filter).ToListAsync();

            return new QuizWithQuestions {
                Quiz = quiz,
                Questions = questions
            };
        }

        public async Task SaveQuizTransactionAsync (Quiz quiz, List<Question> questions) {
            foreach (var q in questions) {
                var filter = Builders<Question>.Filter.Eq(x => x.Id, q.Id);
                await _questions.ReplaceOneAsync(filter, q, new ReplaceOptions { IsUpsert = true });
            }

            quiz.QuestionIds = questions.Select(q => q.Id).ToList();

            var quizFilter = Builders<Quiz>.Filter.Eq(x => x.Id, quiz.Id);
            await _quizzes.ReplaceOneAsync(quizFilter, quiz, new ReplaceOptions { IsUpsert = true });
        }
    }

    public class QuizWithQuestions {
        public Quiz Quiz { get; set; }
        public List<Question> Questions { get; set; }
    }
}
