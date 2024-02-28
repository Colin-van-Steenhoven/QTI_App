using E4_The_Big_Three.Data;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace QTI_App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();

            using var db = new AppDbContext();
            var questions = db.questions.ToList();
            questionsLv.ItemsSource = questions;
        }


        private void questionsLv_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (questionsLv.SelectedItem != null)
            {
                var selectedQuestion = (Question)questionsLv.SelectedItem;
                int questionId = (int)selectedQuestion.Id;

                Frame.Navigate(typeof(EditPage), questionId);
            }
        }

        private void addNewQuestionB_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
