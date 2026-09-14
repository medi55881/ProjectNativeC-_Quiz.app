using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuizApp.Simple.Data;
using QuizApp.Simple.Models;

namespace QuizApp.Simple
{
    // Startscherm met drie knoppen naar de rest van de app.
    public class MainForm : Form
    {
        private Button btnQuiz;
        private Button btnQuestions;
        private Button btnResults;

        public MainForm()
        {
            this.Text = "Quiz-applicatie";
            this.Width = 420;
            this.Height = 320;
            this.StartPosition = FormStartPosition.CenterScreen;

            Label title = new Label();
            title.Text = "Quiz-applicatie";
            title.Font = new Font(this.Font.FontFamily, 16, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(30, 20);

            btnQuiz = new Button();
            btnQuiz.Text = "Quiz starten (student)";
            btnQuiz.Location = new Point(30, 80);
            btnQuiz.Width = 320;
            btnQuiz.Height = 40;
            btnQuiz.Click += btnQuiz_Click;

            btnQuestions = new Button();
            btnQuestions.Text = "Vragen beheren (docent)";
            btnQuestions.Location = new Point(30, 130);
            btnQuestions.Width = 320;
            btnQuestions.Height = 40;
            btnQuestions.Click += btnQuestions_Click;

            btnResults = new Button();
            btnResults.Text = "Resultaten bekijken (docent)";
            btnResults.Location = new Point(30, 180);
            btnResults.Width = 320;
            btnResults.Height = 40;
            btnResults.Click += btnResults_Click;

            this.Controls.Add(title);
            this.Controls.Add(btnQuiz);
            this.Controls.Add(btnQuestions);
            this.Controls.Add(btnResults);
        }

        private void btnQuiz_Click(object sender, EventArgs e)
        {
            QuizForm quizForm = new QuizForm();
            quizForm.Show();
        }

        private void btnQuestions_Click(object sender, EventArgs e)
        {
            QuestionsForm questionsForm = new QuestionsForm();
            questionsForm.Show();
        }

        private void btnResults_Click(object sender, EventArgs e)
        {
            ResultsForm resultsForm = new ResultsForm();
            resultsForm.Show();
        }
    }
}
