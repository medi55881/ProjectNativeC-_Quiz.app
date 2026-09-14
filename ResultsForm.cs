using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuizApp.Simple.Data;
using QuizApp.Simple.Models;

namespace QuizApp.Simple
{
    // Docent: resultaten van studenten bekijken.
    public class ResultsForm : Form
    {
        private DataGridView grid;

        public ResultsForm()
        {
            this.Text = "Resultaten";
            this.Width = 700;
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
            grid.Columns.Add("Student", "Student");
            grid.Columns.Add("Score", "Score");
            grid.Columns.Add("Percentage", "Percentage");
            grid.Columns.Add("Date", "Datum");
            grid.Columns["Id"].Visible = false;
            grid.CellDoubleClick += grid_CellDoubleClick;

            Panel toolbar = new Panel();
            toolbar.Dock = DockStyle.Top;
            toolbar.Height = 45;

            Button btnDetails = new Button();
            btnDetails.Text = "Details bekijken";
            btnDetails.Location = new Point(10, 8);
            btnDetails.Width = 140;
            btnDetails.Click += btnDetails_Click;
            toolbar.Controls.Add(btnDetails);

            this.Controls.Add(grid);
            this.Controls.Add(toolbar);

            this.Load += ResultsForm_Load;
        }

        private void ResultsForm_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            List<QuizResult> results = Database.GetAllResults();

            grid.Rows.Clear();
            foreach (QuizResult result in results)
            {
                int percentage = 0;
                if (result.TotalQuestions > 0)
                {
                    percentage = (int)Math.Round(100.0 * result.Score / result.TotalQuestions);
                }

                grid.Rows.Add(
                    result.Id,
                    result.StudentName,
                    result.Score + " / " + result.TotalQuestions,
                    percentage + "%",
                    result.DateTaken.ToString("dd-MM-yyyy HH:mm"));
            }
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ShowDetails();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            ShowDetails();
        }

        private void ShowDetails()
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecteer eerst een resultaat.");
                return;
            }

            int id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            QuizResult result = Database.GetResultWithAnswers(id);

            if (result == null)
            {
                return;
            }

            ResultDetailsForm detailsForm = new ResultDetailsForm(result);
            detailsForm.ShowDialog(this);
        }
    }
}
