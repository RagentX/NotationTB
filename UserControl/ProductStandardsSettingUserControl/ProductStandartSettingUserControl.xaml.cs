using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using NotationTB.Data;
using NotationTB.Models;
using NotationTB.UserControl.ProductStandardSettingUserControl;
using NotationTB.UserControl.ProductStandardsSettingUserControl;

namespace NotationTB.UserControl
{
    /// <summary>
    /// Логика взаимодействия для ProductStandartSettingUserControl.xaml
    /// </summary>
    public partial class ProductStandartSettingUserControl : System.Windows.Controls.UserControl
    {
        private AppDbContext _context = new AppDbContext();

        public List<OperationsType> AllOperations { get; private set; }
        public List<int> SelectedOperationIds { get; private set; } = new();
        public List<OperationRow> Rows { get; private set; }

        public ProductStandartSettingUserControl()
        {
            InitializeComponent();
            Loaded += OperationTableControl_Loaded;
        }

        private void OperationTableControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadOperations();
            LoadRows();
            BuildCheckBoxPanel();
            BuildDataGrid();
        }

        private void LoadOperations()
        {
            AllOperations = _context.OperationsTypes.AsNoTracking().OrderBy(o => o.Id).ToList();
            SelectedOperationIds = AllOperations.Select(o => o.Id).ToList(); // по умолчанию все
        }

        private void LoadRows()
        {
            // Пример. Тут можешь подгружать реальные данные из других таблиц
            Rows = new List<OperationRow>
        {
            new OperationRow { Classification = "A1", Values = new Dictionary<int, string> { {1, "✓"}, {2, "✗"} } },
            new OperationRow { Classification = "B2", Values = new Dictionary<int, string> { {1, "✗"}, {2, "✓"} } }
        };
        }

        private void BuildCheckBoxPanel()
        {
            OperationCheckBoxPanel.Children.Clear();

            foreach (var op in AllOperations)
            {
                var cb = new CheckBox
                {
                    Content = op.Id.ToString(),
                    ToolTip = op.Name,
                    IsChecked = true,
                    Margin = new Thickness(4, 0, 0, 0)
                };

                cb.Checked += (s, e) =>
                {
                    if (!SelectedOperationIds.Contains(op.Id))
                    {
                        SelectedOperationIds.Add(op.Id);
                        BuildDataGrid();
                    }
                };

                cb.Unchecked += (s, e) =>
                {
                    if (SelectedOperationIds.Contains(op.Id))
                    {
                        SelectedOperationIds.Remove(op.Id);
                        BuildDataGrid();
                    }
                };

                OperationCheckBoxPanel.Children.Add(cb);
            }
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

                OperationsGrid.Columns.Add(new DataGridTextColumn
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
    }
    public class OperationRow
    {
        public string Classification { get; set; }
        public Dictionary<int, string> Values { get; set; } = new();
    }
}