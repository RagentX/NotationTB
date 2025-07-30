//using NotationTB.SqlTables;

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

        private List<NotationPart> notationParts = new List<NotationPart>();
        private List<OperationsType> allOperations = new();
        private Dictionary<int, bool> SelectedOperationIds = new();

        /// <summary>
        /// Главное окно приложения
        /// </summary>
        /// <param name="userID">Код пользователя</param>
        public MainWindow(int userID)
        {
            InitializeComponent();
            this.userID = userID;
            using (var db = new AppDbContext())
            {
                classDesignations = db.ClassificationDesignations;
                foreach (var materialsStamp in classDesignations)
                {
                    ClassificationDesignationsComboBox.Items.Add(materialsStamp);
                }
            }

            AddDetail();


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
            OnUpdateSize += notationPart.UpdateSize;
            OnUpdateSize?.Invoke(NameHeaderLabel.ActualWidth,
                PlanNameHeaderLabel.ActualWidth,
                MaterialStampHeaderLabel.ActualWidth,
                MaterialStandartHeaderLabel.ActualWidth,
                ProductStandartHeaderLabel.ActualWidth,
                OtherRuleHeaderLabel.ActualWidth);
            OnUpdateClassificationDesignation += notationPart.UpdateOtherRules;
            if (ClassificationDesignationsComboBox.SelectedIndex >= 0)
                OnUpdateClassificationDesignation.Invoke((ClassificationDesignationsComboBox.SelectedItem as ClassificationDesignation).Id);
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
    }
}