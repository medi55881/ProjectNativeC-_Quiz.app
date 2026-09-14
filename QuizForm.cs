using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuizApp.Simple.Data;
using QuizApp.Simple.Models;

namespace QuizApp.Simple
{
    // Houdt bij welke invoer-controls (radiobuttons of een tekstvak) bij welke vraag horen,
    // zodat we bij het inleveren weten wat de student per vraag heeft ingevuld.
    public class AnswerControl
    {
        public Question Question;
        public List<RadioButton> Options = new List<RadioButton>();
        public TextBox OpenAnswerBox;
    }

    // Student: naam invullen -> vragen beantwoorden -> score bekijken.
    // De drie stappen zijn drie panelen die om beurten zichtbaar zijn.
    public class QuizForm : Form
    {
        private Panel panelName;
        private Panel panelQuiz;
        private Panel panelResult;

        private TextBox txtName;
        private FlowLayoutPanel questionsPanel;
        private List<AnswerControl> answerControls = new List<AnswerControl>();

        private DataGridView resultGrid;
        private Label scoreLabel;

        public QuizForm()
        {
            this.Text = "Quiz";
            this.Width = 700;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;

            BuildNamePanel();
            BuildQuizPanelContainer();
            BuildResultPanel();

            this.Controls.Add(panelQuiz);
            this.Controls.Add(panelResult);
            this.Controls.Add(panelName);

            ShowOnly(panelName);
        }

        private void ShowOnly(Panel panelToShow)
        {
            panelName.Visible = (panelToShow == panelName);
            panelQuiz.Visible = (panelToShow == panelQuiz);
            panelResult.Visible = (panelToShow == panelResult);
        }

        private void BuildNamePanel()
        {
            panelName = new Panel();
            panelName.Dock = DockStyle.Fill;

            Label label = new Label();
            label.Text = "Jouw naam:";
            label.Location = new Point(30, 30);
            label.AutoSize = true;

            txtName = new TextBox();
            txtName.Location = new Point(30, 55);
            txtName.Width = 250;

            Button btnStart = new Button();
            btnStart.Text = "Start";
            btnStart.Location = new Point(30, 90);
            btnStart.Width = 100;
            btnStart.Click += btnStart_Click;

            panelName.Controls.Add(label);
            panelName.Controls.Add(txtName);
            panelName.Controls.Add(btnStart);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartQuiz();
        }

        private void StartQuiz()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vul eerst je naam in.");
                return;
            }

            List<Question> questions = Database.GetAllQuestions();

            if (questions.Count == 0)
            {
                MessageBox.Show("Er zijn nog geen vragen. Vraag de docent om vragen toe te voegen.");
                return;
            }

            BuildQuizPanel(questions);
            ShowOnly(panelQuiz);
        }

        private void BuildQuizPanelContainer()
        {
            panelQuiz = new Panel();
            panelQuiz.Dock = DockStyle.Fill;

            questionsPanel = new FlowLayoutPanel();
            questionsPanel.Dock = DockStyle.Fill;
            questionsPanel.AutoScroll = true;
            questionsPanel.FlowDirection = FlowDirection.TopDown;
            questionsPanel.WrapContents = false;
        }

        private void BuildQuizPanel(List<Question> questions)
        {
            questionsPanel.Controls.Clear();
            answerControls.Clear();

            int questionNumber = 0;

            foreach (Question question in questions)
            {
                questionNumber++;

                // Eigen paneel (geen GroupBox): zo mag de vraagtekst over meerdere regels lopen
                // in plaats van afgekapt te worden door een titelbalk.
                Panel box = new Panel();
                box.Width = 640;
                box.AutoSize = true;
                box.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                box.BorderStyle = BorderStyle.FixedSingle;
                box.Margin = new Padding(0, 0, 0, 12);
                box.Padding = new Padding(10);

                Label questionLabel = new Label();
                questionLabel.Text = questionNumber + ". " + question.Text;
                questionLabel.Font = new Font(this.Font, FontStyle.Bold);
                questionLabel.AutoSize = true;
                questionLabel.MaximumSize = new Size(600, 0);
                questionLabel.Location = new Point(10, 10);
                box.Controls.Add(questionLabel);

                FlowLayoutPanel inner = new FlowLayoutPanel();
                inner.FlowDirection = FlowDirection.TopDown;
                inner.AutoSize = true;
                inner.WrapContents = false;
                inner.Location = new Point(10, questionLabel.Bottom + 8);

                AnswerControl answerControl = new AnswerControl();
                answerControl.Question = question;

                if (question.Type == QuestionType.MultipleChoice)
                {
                    AddOptionIfNotEmpty(inner, answerControl, "A", question.OptionA);
                    AddOptionIfNotEmpty(inner, answerControl, "B", question.OptionB);
                    AddOptionIfNotEmpty(inner, answerControl, "C", question.OptionC);
                }
                else
                {
                    TextBox answerBox = new TextBox();
                    answerBox.Width = 500;
                    inner.Controls.Add(answerBox);
                    answerControl.OpenAnswerBox = answerBox;
                }

                box.Controls.Add(inner);
                questionsPanel.Controls.Add(box);
                answerControls.Add(answerControl);
            }

            Panel footer = new Panel();
            footer.Dock = DockStyle.Bottom;
            footer.Height = 50;

            Button btnSubmit = new Button();
            btnSubmit.Text = "Inleveren";
            btnSubmit.Location = new Point(10, 8);
            btnSubmit.Width = 120;
            btnSubmit.Click += btnSubmit_Click;
            footer.Controls.Add(btnSubmit);

            panelQuiz.Controls.Clear();
            panelQuiz.Controls.Add(questionsPanel);
            panelQuiz.Controls.Add(footer);
        }

        private void AddOptionIfNotEmpty(FlowLayoutPanel inner, AnswerControl answerControl, string letter, string optionText)
        {
            if (string.IsNullOrWhiteSpace(optionText))
            {
                return;
            }

            RadioButton radio = new RadioButton();
            radio.Text = letter + ": " + optionText;
            radio.Tag = letter;
            radio.AutoSize = true;

            inner.Controls.Add(radio);
            answerControl.Options.Add(radio);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            SubmitQuiz();
        }

        private void SubmitQuiz()
        {
            QuizResult result = new QuizResult();
            result.StudentName = txtName.Text.Trim();
            result.DateTaken = DateTime.Now;

            foreach (AnswerControl answerControl in answerControls)
            {
                string given = "";

                if (answerControl.OpenAnswerBox != null)
                {
                    given = answerControl.OpenAnswerBox.Text.Trim();
                }
                else
                {
                    foreach (RadioButton radio in answerControl.Options)
                    {
                        if (radio.Checked)
                        {
                            given = radio.Tag.ToString();
                        }
                    }
                }

                bool isCorrect = string.Equals(given, answerControl.Question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);

                QuizAnswer answer = new QuizAnswer();
                answer.QuestionId = answerControl.Question.Id;
                answer.GivenAnswer = given;
                answer.IsCorrect = isCorrect;
                answer.QuestionText = answerControl.Question.Text;
                answer.QuestionCorrectAnswer = answerControl.Question.CorrectAnswer;

                result.Answers.Add(answer);
            }

            result.TotalQuestions = result.Answers.Count;

            int correctCount = 0;
            foreach (QuizAnswer answer in result.Answers)
            {
                if (answer.IsCorrect)
                {
                    correctCount++;
                }
            }
            result.Score = correctCount;

            Database.SaveQuizResult(result);

            ShowResult(result);
        }

        private void BuildResultPanel()
        {
            panelResult = new Panel();
            panelResult.Dock = DockStyle.Fill;

            resultGrid = new DataGridView();
            resultGrid.Dock = DockStyle.Fill;
            resultGrid.ReadOnly = true;
            resultGrid.AllowUserToAddRows = false;
            resultGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            resultGrid.Columns.Add("Question", "Vraag");
            resultGrid.Columns.Add("Given", "Jouw antwoord");
            resultGrid.Columns.Add("Correct", "Juiste antwoord");
            resultGrid.Columns.Add("Ok", "Resultaat");

            scoreLabel = new Label();
            scoreLabel.Dock = DockStyle.Top;
            scoreLabel.Height = 40;
            scoreLabel.Font = new Font(SystemFonts.DefaultFont.FontFamily, 12, FontStyle.Bold);

            Button btnAgain = new Button();
            btnAgain.Text = "Nog een keer";
            btnAgain.Dock = DockStyle.Bottom;
            btnAgain.Height = 40;
            btnAgain.Click += btnAgain_Click;

            panelResult.Controls.Add(resultGrid);
            panelResult.Controls.Add(scoreLabel);
            panelResult.Controls.Add(btnAgain);
        }

        private void btnAgain_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            ShowOnly(panelName);
        }

        private void ShowResult(QuizResult result)
        {
            scoreLabel.Text = result.StudentName + " scoorde " + result.Score + " / " + result.TotalQuestions;

            resultGrid.Rows.Clear();

            foreach (QuizAnswer answer in result.Answers)
            {
                string outcome = "Fout";
                Color rowColor = Color.LightSalmon;

                if (answer.IsCorrect)
                {
                    outcome = "Goed";
                    rowColor = Color.LightGreen;
                }

                int rowIndex = resultGrid.Rows.Add(answer.QuestionText, answer.GivenAnswer, answer.QuestionCorrectAnswer, outcome);
                resultGrid.Rows[rowIndex].DefaultCellStyle.BackColor = rowColor;
            }

            ShowOnly(panelResult);
        }
    }
}
