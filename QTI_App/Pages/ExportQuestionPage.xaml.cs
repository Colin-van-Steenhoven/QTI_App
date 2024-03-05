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
using Microsoft.EntityFrameworkCore;

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
            var tagList = db.Tags.ToList();
            var exportQuestions = db.Questions.Include(q => q.Answers)
                                              .Include(q => q.QuestionTags)
                                              .ThenInclude(t => t.Tag)
                                              .ToList();
            selectQuestionsLB.ItemsSource = exportQuestions;
            searchTagCB.ItemsSource = tagList;

            searchTagCB.SelectionChanged += SearchTagCB_SelectionChanged;
        }

        private void SearchTagCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchTagCB.SelectedItem != null)
            {
                using var db = new AppDbContext();
                var selectedTag = (Tag)searchTagCB.SelectedItem;
                var filteredQuestions = db.Questions.Include(q => q.Answers)
                                                    .Include(q => q.QuestionTags)
                                                    .ThenInclude(t => t.Tag)
                                                    .Where(q => q.QuestionTags.Any(qt => qt.TagId == selectedTag.Id))
                                                    .ToList();
                selectQuestionsLB.ItemsSource = filteredQuestions;
            }
            else
            {
                // If no tag is selected, show all questions
                using var db = new AppDbContext();
                var exportQuestions = db.Questions.Include(q => q.Answers)
                                                  .Include(q => q.QuestionTags)
                                                  .ThenInclude(t => t.Tag)
                                                  .ToList();
                selectQuestionsLB.ItemsSource = exportQuestions;
            }
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
                    // Haal gegevens op uit de database voor de vraag.
                    Question question = GetQuestionDataFromDatabase(item);

                    // Maak voor elke vraag een 'assessmentItem'-element aan.
                    XElement assessmentItem = new XElement("assessmentItem",
                        new XAttribute("identifier", question.Id),
                        new XElement("itemBody",
                            new XElement("choiceInteraction",
                                new XElement("prompt", question.Text)
                            )
                        )
                    );

                    // Voeg antwoorden toe aan de vraag, indien beschikbaar.
                    foreach (var answer in question.Answers)
                    {
                        // Maak een 'simpleChoice'-element aan voor elk antwoord.
                        XElement simpleChoice = new XElement("simpleChoice",
                            new XAttribute("identifier", answer.Id),
                            answer.Text
                        );

                        // Voeg het 'simpleChoice'-element toe aan de vraag.
                        assessmentItem.Element("itemBody")
                                     .Element("choiceInteraction")
                                     .Add(simpleChoice);
                    }

                    // Voeg tags toe aan de vraag, indien beschikbaar.
                    foreach (var tag in question.QuestionTags)
                    {
                        // Voeg de naam van de tag toe aan het 'assessmentItem'-element.
                        assessmentItem.Add(new XElement("tag", tag.Tag.Name));
                    }

                    // Voeg het 'assessmentItem'-element toe aan de hoofdstructuur.
                    xmlDocument.Root.Add(assessmentItem);

                    // Voeg een bronvermelding toe voor de vraag.
                    manifestContent.Append($"<resource identifier=\"{question.Id}\" type=\"imsqti_item_xmlv2p1\">\n");
                    manifestContent.Append($"<file href=\"QuestionFiles/{question.Id}.xml\"/>\n");
                    manifestContent.Append("</resource>\n");
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

        // Methode om vraaggegevens uit de database op te halen
        private Question GetQuestionDataFromDatabase(object item)
        { 
            // Controleer of het item een Question-object is
            if (!(item is Question questionItem))
            {
                // Als het item geen Question-object is, retourneer null of handel de fout af volgens jouw vereisten
                return null;
            }

            using (var db = new AppDbContext())
            {
                // Haal de vraag op uit de database op basis van het Id van het Question-object
                var question = db.Questions
                                 .Include(q => q.Answers) // Inclusief antwoorden
                                 .Include(q => q.QuestionTags)// Inclusief tags van de vraag
                                 .ThenInclude(t => t.Tag)//Haalt vanuit de QuestionTags klasse de Tag op
                                 .SingleOrDefault(q => q.Id == questionItem.Id);

                return question;
            }
        }
    }
}
