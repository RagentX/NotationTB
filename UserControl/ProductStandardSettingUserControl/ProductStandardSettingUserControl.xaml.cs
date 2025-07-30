using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

namespace NotationTB.UserControl.ProductStandardSettingUserControl
{
    /// <summary>
    /// Логика взаимодействия для ProductStandardSettingUserControl.xaml
    /// </summary>
    public partial class ProductStandardSettingUserControl : System.Windows.Controls.UserControl
    {
        public int Id { get; private set; }
        public List<OperationsType> AllOperations { get; private set; }
        public List<int> SelectedOperationIds { get; private set; } = new();
        public List<OperationRow> Rows { get; set; }

        private int productTypeID;

        

        private NotationRule notationRule;

        private ProductsStandard productsType;

        public event Action<ProductsStandard, ProductsStandard, int> StandardChange;
        

        public ProductStandardSettingUserControl(int materialTypeId, int productTypeId, int id)
        {
            InitializeComponent();
            this.notationRule = new NotationRule(materialTypeId, productTypeId);
            Id = id;
            AfterInitialize();
        }
        public ProductStandardSettingUserControl(int combinationId, int id)
        {
            InitializeComponent();
            this.notationRule = new NotationRule(combinationId);
            Id = id;
            AfterInitialize();
        }
        private void AfterInitialize()
        {
            LoadOperations();
            LoadRows();
            BuildOperationList();
            BuildDataGrid();
        }
        public void Save()
        {
            notationRule.Save();
        }

        

        private void LoadOperations()
        {
            using (AppDbContext db = new AppDbContext())
            {
                AllOperations = db.OperationsTypes.AsNoTracking().OrderBy(o => o.Id).ToList();
            }
            
            //тут включение операций для отображения
            SelectedOperationIds.Clear();
            foreach ( var value in notationRule.OperationRows[0].Values)
            {
                SelectedOperationIds.Add(value.Key);
            }
            
        }

        private void LoadRows()
        {
            Rows = notationRule.OperationRows;
        }

        private void BuildOperationList()
        {
            List<OperationListItem> items = new List<OperationListItem>();

            foreach (var op in AllOperations)
            {
                items.Add(new OperationListItem
                {
                    Id = op.Id,
                    Display = op.Id.ToString() + " " + op.Name,
                    IsSelected = SelectedOperationIds.Contains(op.Id)
                });
            }

            OperationListBox.ItemsSource = items;

            // подписка
            foreach (OperationListItem item in OperationListBox.Items)
            {
                item.PropertyChanged += (s, e) => OnOperationSelectionChanged();
            }
        }

        private void OnOperationSelectionChanged()
        {
            SelectedOperationIds = OperationListBox.Items
                .OfType<OperationListItem>()
                .Where(i => i.IsSelected)
                .Select(i => i.Id)
                .ToList();

            BuildDataGrid();
        }

        private void BuildDataGrid()
        {
            OperationsGrid.Columns.Clear();

            // Классификация
            OperationsGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Классификация",
                Binding = new Binding("Classification")
            });

            foreach (var opId in SelectedOperationIds)
            {
                var op = AllOperations.FirstOrDefault(o => o.Id == opId);
                if (op == null) continue;

                OperationsGrid.Columns.Add(new DataGridCheckBoxColumn()
                {
                    Header = new TextBlock
                    {
                        Text = op.Id.ToString(),
                        ToolTip = op.Name
                    },
                    Binding = new Binding($"Values[{opId}]")
                });
            }

            OperationsGrid.ItemsSource = Rows;
        }

        private void AddStandardButton_Click(object sender, RoutedEventArgs e)
        {
            Rows[0].Values[1] = true;
        }

        private void ProductStandardComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newProductsStandard = ProductStandardComboBox.SelectedItem as ProductsStandard;
            StandardChange.Invoke(productsType, newProductsStandard, Id);
            productsType = newProductsStandard;
        }
    }

    public class OperationListItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Display { get; set; }
        public string Tooltip { get; set; }

        private bool _isSelected = true;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}