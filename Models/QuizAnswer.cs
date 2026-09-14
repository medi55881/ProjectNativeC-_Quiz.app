namespace QuizApp.Simple.Models
{
    // Eén beantwoorde vraag binnen een QuizResult.
    // QuestionText en QuestionCorrectAnswer worden er apart bij opgehaald (met een JOIN in Database.cs),
    // zodat het detailscherm de vraag kan tonen zonder zelf opnieuw in de database te zoeken.
    public class QuizAnswer
    {
        public int Id { get; set; }
        public int QuizResultId { get; set; }
        public int QuestionId { get; set; }

        public string QuestionText { get; set; } = "";
        public string QuestionCorrectAnswer { get; set; } = "";

        public string GivenAnswer { get; set; } = "";
        public bool IsCorrect { get; set; }
    }
}
