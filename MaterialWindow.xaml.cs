using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NotationTB.Data;
using NotationTB.Models;
using NotationTB.UserControl;

namespace NotationTB
{
    /// <summary>
    /// Логика взаимодействия для AddNewMatWindow.xaml
    /// </summary>
    public partial class MaterialWindow : Window
    {
        private List<TabItem> tabItems = new List<TabItem>();

        private MaterialsType materialsType;

        public event Action SaveAll;

        public MaterialWindow(int materialId)
        {
            //вот тут жопа тут нужно уже существующие загружать
            //InitializeComponent();
            //for (int i = 0; i < 5; i++)
            //{
            //    //ProductsStandardsUserControl productsStandardsUserControl = new ProductsStandardsUserControl(i);
            //    //тут бинды на события
            //    TabItem tabItem = new TabItem
            //    {
            //        Header = tabItemsHeaderTextBlocks[i],
            //        Content = productsStandardsUserControl
            //    };
            //    tabItems.Add(tabItem);
            //    ProductsStandardsTabControl.Items.Add(tabItem);

            //}
        }
        public MaterialWindow()
        {
            InitializeComponent();
            AfterInitialize();
        }
        private void AfterInitialize()
        {
            using (var db = new AppDbContext())
            {
                MaterialTypeComboBox.ItemsSource = db.MaterialsTypes.ToArray();
            }

        }
        public void HeaderUpdate(ProductsType productsType, MaterialsStandard materialsStandard, int id)
        {
            if (productsType == null && materialsStandard == null)
            {
                tabItems[id].Header = "n/a";
                tabItems[id].Background = new SolidColorBrush(Colors.Red);
                return;
            }

            if (productsType == null)
            {
                tabItems[id].Header = $"n/a,{materialsStandard.Name}";
                tabItems[id].Background = new SolidColorBrush(Colors.Red);
                return;
            }

            if (materialsStandard == null)
            {
                tabItems[id].Header = $"{productsType.Name}, n/a";
                tabItems[id].Background = new SolidColorBrush(Colors.Red);
                return;
            }
            tabItems[id].Background = new SolidColorBrush(Colors.Green);
            tabItems[id].Header = $"{productsType.Name},{materialsStandard.Name}";
        }


        


        private void MaterialTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (materialsType == null)
            {
                tabItems.Clear();
                ProductsStandardsTabControl.Items.Clear();
                //for (int i = 0; i < 5; i++)
                //{
                //    materialsType = MaterialTypeComboBox.SelectedItem as MaterialsType;
                //    ProductsStandardsUserControl productsStandardsUserControl = new ProductsStandardsUserControl(materialsType.Id, i + 2);
                //    //тут бинды на события
                //    SaveAll += productsStandardsUserControl.SaveAll;
                //    TabItem tabItem = new TabItem
                //    {
                //        Header = tabItemsHeaderTextBlocks[i],
                //        Content = productsStandardsUserControl
                //    };
                //    tabItems.Add(tabItem);
                //    ProductsStandardsTabControl.Items.Add(tabItem);

                //}
            }
            else
            {
                MaterialTypeComboBox.SelectedItem = materialsType;
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddStandardButton_Click(object sender, RoutedEventArgs e)
        {
            materialsType = MaterialTypeComboBox.SelectedItem as MaterialsType;
            ProductsStandardsUserControl productsStandardsUserControl = new ProductsStandardsUserControl(materialsType, tabItems.Count);
            //тут бинды на события
            SaveAll += productsStandardsUserControl.SaveAll;
            productsStandardsUserControl.HeaderUpdate += HeaderUpdate;
            TabItem tabItem = new TabItem
            {
                Header = "n/a",
                Content = productsStandardsUserControl
            };
            tabItems.Add(tabItem);
            ProductsStandardsTabControl.Items.Add(tabItem);
        }
    }
}
