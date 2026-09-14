using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuizApp.Simple.Data;
using QuizApp.Simple.Models;

namespace QuizApp.Simple
{
    // Docent: vragenbank beheren (toevoegen, wijzigen, verwijderen, importeren).
    public class QuestionsForm : Form
    {
        private DataGridView grid;

        public QuestionsForm()
        {
            this.Text = "Vragen beheren";
            this.Width = 800;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;

            grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.Columns.Add("Id", "Id");
            grid.Columns.Add("Text", "Vraag");
            grid.Columns.Add("Type", "Type");
            grid.Columns.Add("CorrectAnswer", "Juist antwoord");
            grid.Columns["Id"].Visible = false;

            Panel toolbar = new Panel();
            toolbar.Dock = DockStyle.Top;
            toolbar.Height = 45;

            Button btnAdd = new Button();
            btnAdd.Text = "Toevoegen";
            btnAdd.Location = new Point(10, 8);
            btnAdd.Width = 100;
            btnAdd.Click += btnAdd_Click;

            Button btnEdit = new Button();
            btnEdit.Text = "Wijzigen";
            btnEdit.Location = new Point(120, 8);
            btnEdit.Width = 100;
            btnEdit.Click += btnEdit_Click;

            Button btnDelete = new Button();
            btnDelete.Text = "Verwijderen";
            btnDelete.Location = new Point(230, 8);
            btnDelete.Width = 100;
            btnDelete.Click += btnDelete_Click;

            Button btnImport = new Button();
            btnImport.Text = "Importeren (csv)...";
            btnImport.Location = new Point(340, 8);
            btnImport.Width = 150;
            btnImport.Click += btnImport_Click;

            toolbar.Controls.Add(btnAdd);
            toolbar.Controls.Add(btnEdit);
            toolbar.Controls.Add(btnDelete);
            toolbar.Controls.Add(btnImport);

            this.Controls.Add(grid);
            this.Controls.Add(toolbar);

            this.Load += QuestionsForm_Load;
        }

        private void QuestionsForm_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            List<Question> questions = Database.GetAllQuestions();

            grid.Rows.Clear();
            foreach (Question question in questions)
            {
                grid.Rows.Add(question.Id, question.Text, question.Type, question.CorrectAnswer);
            }
        }

        private int GetSelectedQuestionId()
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecteer eerst een vraag.");
                return -1;
            }

            return (int)grid.SelectedRows[0].Cells["Id"].Value;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            QuestionEditForm dialog = new QuestionEditForm();
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            Database.AddQuestion(dialog.Question);
            RefreshGrid();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int id = GetSelectedQuestionId();
            if (id == -1)
            {
                return;
            }

            Question question = Database.GetQuestionById(id);
            if (question == null)
            {
                return;
            }

            QuestionEditForm dialog = new QuestionEditForm(question);
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            Database.UpdateQuestion(dialog.Question);
            RefreshGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int id = GetSelectedQuestionId();
            if (id == -1)
            {
                return;
            }

            DialogResult confirm = MessageBox.Show("Deze vraag verwijderen?", "Bevestigen", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            Database.DeleteQuestion(id);
            RefreshGrid();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Vragenbestand (*.csv)|*.csv";

            if (openFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            List<Question> imported = Database.ReadQuestionsFromCsvFile(openFileDialog.FileName);

            foreach (Question question in imported)
            {
                Database.AddQuestion(question);
            }

            RefreshGrid();
            MessageBox.Show(imported.Count + " vragen geïmporteerd.");
        }
    }
}
