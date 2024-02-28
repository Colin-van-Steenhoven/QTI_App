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
using System.IO.Compression;
using System.Text;

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
            // Controleer of een item is geselecteerd.
            if (selectQuestionsLB.SelectedItems.Count > 0)
            {
                // Initialiseer een XML-document voor beoordelingsitems.
                XDocument xmlDocument = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("assessmentItems")
                );

                // Initialiseer een stringbuilder voor de inhoud van Imsmanifest.xml.
                StringBuilder manifestContent = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
                manifestContent.Append("<manifest identifier=\"MANIFEST_001\" version=\"1.0\">\n");
                manifestContent.Append("<resources>\n");

                // Itereer over geselecteerde items.
                foreach (var item in selectQuestionsLB.SelectedItems)
                {
                    // Ervan uitgaande dat 'item' een aangepast type 'Vraag' is met eigenschappen 'Id' en 'Tekst'.
                    Question question = item as Question;
                    if (question != null)
                    {
                        // Maak voor elke vraag een 'assessmentItem'-element aan.
                        XElement assessmentItem = new XElement("assessmentItem",
                            new XAttribute("identifier", question.Id),
                            new XElement("itemBody",
                                new XElement("choiceInteraction",
                                    new XElement("prompt", question.Text)
                                // Voeg hier meer elementen toe indien nodig, bijvoorbeeld keuzes voor een meerkeuzevraag.
                                )
                            )
                        );

                        // Voeg het 'assessmentItem'-element toe aan de hoofdstructuur.
                        xmlDocument.Root.Add(assessmentItem);

                        // Voeg een bronvermelding toe voor de vraag.
                        manifestContent.Append($"<resource identifier=\"{question.Id}\" type=\"imsqti_item_xmlv2p1\">\n");
                        manifestContent.Append($"<file href=\"QuestionFiles/{question.Id}.xml\"/>\n");
                        manifestContent.Append("</resource>\n");
                    }
                }

                // Sluit het gedeelte met bronnen in het manifest af.
                manifestContent.Append("</resources>\n");
                manifestContent.Append("</manifest>");

                // Sla het XML-document op naar een bestand.
                string xmlFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/QuestionFiles/Question_ID.xml";
                xmlDocument.Save(xmlFilePath);

                // Sla het manifestbestand op.
                string manifestFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/QuestionFiles/Imsmanifest.xml";
                File.WriteAllText(manifestFilePath, manifestContent.ToString());

                // Zip beide bestanden.
                string zipFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/QuestionFiles/QuestionFiles.zip";
                using (var zip = new ZipArchive(File.Create(zipFilePath), ZipArchiveMode.Create))
                {
                    zip.CreateEntryFromFile(xmlFilePath, Path.GetFileName(xmlFilePath));
                    zip.CreateEntryFromFile(manifestFilePath, Path.GetFileName(manifestFilePath));
                }

                // Verwijder individuele bestanden.
                File.Delete(xmlFilePath);
                File.Delete(manifestFilePath);
            }
        }
    }
}
