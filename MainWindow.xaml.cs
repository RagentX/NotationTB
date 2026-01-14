//using NotationTB.SqlTables;

using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using NotationTB.BusinessLogic;
using NotationTB.BusinessLogic.Object;
using NotationTB.Data;
using NotationTB.Models;
using NotationTB.UserControl;

namespace NotationTB
{

    //в предпросмотре выводить только таблицу контроля качества с наименованиями операций в таблице

    //марка стали Стандарт и ТУ на пф в один столбец
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int userID;
        private DbSet<ClassificationDesignation> classDesignations;

        public event Action<double, double, double, double, double, double> OnUpdateSize;
        public event Action<int> OnUpdateClassificationDesignation;

        private ObservableCollection<NotationPart> notationParts = new ObservableCollection<NotationPart>();
        private List<OperationsType> allOperations = new();
        private Dictionary<int, bool> SelectedOperationIds = new();
        private List<OptionalRule> optionalRules = new();

        private Dictionary<OptionalRule, CheckBox> optionalRulesToCheckBox = new Dictionary<OptionalRule, CheckBox>();

        /// <summary>
        /// Главное окно приложения
        /// </summary>
        /// <param name="userID">Код пользователя</param>
        public MainWindow(int userID)
        {
            InitializeComponent();
            this.userID = userID;
            TbNameTextBox.Text = "test_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            using (var db = new AppDbContext())
            {
                classDesignations = db.ClassificationDesignations;
                foreach (var materialsStamp in classDesignations)
                {
                    ClassificationDesignationsComboBox.Items.Add(materialsStamp);
                }
            }
            OnUpdateClassificationDesignation += UpdateOtherRules;
        }

        private void UpdateSizeNotationTable(object sender, SizeChangedEventArgs e)
        {
            OnUpdateSize?.Invoke(HeaderGrid.ColumnDefinitions[0].ActualWidth,
                HeaderGrid.ColumnDefinitions[2].ActualWidth,
                HeaderGrid.ColumnDefinitions[4].ActualWidth,
                HeaderGrid.ColumnDefinitions[6].ActualWidth,
                HeaderGrid.ColumnDefinitions[8].ActualWidth,
                HeaderGrid.ColumnDefinitions[10].ActualWidth);
        }


        private void AddDetailButton_Click(object sender, RoutedEventArgs e)
        {
            AddDetail();
        }

        private void AddDetail()
        {
            NotationPart notationPart = new NotationPart();
            notationParts.Add(notationPart);
            notationPart.mainOptionalRulesToCheckBox = optionalRulesToCheckBox;
            OnUpdateSize += notationPart.UpdateSize;
            OnUpdateSize?.Invoke(NameHeaderLabel.ActualWidth,
                PlanNameHeaderLabel.ActualWidth,
                MaterialStampHeaderLabel.ActualWidth,
                MaterialStandartHeaderLabel.ActualWidth,
                ProductStandartHeaderLabel.ActualWidth,
                OtherRuleHeaderLabel.ActualWidth);
            OnUpdateClassificationDesignation += notationPart.UpdateDesignationId;
            if (ClassificationDesignationsComboBox.SelectedIndex >= 0)
                OnUpdateClassificationDesignation.Invoke(
                    (ClassificationDesignationsComboBox.SelectedItem as ClassificationDesignation).Id);
            detailsStackPanel.Children.Add(notationPart);
        }

        private void ClassificationDesignationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnUpdateClassificationDesignation.Invoke(
                (ClassificationDesignationsComboBox.SelectedItem as ClassificationDesignation).Id);
        }

        private void OperationsTypesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OperationsTypesWindow operationsTypesWindow = new OperationsTypesWindow();
            operationsTypesWindow.Show();
        }

        private void AddMatComb_Click(object sender, RoutedEventArgs e)
        {
            AddMatCombinations addMatCombinations = new();
            addMatCombinations.Show();
        }
        
        private void Test_Click(object sender, RoutedEventArgs e)
        {
            MaterialWindow materialWindow = new MaterialWindow();
            materialWindow.Show();
        }

        private void PreviewDataGridOperationsUpdate()
        {
            using (AppDbContext db = new AppDbContext())
            {
                allOperations = db.OperationsTypes.AsNoTracking().OrderBy(o => o.Id).ToList();
            }

            //тут включение операций для отображения
            SelectedOperationIds.Clear();
            foreach (var previewRow in notationParts)
            {
                foreach (var value in previewRow.BindValues)
                {
                    SelectedOperationIds[value.Key] = true;
                }
            }

        }

        public void PreviewDataGridUpdate()
        {
            PreviewDataGrid.Columns.Clear();
            // Классификация
            PreviewDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Наименование детали",
                Binding = new Binding("DetailName")
            });
            PreviewDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Обозначение по чертежу",
                Binding = new Binding("PlanName")
            });
            PreviewDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Марка стали",
                Binding = new Binding("MaterialStamp")
            });
            PreviewDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Стандарт или ТУ",
                Binding = new Binding("MaterialStandard")
            });
            PreviewDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ТУ на полуфабрикат",
                Binding = new Binding("ProductStandard")
            });
            foreach (var opId in SelectedOperationIds)
            {
                var op = allOperations.FirstOrDefault(o => o.Id == opId.Key && opId.Value == true);
                if (op == null) continue;

                PreviewDataGrid.Columns.Add(new DataGridCheckBoxColumn()
                {
                    Header = new TextBlock
                    {
                        Text = op.Id.ToString(),
                        ToolTip = op.Name
                    },
                    Binding = new Binding($"BindValues[{opId.Key}]")
                });
            }

            PreviewDataGrid.ItemsSource = notationParts;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Name as string;

            switch (tabItem)
            {
                case "PreviewTabItem":
                    PreviewDataGridOperationsUpdate();
                    PreviewDataGridUpdate();
                    break;
                default:
                    return;
            }
        }

        public void UpdateOtherRules(int classDesignationId)
        {
            using (var db = new AppDbContext())
            {
                optionalRules = db.OptionalRules.Where(o =>
                    (o.DesignationId == classDesignationId || o.DesignationId == null) &&
                    o.ForAll == true &&
                    o.MaterialTypeId == null).ToList();
                OptionalRulesMenu.Items.Clear();
                optionalRulesToCheckBox.Clear();
                foreach (var optionalRule in optionalRules)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Content = optionalRule.ToString();
                    checkBox.Checked += UpdateCheckBoxHeader;
                    checkBox.Unchecked += UpdateCheckBoxHeader;
                    optionalRulesToCheckBox[optionalRule] = checkBox;
                    OptionalRulesMenu.Items.Add(checkBox);
                }

                UpdateCheckBoxHeader();
            }
        }

        private void UpdateCheckBoxHeader(object sender, RoutedEventArgs e)
        {
            UpdateCheckBoxHeader();
        }
        private void UpdateCheckBoxHeader()
        {
            string header = "Выбрано: ";
            foreach (var optionalRuleToCheckBox in optionalRulesToCheckBox)
            {
                if (optionalRuleToCheckBox.Value.IsChecked == true)
                {
                    header += optionalRuleToCheckBox.Value.Content + ", ";
                }
            }

            OptionalRulesMenu.Header = header;
            PreviewDataGridUpdate();
        }

        private void SaveWordButton_Click(object sender, RoutedEventArgs e)
        {
            CreateWordTB.CreateWord(notationParts.ToList(), TbNameTextBox.Text);
        }

        private void DellButton_Click(object sender, RoutedEventArgs e)
        {
            detailsStackPanel.Children.Clear();
            List<NotationPart> removeList = new List<NotationPart>();
            foreach (var notationPart in notationParts)
            {
                if (notationPart.IsSelected)
                {
                    removeList.Add(notationPart);
                }
                else
                {
                    detailsStackPanel.Children.Add(notationPart);
                }
            }

            foreach (var notationPart in removeList)
            {
                notationParts.Remove(notationPart);
            }
        }
    }
}