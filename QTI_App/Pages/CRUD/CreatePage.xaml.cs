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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        private int? currentTagId;

        private List<Tag> tags;
        private ObservableCollection<Tag> currentQuestionTags;
        public int? tagId = null;
        public CreatePage()
        {
            this.InitializeComponent();
            answerListView.ItemsSource = answers;
            currentQuestionTags = new ObservableCollection<Tag>();

            currentTagId = tagId;
            using (var db = new AppDbContext())
            {
                tagsComboBox.ItemsSource = db.Tags.ToList();
            }
        }

        private void saveB_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();

            var text = questionTB.Text;

            if (answers.Count >= 2 && answers.Any(a => a.IsCorrect))
            {
                var newQuestion = new Question
                {
                    Text = text
                };

                db.Questions.Add(newQuestion);
                db.SaveChanges();


                foreach (var currentQuestionTag in currentQuestionTags)
                {
                    var tag = db.Tags.First(g => g.Id == currentQuestionTag.Id);

                    db.QuestionTags.Add(new QuestionTag
                    {
                        Question = newQuestion,
                        Tag = tag
                    });

                }

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
                ShowErrorDialog("There must be at least 2 answers with at least 1 correct answer.");
            }
        }
        private void TagRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var tag = (Tag)button.DataContext;

            currentQuestionTags.Remove(tag);
        }

        private void AddTagButton_Click_1(object sender, RoutedEventArgs e)
        {
            var selectedTag = (Tag)tagsComboBox.SelectedItem;

            if (selectedTag == null)
            {
                return;
            }
            if (!currentQuestionTags.Contains(selectedTag))
            {
                currentQuestionTags.Add(selectedTag);
                TagListView.ItemsSource = currentQuestionTags;
            }
            else
            {
                ShowErrorDialog("Tag already added.");
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
