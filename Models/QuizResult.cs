using System;
using System.Collections.Generic;

namespace QuizApp.Simple.Models
{
    // Eén afgeronde poging van een student.
    public class QuizResult
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = "";
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime DateTaken { get; set; } = DateTime.Now;

        public List<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
    }
}
