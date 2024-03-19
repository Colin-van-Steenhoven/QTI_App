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
using Microsoft.EntityFrameworkCore;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace QTI_App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private NavigationService _navigationService;
        public HomePage()
        {
            this.InitializeComponent();
            InitializeQuestions();
        }
        private void InitializeQuestions()
        {
            using (var db = new AppDbContext())
            {
                var questions = db.Questions
                    .Include(b => b.QuestionTags)
                    .ThenInclude(qt => qt.Tag)
                    .Include(a => a.Answers)
                    .ToList();
                questionsLv.ItemsSource = questions;
            }
        }
        private void questionsLv_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (questionsLv.SelectedItem != null)
            {
                var selectedQuestion = (Question)questionsLv.SelectedItem;

                Frame.Navigate(typeof(EditPage), selectedQuestion);
            }
        }
        private void addNewQuestionB_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CreatePage));
        }
       private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                string searchText = searchTextBox.Text.ToLower();
                var filteredQuestions = db.Questions.Where(q => q.Text.ToLower().Contains(searchText)).ToList();
                questionsLv.ItemsSource = filteredQuestions;
            }
        }   

        private void exportQuestionB_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ExportQuestionPage));
        }
    }
}
