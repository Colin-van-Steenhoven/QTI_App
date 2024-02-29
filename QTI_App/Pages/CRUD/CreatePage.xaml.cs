using Microsoft.EntityFrameworkCore;
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
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace QTI_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreatePage : Page
    {
        private int? currentTagId;

        private List<Question> questions;
        private List<Tag> tags;
        private ObservableCollection<Tag> currentQuestionTags;
        public int? tagId = null;
        private List<Question> questions;
        private ObservableCollection<Answer> answers = new ObservableCollection<Answer>();


        public CreatePage()
        {
            this.InitializeComponent();
            
            currentTagId = tagId;
            answerListView.ItemsSource = answers;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            using var db = new AppDbContext();

            var text = QuestionTextBox.Text;

            if (answers.Any(a => a.IsCorrect))
            {

                currentQuestionTags = new ObservableCollection<Tag>();
                TagListView.ItemsSource = Tag;

            }
            RefreshTagComboBox();
            RefreshQuestionInformation();
        }

        private void RefreshTagComboBox()
        {
            using (var db = new AppDbContext())
            {
                tagsComboBox.ItemsSource = db.tags.ToList();
            }
        }

        private void RefreshQuestionInformation()
        {
            if (currentTagId == null)
            {
                currentQuestionTags = new ObservableCollection<Tag>();
                TagListView.ItemsSource = Tag;
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

            using (var db = new AppDbContext())
            {
                var question = db.questions
                    .Include(g => g.QuestionTags)
                    .FirstOrDefault(g => g.Id == currentTagId);
            }

            currentQuestionTags = new ObservableCollection<Tag>();
            TagListView.ItemsSource = currentQuestionTags;
        }

        private void SaveQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                Question question;

                if (currentTagId == null)
                {
                    question = new Question();
                    question.Tags = new List<Tag>();
                }
                else
                {
                    question = db.questions
                        .Include(g => g.Tags)
                        .FirstOrDefault(g => g.Id == currentTagId);
                }

                var text = QuestionTextBox.Text;

                var questionTags = new List<QuestionTag>();

                foreach (var currentQuestionTag in currentQuestionTags)
                {
                    var tag = db.tags.First(g => g.Id == currentQuestionTag.Id);

                    var questionTag = new QuestionTag
                    {
                        Question = question,
                        Tag = tag
                    };

                    questionTags.Add(questionTag);
                }

                question.QuestionTags = questionTags;

                if (currentTagId == null)
                {
                    db.questions.Add(new Question
                    {
                        Text = text,
                        QuestionTags = questionTags
                    });
                }

                db.SaveChanges();
            }
            RefreshQuestionInformation();
        }


        private void TagRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var tag = (Tag)button.DataContext;

            currentQuestionTags.Remove(tag);
        }

        private void AddTagButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTag = (Tag)tagsComboBox.SelectedItem;

            if (selectedTag == null)
            {
                return;
            }

            if (!currentQuestionTags.Any(tag => tag.Id == selectedTag.Id))
            {
                currentQuestionTags.Add(selectedTag);
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
