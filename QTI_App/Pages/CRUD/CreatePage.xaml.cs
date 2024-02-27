using E4_The_Big_Three.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public CreatePage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            using var db = new AppDbContext();

            using (var dbContext = new AppDbContext())
            {
                var text = QuestionTextBox.Text;

                db.questions.Add(new Question 
                {
                    Text = text 
                });



               db.SaveChanges();

                /*questions = dbContext.questions.ToList();
                QuestionTextBox.ItemsSource = questions;*/
            }

        }
    }
}
