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

namespace QTI_App.Pages.CRUD
{
    public sealed partial class EditPage : Page
    {
        private int questionId;
        private Question selectedQuestion;

        public EditPage()
        {
            LoadSelectedQuestion(questionId);
        }

        private void LoadSelectedQuestion(int questionId)
        {
            using (var db = new AppDbContext())
            {
                selectedQuestion = db.questions.FirstOrDefault(n => n.Id == questionId);

                if (selectedQuestion != null)
                {
                    questionTB.Text = selectedQuestion.Text;
                }
            }
        }

        private void saveB_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
