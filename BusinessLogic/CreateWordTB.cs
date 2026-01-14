using System.IO;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Windows.Controls;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using NotationTB.Data;
using NotationTB.UserControl;

namespace NotationTB.BusinessLogic;

public class CreateWordTB
{
    //private static void FindAndReplace(Application wordApp, object findText, object replaceWithText)
    //{
    //    wordApp.ActiveWindow.View.ShowFieldCodes = true;
    //    //options
    //    object matchCase = false;
    //    object matchWholeWord = false;
    //    object matchWildCards = false;
    //    object matchSoundsLike = false;
    //    object matchAllWordForms = false;
    //    object forward = true;
    //    object format = false;
    //    object matchKashida = false;
    //    object matchDiacritics = false;
    //    object matchAlefHamza = false;
    //    object matchControl = false;
    //    object matchPrefix = false;
    //    object matchSuffix = false;
    //    object matchPhrase = false;
    //    object ignoreSpace = true;
    //    object ignorePunct = true;
    //    object replace = 2;
    //    object wrap = 1;
    //    //execute find and replace
    //    //doc.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
    //    //    ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format,
    //    //    ref replaceWithText, ref replace,
    //    //    ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);
    //    //var range = wordApp.ActiveDocument.Range(0, wordApp.ActiveDocument.Content.End);
    //    //var header =
    //    //range.Delete();
    //    wordApp.Selection.Find.Execute(
    //        ref findText,
    //        ref matchCase,
    //        ref matchWholeWord,
    //        ref matchWildCards,
    //        ref matchSoundsLike,
    //        ref matchAllWordForms,
    //        ref forward,
    //        ref wrap,
    //        ref format,
    //        ref replaceWithText,
    //        ref replace,
    //        ref matchKashida,
    //        ref matchDiacritics,
    //        ref matchAlefHamza,
    //        ref matchControl
    //       );
    //}

    private static void FindAndReplace(Microsoft.Office.Interop.Word.Application wordApplication, string findText, object replaceWithText)
    {
        //ЭТУ ФУНКЦИЮ НЕЛЬЗЯ ПРИМЕНЯТЬ К БОЛЬШИМ ДОКУМЕНТАМ, ОНА ОЧЕНЬ СИЛЬНО НА НИХ ТОРМОЗИТ
        wordApplication.ActiveWindow.View.ShowFieldCodes = true;
        var wordDocument = wordApplication.ActiveWindow.Document;
        foreach (Microsoft.Office.Interop.Word.Section section in wordDocument.Sections)
        {
            wordDocument.TrackRevisions = false; //Disable Tracking for the Field replacement operation
            //получение всех верхних колонтитулов
            Microsoft.Office.Interop.Word.HeadersFooters headers = section.Headers;
            //замена во всех верхних колонтитулах
            foreach (Microsoft.Office.Interop.Word.HeaderFooter header in headers)
            {
                ReplaceInRange(header.Range, findText, replaceWithText);
                //замена во всех фигурах (таблицы, это тоже фигуры в колонтитулах)
                foreach (Microsoft.Office.Interop.Word.Shape shape in header.Shapes)
                {
                    if (shape.TextFrame.HasText != 0)
                        ReplaceInRange(shape.TextFrame.TextRange, findText, replaceWithText);
                }
            }
            //получение всех нижних колонтитулов
            Microsoft.Office.Interop.Word.HeadersFooters footers = section.Footers;
            //замена во всех нижних колонтитулах
            foreach (Microsoft.Office.Interop.Word.HeaderFooter footer in footers)
            {
                ReplaceInRange(footer.Range, findText, replaceWithText);
                //замена во всех фигурах (таблицы, это тоже фигуры в колонтитулах)
                foreach (Microsoft.Office.Interop.Word.Shape shape in footer.Shapes)
                {
                    if (shape.TextFrame.HasText != 0)
                        ReplaceInRange(shape.TextFrame.TextRange, findText, replaceWithText);
                }
            }

            
        }
        //замена во всех текстовых полях
        foreach (Microsoft.Office.Interop.Word.Shape shape in wordDocument.Shapes)
        {
            if (shape.TextFrame.HasText != 0)
                ReplaceInRange(shape.TextFrame.TextRange, findText, replaceWithText);
        }
        //замена в основном тексте документа
        ReplaceInRange(wordDocument.Content, findText, replaceWithText);
    }


    private static void ReplaceInRange(Microsoft.Office.Interop.Word.Range range, object findText, object replaceWithText)
    {
        object matchCase = false;
        object matchWholeWord = false;
        object matchWildCards = false;
        object matchSoundsLike = false;
        object matchAllWordForms = false;
        object forward = true;
        object format = false;
        object matchKashida = false;
        object matchDiacritics = false;
        object matchAlefHamza = false;
        object matchControl = false;
        object matchPrefix = false;
        object matchSuffix = false;
        object matchPhrase = false;
        object ignoreSpace = true;
        object ignorePunct = true;
        object replace = 2;
        object wrap = 1;
        range.Find.Execute(
            ref findText,
            ref matchCase,
            ref matchWholeWord,
            ref matchWildCards,
            ref matchSoundsLike,
            ref matchAllWordForms,
            ref forward,
            ref wrap,
            ref format,
            ref replaceWithText,
            ref replace,
            ref matchKashida,
            ref matchDiacritics,
            ref matchAlefHamza,
            ref matchControl
            );
    }
    public static void CreateWord(List<NotationPart> notationParts, string tbName)
    {
        var dummyFileName = "Save Here";
        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var sf = new SaveFileDialog();
        sf.FileName = dummyFileName;
        sf.CheckFileExists = false;
        sf.Filter = "Directory | directory";
        if (sf.ShowDialog() == true)
        {
            var savePath = Path.GetDirectoryName(sf.FileName);
            var tbType = "Тип";
            var tbClass = "Класс";
            var tbClassIndex = 0;
            var joinType = 0;

            var wordApp = new Application();
            wordApp.Visible = true;
            var wordDoc = wordApp.Documents.Open(exeDir + @"\\Pattern\\tbPattern.docx");
            var filePath = @"\\" + tbName + ".docx";
            try
            {
                FindAndReplace(wordApp, "tbName", tbName);
                FindAndReplace(wordApp, "tbType", tbType);
                FindAndReplace(wordApp, "tbClass", tbClass);
                var cellIndex = 5;
                var itFirstPartControl = true;
                var itLastPartControlIsBracing = false;
                Dictionary<int, int> SelectedOperationIds = new();
                var table = wordDoc.Tables[1];
                var сolumnsIndex = 4;
                foreach (var notationPart in notationParts)
                foreach (var value in notationPart.BindValues)
                    if (!SelectedOperationIds.ContainsKey(value.Key))
                    {
                        SelectedOperationIds[value.Key] = сolumnsIndex;
                        table.Columns.Add();
                        table.Cell(3, сolumnsIndex).Range.Text = value.Key.ToString();
                        using (var db = new AppDbContext())
                        {
                            table.Cell(2, сolumnsIndex).Range.Text = db.OperationsTypes.Where(o => o.Id == value.Key)
                                .FirstOrDefault().Name;
                        }

                        сolumnsIndex++;
                    }

                if (сolumnsIndex == 4)
                    return;
                var rowIndex = 4;
                foreach (var notationPart in notationParts)
                {
                    table.Rows.Add();
                    table.Cell(rowIndex, 1).Range.Text = notationPart.DetailName;
                    table.Cell(rowIndex, 2).Range.Text = notationPart.PlanName;
                    table.Cell(rowIndex, 3).Range.Text = notationPart.MaterialStamp;
                    foreach (var value in notationPart.BindValues)
                        if (SelectedOperationIds.ContainsKey(value.Key))
                            table.Cell(rowIndex, SelectedOperationIds[value.Key]).Range.Text = "+C";

                    rowIndex++;
                }

                table.Cell(1, 4).Range.Text = "Наименование операции / Operation name";
                table.Cell(1, 4).Range.Orientation = WdTextOrientation.wdTextOrientationHorizontal;
                table.Cell(1, 4).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                table.Cell(1, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                table.Cell(1, 4).Merge(table.Cell(1, сolumnsIndex - 1));
                table.Cell(1, 1).Merge(table.Cell(2, 1));
                table.Cell(1, 2).Merge(table.Cell(2, 2));
                table.Cell(1, 3).Merge(table.Cell(2, 3));
                table.Cell(3, 1).Merge(table.Cell(3, 3));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                wordDoc.SaveAs2(savePath + filePath);
            }
        }
    }
}