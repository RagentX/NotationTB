//using NotationTB.SqlTables;
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
                    ClassificationDesignationsComboBox.Items.Add(materialsStamp.Name);
                }
            }
            
        }

        public event Action<double, double, double, double, double, double> OnUpdateSize;
        public event Action<int> OnUpdateClassificationDesignation;

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
            NotationPart notationPart = new NotationPart();
            OnUpdateSize += notationPart.UpdateSize;
            OnUpdateSize?.Invoke(NameHeaderLabel.ActualWidth,
                PlanNameHeaderLabel.ActualWidth,
                MaterialStampHeaderLabel.ActualWidth,
                MaterialStandartHeaderLabel.ActualWidth,
                ProductStandartHeaderLabel.ActualWidth,
                OtherRuleHeaderLabel.ActualWidth);
            OnUpdateClassificationDesignation += notationPart.UpdateOtherRules;
            OnUpdateClassificationDesignation.Invoke(ClassificationDesignationsComboBox.SelectedIndex + 1);
            detailsStackPanel.Children.Add(notationPart);
        }

        private void ClassificationDesignationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnUpdateClassificationDesignation.Invoke(ClassificationDesignationsComboBox.SelectedIndex + 1);
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
    }
}