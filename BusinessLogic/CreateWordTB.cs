using Microsoft.Win32;
using NotationTB.UserControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.MSProject;

namespace NotationTB.BusinessLogic
{
    public class CreateWordTB
    {
        private static void FindAndReplace(Microsoft.Office.Interop.Word.Application doc, object findText, object replaceWithText)
        {
            //options
            object matchCase = false;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object matchAllWordForms = true;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiacritics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object read_only = false;
            object visible = true;
            object replace = 2;
            object wrap = 1;
            //execute find and replace
            doc.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
                ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
                ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);
        }
        public static void CreateWord(List<NotationPart> notationParts)
        {
            string dummyFileName = "Save Here";
            string exeDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            SaveFileDialog sf = new SaveFileDialog();
            sf.FileName = dummyFileName;
            sf.CheckFileExists = false;
            sf.Filter = "Directory | directory";
            if (sf.ShowDialog() == true)
            {
                string savePath = System.IO.Path.GetDirectoryName(sf.FileName);
                string tnName = "Название";
                string tbType = "Тип";
                string tbClass = "Класс";
                int tbClassIndex = 0;
                int joinType = 0;

                var wordApp = new Microsoft.Office.Interop.Word.Application();
                wordApp.Visible = true;
                Microsoft.Office.Interop.Word.Document wordDoc = wordApp.Documents.Open(exeDir + @"\\Pattern\\tbPattern.docx");
                string filePath = @"\\" + tnName + ".docx";
                try
                {

                    FindAndReplace(wordApp, "tbName", tnName);
                    FindAndReplace(wordApp, "tbType", tbType);
                    FindAndReplace(wordApp, "tbClass", tbClass);
                    int cellIndex = 5;
                    bool itFirstPartControl = true;
                    bool itLastPartControlIsBracing = false;
                    Dictionary<int, int> SelectedOperationIds = new();
                    Microsoft.Office.Interop.Word.Table table = wordDoc.Tables[1];
                    int сolumnsIndex = 4;
                    foreach (NotationPart notationPart in notationParts)
                    {
                        foreach (var value in notationPart.BindValues)
                        {
                            if (!SelectedOperationIds.ContainsKey(value.Key))
                            {
                                SelectedOperationIds[value.Key] = сolumnsIndex;
                                table.Columns.Add();
                                table.Cell(3, сolumnsIndex).Range.Text = value.Key.ToString();
                                сolumnsIndex++;
                            }
                        }
                        
                    }
                    if(сolumnsIndex == 4)
                        return;
                    int rowIndex = 4;
                    foreach (NotationPart notationPart in notationParts)
                    {
                        table.Rows.Add();
                        table.Cell(rowIndex, 1).Range.Text = notationPart.DetailName;
                        table.Cell(rowIndex, 2).Range.Text = notationPart.PlanName;
                        table.Cell(rowIndex,3).Range.Text = notationPart.MaterialStamp;
                        foreach (var value in notationPart.BindValues)
                        {
                            if (SelectedOperationIds.ContainsKey(value.Key))
                            {
                                table.Cell(rowIndex, SelectedOperationIds[value.Key]).Range.Text = "+";
                            }
                        }

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
                catch (System.Exception e)
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
}
