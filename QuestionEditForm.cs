using System;
using System.Drawing;
using System.Windows.Forms;
using QuizApp.Simple.Models;

namespace QuizApp.Simple
{
    // Formulier om één vraag toe te voegen of te wijzigen.
    public class QuestionEditForm : Form
    {
        private TextBox txtText;
        private ComboBox cmbType;
        private TextBox txtOptionA;
        private TextBox txtOptionB;
        private TextBox txtOptionC;
        private TextBox txtCorrectAnswer;

        public Question Question;

        public QuestionEditForm() : this(null)
        {
        }

        public QuestionEditForm(Question questionToEdit)
        {
            if (questionToEdit == null)
            {
                Question = new Question();
                this.Text = "Nieuwe vraag";
            }
            else
            {
                Question = questionToEdit;
                this.Text = "Vraag wijzigen";
            }

            this.Width = 570;
            this.Height = 340;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblText = new Label();
            lblText.Text = "Vraag:";
            lblText.Location = new Point(20, 20);
            lblText.AutoSize = true;

            txtText = new TextBox();
            txtText.Location = new Point(150, 20);
            txtText.Width = 380;
            txtText.Height = 50;
            txtText.Multiline = true;

            Label lblType = new Label();
            lblType.Text = "Type:";
            lblType.Location = new Point(20, 83);
            lblType.AutoSize = true;

            cmbType = new ComboBox();
            cmbType.Location = new Point(150, 80);
            cmbType.Width = 200;
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.Items.Add(QuestionType.MultipleChoice);
            cmbType.Items.Add(QuestionType.Open);

            Label lblA = new Label();
            lblA.Text = "Antwoord A:";
            lblA.Location = new Point(20, 118);
            lblA.AutoSize = true;

            txtOptionA = new TextBox();
            txtOptionA.Location = new Point(150, 115);
            txtOptionA.Width = 380;

            Label lblB = new Label();
            lblB.Text = "Antwoord B:";
            lblB.Location = new Point(20, 148);
            lblB.AutoSize = true;

            txtOptionB = new TextBox();
            txtOptionB.Location = new Point(150, 145);
            txtOptionB.Width = 380;

            Label lblC = new Label();
            lblC.Text = "Antwoord C:";
            lblC.Location = new Point(20, 178);
            lblC.AutoSize = true;

            txtOptionC = new TextBox();
            txtOptionC.Location = new Point(150, 175);
            txtOptionC.Width = 380;

            Label lblCorrect = new Label();
            lblCorrect.Text = "Juiste antwoord:";
            lblCorrect.Location = new Point(20, 213);
            lblCorrect.AutoSize = true;

            txtCorrectAnswer = new TextBox();
            txtCorrectAnswer.Location = new Point(150, 210);
            txtCorrectAnswer.Width = 380;

            Label lblHint = new Label();
            lblHint.Text = "(bij meerkeuze: A, B of C. Bij een open vraag: het exacte antwoord.)";
            lblHint.Location = new Point(20, 235);
            lblHint.AutoSize = true;
            lblHint.ForeColor = Color.Gray;

            Button btnOk = new Button();
            btnOk.Text = "Opslaan";
            btnOk.Location = new Point(360, 260);
            btnOk.Width = 80;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Click += btnOk_Click;

            Button btnCancel = new Button();
            btnCancel.Text = "Annuleren";
            btnCancel.Location = new Point(450, 260);
            btnCancel.Width = 80;
            btnCancel.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblText);
            this.Controls.Add(txtText);
            this.Controls.Add(lblType);
            this.Controls.Add(cmbType);
            this.Controls.Add(lblA);
            this.Controls.Add(txtOptionA);
            this.Controls.Add(lblB);
            this.Controls.Add(txtOptionB);
            this.Controls.Add(lblC);
            this.Controls.Add(txtOptionC);
            this.Controls.Add(lblCorrect);
            this.Controls.Add(txtCorrectAnswer);
            this.Controls.Add(lblHint);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            LoadFromQuestion();
        }

        private void LoadFromQuestion()
        {
            txtText.Text = Question.Text;
            cmbType.SelectedItem = Question.Type;
            txtOptionA.Text = Question.OptionA;
            txtOptionB.Text = Question.OptionB;
            txtOptionC.Text = Question.OptionC;
            txtCorrectAnswer.Text = Question.CorrectAnswer;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtText.Text) || string.IsNullOrWhiteSpace(txtCorrectAnswer.Text))
            {
                MessageBox.Show("Vul minimaal de vraag en het juiste antwoord in.");
                this.DialogResult = DialogResult.None;
                return;
            }

            Question.Text = txtText.Text.Trim();

            if (cmbType.SelectedItem != null)
            {
                Question.Type = (QuestionType)cmbType.SelectedItem;
            }

            Question.OptionA = txtOptionA.Text;
            Question.OptionB = txtOptionB.Text;
            Question.OptionC = txtOptionC.Text;
            Question.CorrectAnswer = txtCorrectAnswer.Text.Trim();
        }
    }
}
