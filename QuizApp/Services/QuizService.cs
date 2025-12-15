using MongoDB.Driver;

namespace QuizApp.Services {
    public class QuizService {
        private readonly IMongoCollection<Quiz> _quizzes;
        private readonly IMongoCollection<Question> _questions;
        public QuizService (IMongoDatabase db) {
            _questions = db.GetCollection<Question>("questions");
            _quizzes = db.GetCollection<Quiz>("quizzes");
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
    }

    public class QuizWithQuestions {
        public Quiz Quiz { get; set; }
        public List<Question> Questions { get; set; }
    }
}
