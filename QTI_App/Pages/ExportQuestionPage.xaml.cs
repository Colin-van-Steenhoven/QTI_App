using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using E4_The_Big_Three.Data;
using System.Xml.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace QTI_App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExportQuestionPage : Page
    {
        public ExportQuestionPage()
        {
            this.InitializeComponent();

            using var db = new AppDbContext();
            var exportQuestions = db.questions.ToList();
            selectQuestionsLB.ItemsSource = exportQuestions;
        }

        private void generateQTIB_Click(object sender, RoutedEventArgs e)
        {
            // Check if any item is selected
            if (selectQuestionsLB.SelectedItems.Count > 0)
            {
                // Initialize an XML document
                XDocument qtiDocument = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("assessmentItems")
                );

                // Iterate over selected items
                foreach (var item in selectQuestionsLB.SelectedItems)
                {
                    // Assuming 'item' is of a custom type 'Question' that has 'Id' and 'Text' properties
                    Question question = item as Question;
                    if (question != null)
                    {
                        // Create an 'assessmentItem' element for each question
                        XElement assessmentItem = new XElement("assessmentItem",
                            new XAttribute("identifier", question.Id),
                            new XElement("itemBody",
                                new XElement("choiceInteraction",
                                    new XElement("prompt", question.Text)
                                // Add more elements here as needed, e.g., choices for a multiple-choice question
                                )
                            )
                        );

                        // Add the 'assessmentItem' element to the root
                        qtiDocument.Root.Add(assessmentItem);
                    }
                }

                // Save the QTI document to a file
                string filePath = "\\questionFiles";
                qtiDocument.Save(filePath);

            //    // Inform the user that the file has been generated
            //    MessageBox.Show($"QTI file has been generated at {filePath}");
            //}
            //else
            //{
            //    MessageBox.Show("Please select at least one question.");
            }
        }
    }
}
