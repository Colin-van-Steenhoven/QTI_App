using QTI_App.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using QTI_App.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace QTI_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreatePage : Page
    {
        private List<Question> questions;
        private ObservableCollection<Answer> answers = new ObservableCollection<Answer>();


        public CreatePage()
        {
            this.InitializeComponent();
            answerListView.ItemsSource = answers;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            using var db = new AppDbContext();

            var text = QuestionTextBox.Text;

            if (answers.Any(a => a.IsCorrect))
            {
                var newQuestion = new Question
                {
                    Text = text
                };

                db.Questions.Add(newQuestion);
                db.SaveChanges();

                foreach (var answer in answers)
                {
                    db.Answers.Add(new Answer
                    {
                        Text = answer.Text,
                        IsCorrect = answer.IsCorrect,
                        QuestionId = newQuestion.Id
                    }); 
                }

                db.SaveChanges();
                this.Frame.GoBack();
            }
            else
            {
                ShowErrorDialog("Er moet minimaal 1 goed antwoord zijn.");
            }

        }

        private async void addAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowMakeAnswerDialogAsync();
        }


        private async Task ShowMakeAnswerDialogAsync()
        {
            var result = await makeAnswernDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                answers.Add(new Answer
                {
                    Text = answerTb.Text,
                    IsCorrect = (bool)isCorrectCheckBox.IsChecked,
                    
                });

                isCorrectCheckBox.IsChecked = false;
                answerTb.Text = string.Empty;

            }
        }
        private async Task ShowErrorDialog(string errorMessage)
        {
            ErrorMessageText.Text = errorMessage;
            await errorDialog.ShowAsync();
        }

        private void deleteAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var answer = (Answer)button.DataContext;

            answers.Remove(answer);
        }
    }
}
