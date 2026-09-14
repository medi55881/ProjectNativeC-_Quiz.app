namespace QuizApp.Simple.Models
{
    // Eén quizvraag. Kan een meerkeuzevraag zijn (met OptionA/B/C) of een open vraag.
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public QuestionType Type { get; set; } = QuestionType.MultipleChoice;

        public string OptionA { get; set; } = "";
        public string OptionB { get; set; } = "";
        public string OptionC { get; set; } = "";

        // Bij meerkeuze: "A", "B" of "C". Bij een open vraag: het exacte verwachte antwoord.
        public string CorrectAnswer { get; set; } = "";
    }
}
