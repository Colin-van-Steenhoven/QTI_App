using E4_The_Big_Three.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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

        public CreatePage()
        {
            this.InitializeComponent();
            
            currentTagId = tagId;

            using var db = new AppDbContext();

            using (var dbContext = new AppDbContext())
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
    }
}
