using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NEXORA_lakonyemmanuel
{
    public sealed partial class QuizPage : Page
    {
        private List<QuizQuestion> questions;
        private int currentQuestionIndex = 0;
        private int correctAnswers = 0;
        private int questionsAttempted = 0;
        private int timeLeft = 1200;
        private DispatcherTimer timer;
        private bool quizFinished = false;

        private int customerID = 0;
        private string studentName = "Customer";

        string connectionString =
            "Data Source=R103-PC02;Initial Catalog=EcommerceDB_LakonyEmmanuel;User ID=sa;Password=sap;Encrypt=False";
        public QuizPage()
        {
            this.InitializeComponent();

            LoadQuestions();

            txtprogress.Maximum = questions.Count;

            LoadQuestion();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            UpdateTimer();
        }

        private void LoadQuestions()
        {
            questions = new List<QuizQuestion>()
            {
                new QuizQuestion
                {
                    Question = "What is e-commerce?",
                    OptionA = "Buying and selling goods online",
                    OptionB = "Repairing computers",
                    OptionC = "Designing buildings",
                    OptionD = "Installing operating syaytems",
                    CorrectAnswer = "A"
                },

                new QuizQuestion
                {
                    Question = "Which of the following is an example of an e-commerce platform?",
                    OptionA = "Microsoft Word",
                    OptionB = "Amazon",
                    OptionC = "Calculator",
                    OptionD = "Notepad",
                    CorrectAnswer = "B"
                },

                new QuizQuestion
                {
                    Question = "Which feature allows a customer to keep goods before checkout",
                    OptionA = "Search engine",
                    OptionB = "Shopping cart",
                    OptionC = "Web browser",
                    OptionD = "Login Page",
                    CorrectAnswer = "B"
                },

                new QuizQuestion
                {
                    Question = "What is the main purpose of an online product catalogue?",
                    OptionA = "To display available products and their information",
                    OptionB = "To create passwords",
                    OptionC = "To manage employees",
                    OptionD = "Login Page",
                    CorrectAnswer = "A"
                },

                 new QuizQuestion
                {
                    Question = "Which information is useful when describing an electronic product?",
                    OptionA = "Product specifications",
                    OptionB = "Customer password",
                    OptionC = "Employee salary",
                    OptionD = "Database password",
                    CorrectAnswer = "A"
                },

            };
        }

        private void LoadQuestion()
        {
            QuizQuestion question = questions[currentQuestionIndex];

            txtquestionnumber.Text =
                "Question " + (currentQuestionIndex + 1) +
                " of " + questions.Count;

            txtquestion.Text = question.Question;

            radioa.Content = question.OptionA;
            radiob.Content = question.OptionB;
            radioc.Content = question.OptionC;
            radiod.Content = question.OptionD;

            txtprogress.Value = currentQuestionIndex + 1;

            radioa.IsChecked = false;
            radiob.IsChecked = false;
            radioc.IsChecked = false;
            radiod.IsChecked = false;

            if (currentQuestionIndex == questions.Count - 1)
            {
                btnnextquestion.Content = "SUBMIT QUIZ";
            }
            else
            {
                btnnextquestion.Content = "NEXT QUESTION";
            }
        }

        private string GetSelectedAnswer()
        {
            if (radioa.IsChecked == true)
                return "A";

            if (radiob.IsChecked == true)
                return "B";

            if (radioc.IsChecked == true)
                return "C";

            if (radiod.IsChecked == true)
                return "D";

            return "";
        }

        private async void btnnextquestion_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (quizFinished)
                return;

            string selectedAnswer = GetSelectedAnswer();

            if (selectedAnswer == "")
            {
                await MessageBox("Please select an answer before continuing.");
                return;
            }

            questionsAttempted++;

            if (selectedAnswer ==
                questions[currentQuestionIndex].CorrectAnswer)
            {
                correctAnswers++;
            }

            if (currentQuestionIndex < questions.Count - 1)
            {
                currentQuestionIndex++;
                LoadQuestion();
            }
            else
            {
                await FinishQuiz();
            }
        }

        private async void Timer_Tick(
            object sender,
            object e)
        {
            timeLeft--;

            UpdateTimer();

            if (timeLeft <= 0)
            {
                timer.Stop();

                await MessageBox("Time is up.");

                await FinishQuiz();
            }
        }

        private void UpdateTimer()
        {
            int minutes = timeLeft / 60;
            int seconds = timeLeft % 60;

            txttimeleft.Text =
                "Time Left: " +
                minutes.ToString("00") +
                ":" +
                seconds.ToString("00");
        }

        private async Task FinishQuiz()
        {
            if (quizFinished)
                return;

            quizFinished = true;

            timer.Stop();

            int wrongAnswers =
                questionsAttempted - correctAnswers;

            int scorePercentage =
                (correctAnswers * 100) / questions.Count;

            await SaveQuizResult(
                wrongAnswers,
                scorePercentage
            );

            await MessageBox(

                "Quiz Completed\n\n" +                
                "Questions: " + questionsAttempted +
                "\nCorrect Answers: " + correctAnswers +
                "\nWrong Answers: " + wrongAnswers +
                "\nScore: " + scorePercentage + "%"


            );

            Frame.Navigate(typeof(MenuPage));
        }

        private async Task SaveQuizResult(
            int wrongAnswers,
            int scorePercentage)
        {
            try
            {
                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query =
                        @"INSERT INTO QuizResult_tbl
                        (
                            CustomerID,
                            StudentName,
                            QuestionsAttempted,
                            CorrectAnswers,
                            WrongAnswer,
                            ScorePercentage,
                            AssessmentDate
                        )
                        VALUES
                        (
                            @CustomerID,
                            @StudentName,
                            @QuestionsAttempted,
                            @CorrectAnswers,
                            @WrongAnswer,
                            @ScorePercentage,
                            @AssessmentDate
                        )";

                    SqlCommand command =
                        new SqlCommand(query, connection);

                    if (customerID == 0)
                    {
                        command.Parameters.AddWithValue(
                            "@CustomerID",
                            DBNull.Value
                        );
                    }
                    else
                    {
                        command.Parameters.AddWithValue(
                            "@CustomerID",
                            customerID
                        );
                    }

                    command.Parameters.AddWithValue(
                        "@StudentName",
                        studentName
                    );

                    command.Parameters.AddWithValue(
                        "@QuestionsAttempted",
                        questionsAttempted
                    );

                    command.Parameters.AddWithValue(
                        "@CorrectAnswers",
                        correctAnswers
                    );

                    command.Parameters.AddWithValue(
                        "@WrongAnswer",
                        wrongAnswers
                    );

                    command.Parameters.AddWithValue(
                        "@ScorePercentage",
                        scorePercentage
                    );

                    command.Parameters.AddWithValue(
                        "@AssessmentDate",
                        DateTime.Now
                    );

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                await MessageBox(
                    "Result could not be saved. " +
                    ex.Message
                );
            }
        }

        private async Task MessageBox(string message)
        {
            ContentDialog dialog =
                new ContentDialog()
                {
                    Title = "E-Commerce Quiz Result",
                    Content = message,
                    CloseButtonText = "Exit"
                };

            await dialog.ShowAsync();
        }
    }

    public class QuizQuestion
    {
        public string Question { get; set; }

        public string OptionA { get; set; }

        public string OptionB { get; set; }

        public string OptionC { get; set; }

        public string OptionD { get; set; }

        public string CorrectAnswer { get; set; }
    }
}