using QTI_App.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using QTI_App.Controllers;
using QTI_App.Pages;
using QTI_App.Pages.CRUD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QTI_App.Pages.CRUD
{
    public sealed partial class EditPage : Page
    {
        private Question selectedQuestion;
        private ObservableCollection<Answer> answers = new ObservableCollection<Answer>();

        private int? currentTagId;

        private ObservableCollection<Tag> currentQuestionTags;
        public int? tagId = null;

        public EditPage()
        {
            InitializeComponent();
            answerListView.ItemsSource = answers;
            currentQuestionTags = new ObservableCollection<Tag>();
            TagListView.ItemsSource = currentQuestionTags;

            currentTagId = tagId;
            using (var db = new AppDbContext())
            {
                tagsComboBox.ItemsSource = db.Tags.ToList();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            selectedQuestion = (Question)e.Parameter;
            foreach (var questiontag in selectedQuestion.QuestionTags) 
            {
                currentQuestionTags.Add(questiontag.Tag);
            }
            foreach (var questionawnser in selectedQuestion.Answers) 
            {
                answers.Add(questionawnser);
            }

            questionTB.Text = selectedQuestion.Text.ToString();
        }

        private void saveB_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var question = db.Questions
                    .Include(b => b.QuestionTags)
                    .ThenInclude(qt => qt.Tag)
                    .Include(a => a.Answers)
                    .First(x => x.Id == selectedQuestion.Id);

                // Update question text
                question.Text = questionTB.Text;

                //// Track removed answers
                var removedAnswers = new List<Answer>();
                foreach (var dbAnswer in question.Answers)
                {
                    if (!answers.Any(a => a.Id == dbAnswer.Id))
                    {
                        removedAnswers.Add(dbAnswer);
                    }
                }

                foreach (var removedAnswer in removedAnswers)
                {
                    question.Answers.Remove(removedAnswer);
                }

                // Track updated answers
                foreach (var answer in answers)
                {
                    var existingAnswer = question.Answers.FirstOrDefault(a => a.Id == answer.Id);
                    if (existingAnswer != null)
                    {
                        existingAnswer.Text = answer.Text;
                        existingAnswer.IsCorrect = answer.IsCorrect;
                    }
                }

                // Track added answers
                var addedAnswers = new List<Answer>();
                foreach (var currentAnswer in answers)
                {
                    if (!question.Answers.Any(a => a.Id == currentAnswer.Id))
                    {
                        addedAnswers.Add(new Answer
                        {
                            IsCorrect = currentAnswer.IsCorrect,
                            Text = currentAnswer.Text,
                        });
                    }
                }

                foreach (var addedAnswer in addedAnswers)
                {
                    question.Answers.Add(addedAnswer);
                }

                //// Track removed tags
                var removedTags = new List<QuestionTag>();
                foreach (var dbQuestionTag in question.QuestionTags)
                {
                    if (!currentQuestionTags.Any(a => a.Id == dbQuestionTag.Tag.Id))
                    {
                        removedTags.Add(dbQuestionTag);
                    }
                }

                foreach (var removedTag in removedTags)
                {
                    question.QuestionTags.Remove(removedTag);
                }

                //// Track added tags
                var addedTags = new List<QuestionTag>();
                foreach (var currentTag in currentQuestionTags)
                {
                    if (!question.QuestionTags.Any(a => a.Tag.Id == currentTag.Id))
                    {
                        addedTags.Add(new QuestionTag
                        {
                            TagId = currentTag.Id,
                        });
                    }
                }

                foreach (var addedTag in addedTags)
                {
                    question.QuestionTags.Add(addedTag);
                }

                // Save changes to the database
                db.SaveChanges();

                Frame.GoBack();
            }
        }

        private void deleteAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var answer = (Answer)button.DataContext;

            answers.Remove(answer);
        }

        private async void addAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowMakeAnswerDialogAsync();
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

        private void TagRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var tag = (Tag)button.DataContext;

            currentQuestionTags.Remove(tag);
        }
        private async Task ShowErrorDialog(string errorMessage)
        {
            ErrorMessageText.Text = errorMessage;
            await errorDialog.ShowAsync();
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
    }
}
