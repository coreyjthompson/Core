using System;
using System.Diagnostics;

using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;
using MEI.Web.Models;
using Syncfusion.Presentation;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.AspNetCore.Html;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MEI.Core.DomainModels.Training;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;

using MEI.AbbVie.Infrastructure.Queries.Training;
using MEI.Core.Commands;

using Microsoft.Extensions.Logging;

using Syncfusion.Drawing;
using Syncfusion.PresentationRenderer;

using Module = MEI.Core.DomainModels.Training.Module;
using MEI.AbbVie.Infrastructure.Training.Commands;
using System.Text.Encodings.Web;

namespace MEI.Web.Controllers
{
    public class MaterialsController : Controller
    {
        private readonly IQueryProcessor _abbvieQueries;
        private readonly ICommandProcessor _abbvieCommands;
        private readonly ILogger<MaterialsController> _logger;

        public MaterialsController(IQueryProcessor abbvieQueries, ICommandProcessor abbvieCommands, ILogger<MaterialsController> logger)
        {
            _abbvieQueries = abbvieQueries;
            _abbvieCommands = abbvieCommands;
            _logger = logger;
        }

        ///// <summary>
        ///// </summary>
        ///// <returns></returns>
        public async Task ConvertAllCurrentTrainingPowerpointDecksToImagesAsync()
        {
            var query = new GetAllActiveTrainingModulesWithPptxForConversionQuery();
            var tasks = new List<Task>();
            var trainings = await _abbvieQueries.Execute(query);

            // For testing, only get the top 5 decks
            trainings = trainings.Take(5).ToList();

            foreach (var training in trainings)
            {
                tasks.Add(ConvertPptxToImagesAsync(training));
            }

            await Task.WhenAll(tasks);
        }

        public async Task ConvertTrainingPowerpointDeckToImagesByTrainingId(int id)
        {
            var query = new GetTrainingByIdQuery(){
                TrainingId = id
            };

            var training = await _abbvieQueries.Execute(query);
            await ConvertPptxToImagesAsync(training);
        }

        public async Task WritePptxNoteSlidesToDatabaseAsync(Module trainingModule, IPresentation pptx)
        {
            var chapters = trainingModule.Chapters.OrderBy(c => c.Sequence);
            var noteSlides = new List<SlideSpeakerNotes>();

            foreach (var chapter in chapters)
            {
                var slides = chapter.Slides.OrderBy(s => s.Sequence);

                foreach (var slide in slides)
                {
                    noteSlides.Add(new SlideSpeakerNotes { SlideId = slide.Id, SlideDeckId = trainingModule.SlideDeckId });
                }
            }

            if (noteSlides.Count == pptx.Slides.Count)
            {
                for (var i = 0; i < noteSlides.Count; i++)
                {
                    noteSlides[i].Contents = ConvertNoteSlideToHtml(pptx.Slides[i].NotesSlide.NotesTextBody);
                }
            }

            foreach (var slide in noteSlides)
            {
                if (!string.IsNullOrEmpty(slide.Contents) && slide.Contents != "<p></p>\r\n")
                {
                    await _abbvieCommands.Execute(new AddSpeakerNoteSlideCommand
                    {
                        SlideDeckId = slide.SlideDeckId,
                        SlideId = slide.SlideId,
                        Contents = slide.Contents
                    });
                }

            }
        }

        private IList<int> GetVideoSlideNumbers(string fileName)
        {
            var list = new List<int>();
            var numberOfSlides = CountSlides(fileName);

            for (var i = 0; i < numberOfSlides; i++)
            {
                if (IsVideoInSlide(fileName, i))
                {
                    list.Add(i + 1);
                }
            }

            return list;
        }

        // good
        /// <summary>
        /// </summary>
        /// <param name="notesTextBody"></param>
        /// <returns>
        /// </returns>
        private string ConvertNoteSlideToHtml(ITextBody notesTextBody)
        {
            var html = new StringBuilder();
            var olCounter = 1;

            foreach (var p in notesTextBody.Paragraphs)
            {
                var paragraph = new TagBuilder("p");
                TagBuilder lineItem = null;
                var innerHtml = new StringBuilder();

                if (p.ListFormat.Type.ToString().Trim() == "NotDefined" || p.ListFormat.Type.ToString().Trim() == "Bulleted")
                {
                    if (p.TextParts.Any())
                    {
                        paragraph = new TagBuilder("ul");
                        lineItem = new TagBuilder("li");

                        foreach (var t in p.TextParts)
                        {
                            if (!string.IsNullOrWhiteSpace(t.Text))
                            {
                                var span = ConvertTextPartToSpanTag(t);
                                innerHtml.AppendLine(GetString(span));
                            }
                            else
                            {
                                paragraph = null;
                                lineItem = null;
                            }
                        }
                    }
                }

                if (p.ListFormat.Type.ToString().Trim() == "Numbered")
                {
                    if (p.TextParts.Any())
                    {
                        paragraph = new TagBuilder("ol");
                        paragraph.Attributes.Add("start", olCounter.ToString());
                        lineItem = new TagBuilder("li");
                        foreach (var t in p.TextParts)
                        {
                            if (!string.IsNullOrWhiteSpace(t.Text))
                            {
                                var span = ConvertTextPartToSpanTag(t);
                                innerHtml.AppendLine(GetString(span));
                            }
                            else
                            {
                                paragraph = null;
                                lineItem = null;
                            }
                        }

                        olCounter++;
                    }
                }

                if (p.ListFormat.Type.ToString().Trim() == "None")
                {
                    if (p.TextParts.Any())
                    {
                        paragraph = new TagBuilder("p");
                        lineItem = null;

                        foreach (var t in p.TextParts)
                        {
                            if (!string.IsNullOrWhiteSpace(t.Text))
                            {
                                var span = ConvertTextPartToSpanTag(t);
                                innerHtml.AppendLine(GetString(span));
                            }
                            else
                            {
                                paragraph = null;
                                lineItem = null;
                            }
                        }
                    }
                }

                if (paragraph == null)
                {
                    continue;
                }

                if (lineItem != null)
                {
                    lineItem.InnerHtml.AppendHtml(innerHtml.ToString());
                    paragraph.InnerHtml.AppendHtml(GetString(lineItem));
                }
                else
                {
                    paragraph.InnerHtml.AppendHtml(innerHtml.ToString());
                }

                html.AppendLine(GetString(paragraph));
            }

            return html.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="textPart"></param>
        /// <returns>
        /// </returns>
        private TagBuilder ConvertTextPartToSpanTag(ITextPart textPart)
        {
            var span = new TagBuilder("span");

            if (textPart.Font.Bold)
            {
                span.AddCssClass("font-weight-bold");
            }

            if (textPart.Font.Subscript)
            {
                span.AddCssClass("text-subscript");
            }

            if (textPart.Font.Superscript)
            {
                span.AddCssClass("text-superscript");
            }

            if (textPart.Font.Italic)
            {
                span.AddCssClass("font-italic");
            }

            span.InnerHtml.AppendHtml(textPart.Text);

            return span;
        }

        public static string GetString(IHtmlContent content)
        {
            using (var writer = new StringWriter())
            {
                content.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }

        private string ResolveApplicationTrainingImagePath(string fileName)
        {
            //TODO: figure out the path depending on the client

            //var dataPath = new DirectoryInfo(Request.PhysicalPath + "..\\..\\..\\App_Data\\Presentation\\TrainingSlides").FullName;
            var dataPath = new DirectoryInfo("\\\\websvr2\\e$\\website_virtual_directories\\training_assets\\corey_test_folder_do_not_use").FullName;
            //var dataPath = new DirectoryInfo("corey_test_folder_do_not_use").FullName;

            return $"{dataPath}\\{fileName}";
        }

        private string ResolveApplicationSlideKitFolderPath(string fileName)
        {
            //TODO: figure out the path depending on the client

            // Strutural heart: \\romeo\e$\inetpub\wwwroot\AbbottProgramsWebProdBlue\Slidekits
            // abbvie: \\\\websvr2\\e$\\website_virtual_directories\\slidekit_assets
            var dataPath = new DirectoryInfo(@"\\websvr2\e$\website_virtual_directories\slidekit_assets").FullName;

            return $"{dataPath}\\{fileName}";
        }

        private int CountSlides(string presentationFile)
        {
            // Open the presentation as read-only.
            using (var presentationDocument = PresentationDocument.Open(presentationFile, false))
            {
                // Pass the presentation to the next CountSlides method
                // and return the slide count.
                return CountSlides(presentationDocument);
            }
        }

        // Count the slides in the presentation.
        private int CountSlides(PresentationDocument presentationDocument)
        {
            // Check for a null document object.
            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            var slidesCount = 0;

            // Get the presentation part of document.
            var presentationPart = presentationDocument.PresentationPart;

            // Get the slide count from the SlideParts.
            if (presentationPart != null)
            {
                slidesCount = presentationPart.SlideParts.Count();
            }

            // Return the slide count to the previous method.
            return slidesCount;
        }

        private bool IsVideoInSlide(string docName, int index)
        {
            using (var ppt = PresentationDocument.Open(docName, false))
            {
                // get the relationship ID of the first slide
                var part = ppt.PresentationPart;
                var slideIds = part.Presentation.SlideIdList.ChildElements;

                var relId = ((SlideId)slideIds[index]).RelationshipId;

                // get the slide part from the relationship ID
                var slide = (SlidePart)part.GetPartById(relId);

                // get the videos in the slide
                var videos = slide.Slide.Descendants<DocumentFormat.OpenXml.Presentation.Video>();
                var test = videos.Count();
                return videos.Any();
            }
        }

        private async Task ConvertPptxToImagesAsync(Module trainingModule)
        {

            var filePath = ResolveApplicationSlideKitFolderPath(trainingModule.SlideDeck.FileName);

            if (System.IO.File.Exists(filePath))
            {
                var sequence = 1;
                var counter = 1;
                var videoSlideList = GetVideoSlideNumbers(filePath);

                using (IPresentation pptxDoc = Syncfusion.Presentation.Presentation.Open(filePath))
                {
                    //Initialize the PresentationRenderer to perform image conversion
                    pptxDoc.PresentationRenderer = new PresentationRenderer();

                    try
                    {
                        foreach (var slide in pptxDoc.Slides)
                        {
                            using Stream stream = slide.ConvertToImage(ExportImageFormat.Jpeg);
                            var number = counter;
                            // image name format: TP096_CH02_SL0031.jpg
                            var imageName = string.Format("TP{0:0000}_CH{1:00}_SL{2:0000}.jpg", trainingModule.Id, sequence, number);

                            //Create the output image file stream
                            using (FileStream fileStreamOutput = System.IO.File.Create(ResolveApplicationTrainingImagePath(imageName)))
                            {
                                //Copy the converted image stream into created output stream
                                stream.CopyTo(fileStreamOutput);
                            }

                            counter++;

                            if (videoSlideList.Contains(counter) || videoSlideList.Contains(number))
                            {
                                sequence++;
                            }
                        }

                        // TODO: figure out how to bring this into the slide loop
                        await WritePptxNoteSlidesToDatabaseAsync(trainingModule, pptxDoc);

                        _logger.LogInformation(string.Format("Training ID {0} was successfully converted to JPG in {1}", trainingModule.Id, new DirectoryInfo("corey_test_folder_do_not_use").FullName));

                    }
                    catch (Exception e)
                    {
                        var message = string.Format("Slide deck for Training ID {0} threw an error while converting and could not finish: file name {1}", trainingModule.Id, filePath);
                        _logger.LogError(e, message);
                    }
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public ActionResult ReadPptxNoteSlides(Module trainingModule, IPresentation pptx)
        {
            //var file = ResolveApplicationDataPath("64E-1868716 Gastro DSA IBD Dialogue Institutional Deck.pptx");
            var text = new StringBuilder();

            //var presentation = Presentation.Open(file); // opens the specified presentation

            //HttpContext.Session["presentation"] = presentation; // saves it for later use

            foreach (var slide in pptx.Slides)
            {
                text.AppendLine("Slide #" + slide.SlideNumber);
                text.AppendLine("<br/>");
                text.AppendLine(ConvertNoteSlideToHtml(slide.NotesSlide.NotesTextBody));
                text.AppendLine("<br/><br/>");
            }

            return Content(text.ToString());
        }

    }
}