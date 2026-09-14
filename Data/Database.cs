using System;
using System.Collections.Generic;
using System.IO;
using MySqlConnector;
using QuizApp.Simple.Models;

namespace QuizApp.Simple.Data
{
    // Deze klasse doet al het praatwerk met de MySQL-database.
    // Elke methode doet één ding: verbinding openen, SQL uitvoeren, verbinding weer sluiten.
    // Dit is dezelfde database (Laragon, database "quizapp") als de webversie van de quiz-app.
    public static class Database
    {
        private const string ConnectionString = "Server=localhost;Port=3306;Database=quizapp;User=root;Password=;";

        // ---------- Vragen ----------

        public static List<Question> GetAllQuestions()
        {
            List<Question> questions = new List<Question>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT Id, Text, Type, OptionA, OptionB, OptionC, CorrectAnswer " +
                             "FROM Questions ORDER BY Id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(ReadQuestion(reader));
                        }
                    }
                }
            }

            return questions;
        }

        public static bool HasAnyQuestions()
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM Questions";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    long count = Convert.ToInt64(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public static Question GetQuestionById(int id)
        {
            Question question = null;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT Id, Text, Type, OptionA, OptionB, OptionC, CorrectAnswer " +
                             "FROM Questions WHERE Id = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            question = ReadQuestion(reader);
                        }
                    }
                }
            }

            return question;
        }

        public static void AddQuestion(Question question)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "INSERT INTO Questions (Text, Type, OptionA, OptionB, OptionC, CorrectAnswer) " +
                             "VALUES (@text, @type, @a, @b, @c, @correct)";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@text", question.Text);
                    command.Parameters.AddWithValue("@type", (int)question.Type);
                    command.Parameters.AddWithValue("@a", question.OptionA);
                    command.Parameters.AddWithValue("@b", question.OptionB);
                    command.Parameters.AddWithValue("@c", question.OptionC);
                    command.Parameters.AddWithValue("@correct", question.CorrectAnswer);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateQuestion(Question question)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "UPDATE Questions SET Text = @text, Type = @type, OptionA = @a, " +
                             "OptionB = @b, OptionC = @c, CorrectAnswer = @correct WHERE Id = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@text", question.Text);
                    command.Parameters.AddWithValue("@type", (int)question.Type);
                    command.Parameters.AddWithValue("@a", question.OptionA);
                    command.Parameters.AddWithValue("@b", question.OptionB);
                    command.Parameters.AddWithValue("@c", question.OptionC);
                    command.Parameters.AddWithValue("@correct", question.CorrectAnswer);
                    command.Parameters.AddWithValue("@id", question.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteQuestion(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "DELETE FROM Questions WHERE Id = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static Question ReadQuestion(MySqlDataReader reader)
        {
            Question question = new Question();
            question.Id = reader.GetInt32("Id");
            question.Text = reader.GetString("Text");
            question.Type = (QuestionType)reader.GetInt32("Type");
            question.OptionA = GetStringOrEmpty(reader, "OptionA");
            question.OptionB = GetStringOrEmpty(reader, "OptionB");
            question.OptionC = GetStringOrEmpty(reader, "OptionC");
            question.CorrectAnswer = reader.GetString("CorrectAnswer");
            return question;
        }

        private static string GetStringOrEmpty(MySqlDataReader reader, string columnName)
        {
            int columnIndex = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(columnIndex))
            {
                return "";
            }
            return reader.GetString(columnIndex);
        }

        // ---------- Quizresultaten ----------

        public static void SaveQuizResult(QuizResult result)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string insertResultSql = "INSERT INTO QuizResults (StudentName, Score, TotalQuestions, DateTaken) " +
                                          "VALUES (@name, @score, @total, @date); " +
                                          "SELECT LAST_INSERT_ID();";

                int newResultId;

                using (MySqlCommand command = new MySqlCommand(insertResultSql, connection))
                {
                    command.Parameters.AddWithValue("@name", result.StudentName);
                    command.Parameters.AddWithValue("@score", result.Score);
                    command.Parameters.AddWithValue("@total", result.TotalQuestions);
                    command.Parameters.AddWithValue("@date", result.DateTaken);

                    object idResult = command.ExecuteScalar();
                    newResultId = Convert.ToInt32(idResult);
                }

                result.Id = newResultId;

                foreach (QuizAnswer answer in result.Answers)
                {
                    string insertAnswerSql = "INSERT INTO QuizAnswers (QuizResultId, QuestionId, GivenAnswer, IsCorrect) " +
                                              "VALUES (@resultId, @questionId, @given, @correct)";

                    using (MySqlCommand command = new MySqlCommand(insertAnswerSql, connection))
                    {
                        command.Parameters.AddWithValue("@resultId", newResultId);
                        command.Parameters.AddWithValue("@questionId", answer.QuestionId);
                        command.Parameters.AddWithValue("@given", answer.GivenAnswer);
                        command.Parameters.AddWithValue("@correct", answer.IsCorrect);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static List<QuizResult> GetAllResults()
        {
            List<QuizResult> results = new List<QuizResult>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT Id, StudentName, Score, TotalQuestions, DateTaken " +
                             "FROM QuizResults ORDER BY DateTaken DESC";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            QuizResult result = new QuizResult();
                            result.Id = reader.GetInt32("Id");
                            result.StudentName = reader.GetString("StudentName");
                            result.Score = reader.GetInt32("Score");
                            result.TotalQuestions = reader.GetInt32("TotalQuestions");
                            result.DateTaken = reader.GetDateTime("DateTaken");
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        public static QuizResult GetResultWithAnswers(int resultId)
        {
            QuizResult result = null;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string resultSql = "SELECT Id, StudentName, Score, TotalQuestions, DateTaken " +
                                    "FROM QuizResults WHERE Id = @id";

                using (MySqlCommand command = new MySqlCommand(resultSql, connection))
                {
                    command.Parameters.AddWithValue("@id", resultId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new QuizResult();
                            result.Id = reader.GetInt32("Id");
                            result.StudentName = reader.GetString("StudentName");
                            result.Score = reader.GetInt32("Score");
                            result.TotalQuestions = reader.GetInt32("TotalQuestions");
                            result.DateTaken = reader.GetDateTime("DateTaken");
                        }
                    }
                }

                if (result == null)
                {
                    return null;
                }

                string answersSql = "SELECT a.GivenAnswer, a.IsCorrect, a.QuestionId, q.Text, q.CorrectAnswer " +
                                     "FROM QuizAnswers a " +
                                     "JOIN Questions q ON q.Id = a.QuestionId " +
                                     "WHERE a.QuizResultId = @resultId";

                using (MySqlCommand command = new MySqlCommand(answersSql, connection))
                {
                    command.Parameters.AddWithValue("@resultId", resultId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            QuizAnswer answer = new QuizAnswer();
                            answer.QuizResultId = resultId;
                            answer.QuestionId = reader.GetInt32("QuestionId");
                            answer.GivenAnswer = reader.GetString("GivenAnswer");
                            answer.IsCorrect = reader.GetBoolean("IsCorrect");
                            answer.QuestionText = reader.GetString("Text");
                            answer.QuestionCorrectAnswer = reader.GetString("CorrectAnswer");
                            result.Answers.Add(answer);
                        }
                    }
                }
            }

            return result;
        }

        // ---------- Importeren ----------
        // Ondersteunt alleen .csv (simpeler dan .csv + .json). Formaat, kolommen gescheiden door ";":
        //   id;question;answer_a;answer_b;answer_c;correct_answer
        // Zijn answer_a/b/c leeg?  -> open vraag, correct_answer is het verwachte antwoord.
        // Anders                  -> meerkeuzevraag, correct_answer is "a", "b" of "c".

        public static List<Question> ReadQuestionsFromCsvFile(string filePath)
        {
            List<Question> questions = new List<Question>();
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // Eerste regel is de kop (id;question;answer_a;answer_b;answer_c;correct_answer) -> overslaan.
                if (i == 0 || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(';');
                if (parts.Length < 6)
                {
                    continue;
                }

                string text = parts[1].Trim();
                string optionA = parts[2].Trim();
                string optionB = parts[3].Trim();
                string optionC = parts[4].Trim();
                string correctAnswer = parts[5].Trim();

                if (string.IsNullOrWhiteSpace(text))
                {
                    continue;
                }

                bool hasOptions = optionA != "" || optionB != "" || optionC != "";

                Question question = new Question();
                question.Text = text;
                question.OptionA = optionA;
                question.OptionB = optionB;
                question.OptionC = optionC;

                if (hasOptions)
                {
                    question.Type = QuestionType.MultipleChoice;
                    question.CorrectAnswer = correctAnswer.ToUpper();
                }
                else
                {
                    question.Type = QuestionType.Open;
                    question.CorrectAnswer = correctAnswer;
                }

                questions.Add(question);
            }

            return questions;
        }
    }
}
