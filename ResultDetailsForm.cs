using System.Drawing;
using System.Windows.Forms;
using QuizApp.Simple.Models;

namespace QuizApp.Simple
{
    // Toont de antwoorden van één quizpoging: vraag, gegeven antwoord, juist antwoord, goed/fout.
    public class ResultDetailsForm : Form
    {
        public ResultDetailsForm(QuizResult result)
        {
            this.Text = "Resultaat van " + result.StudentName;
            this.Width = 700;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterParent;

            Label scoreLabel = new Label();
            scoreLabel.Text = result.StudentName + " scoorde " + result.Score + " / " + result.TotalQuestions;
            scoreLabel.Dock = DockStyle.Top;
            scoreLabel.Height = 40;
            scoreLabel.Font = new Font(SystemFonts.DefaultFont.FontFamily, 12, FontStyle.Bold);

            DataGridView grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.Columns.Add("Question", "Vraag");
            grid.Columns.Add("Given", "Antwoord student");
            grid.Columns.Add("Correct", "Juiste antwoord");
            grid.Columns.Add("Ok", "Resultaat");

            foreach (QuizAnswer answer in result.Answers)
            {
                string outcome = "Fout";
                Color rowColor = Color.LightSalmon;

                if (answer.IsCorrect)
                {
                    outcome = "Goed";
                    rowColor = Color.LightGreen;
                }

                int rowIndex = grid.Rows.Add(answer.QuestionText, answer.GivenAnswer, answer.QuestionCorrectAnswer, outcome);
                grid.Rows[rowIndex].DefaultCellStyle.BackColor = rowColor;
            }

            this.Controls.Add(grid);
            this.Controls.Add(scoreLabel);
        }
    }
}
