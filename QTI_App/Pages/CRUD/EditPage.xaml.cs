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

namespace QTI_App.Pages.CRUD
{
    public sealed partial class EditPage : Page
    {
        private Question selectedQuestion;

        public EditPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            selectedQuestion = (Question)e.Parameter;
            questionTB.Text = selectedQuestion.Text.ToString();
        }

        private void saveB_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                selectedQuestion.Text = questionTB.Text;
                db.questions.Update(selectedQuestion);
                db.SaveChangesAsync();

                Frame.GoBack();
            }


        }
    }
}
