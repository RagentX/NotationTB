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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NotationTB.BusinessLogic;
using NotationTB.BusinessLogic.Object;
using NotationTB.Data;
using NotationTB.Models;
using NotationTB.UserControl.ProductStandardSettingUserControl;

namespace NotationTB.UserControl
{
    /// <summary>
    /// Логика взаимодействия для ProductsStandardsUserControl.xaml
    /// </summary>
    public partial class ProductsStandardsUserControl : System.Windows.Controls.UserControl
    {
        private List<TabItem> tabItems = new List<TabItem>();

        private List<ProductStandardSettingUserControl.ProductStandardSettingUserControl>
            productStandardSettingUserControls =
                new List<ProductStandardSettingUserControl.ProductStandardSettingUserControl>();
        private int materialTypeId = -1, productTypeId = -1, combinationId = -1;
        public event Action Save;
        internal List<ProductsStandard> ProductsStandards = new List<ProductsStandard>();
        internal List<ProductsStandard> AllProductsStandards = new List<ProductsStandard>();
        internal List<ProductsStandard> SelectedProductsStandards = new List<ProductsStandard>();
        public ProductsStandardsUserControl(int materialTypeId, int productTypeId)
        {
            InitializeComponent();
            this.materialTypeId = materialTypeId;
            this.productTypeId = productTypeId;
        }

        public void UpdateSelectedProductsStandard(ProductsStandard lastProductsStandard, ProductsStandard newProductsStandard, int id)
        {
            if (lastProductsStandard != null)
            {
                SelectedProductsStandards.Remove(lastProductsStandard);
            }
            SelectedProductsStandards.Add(newProductsStandard);
            UpdateProductsStandards();
            tabItems[id].Header = newProductsStandard.ToString();
        }

        public void SaveAll()
        {
            Save.Invoke();
        }

        private void UpdateProductsStandards()
        {
            using (AppDbContext db = new AppDbContext())
            {
                AllProductsStandards = db.ProductsStandards.ToList();
            }
            ProductsStandards = AllProductsStandards.Except(SelectedProductsStandards).ToList();
        }
        private void AddStandardButton_Click(object sender, RoutedEventArgs e)
        {
            AddTabItem();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateProductsStandards();
        }



        /// <summary>
        /// Добавление новой вкладки с описанием стандарта полуфабриката
        /// </summary>
        /// <param name="id">код стандарта полуфабриката</param>
        private void AddTabItem(int combinationId)
        {
            TabItem tabItem = new TabItem();

            tabItem.Header = "n/a";
            ProductStandardSettingUserControl.ProductStandardSettingUserControl product;
            product = new ProductStandardSettingUserControl.ProductStandardSettingUserControl(materialTypeId, combinationId);
            Save += product.Save;
            tabItem.Content = product;

            tabItems.Add(tabItem);
            StandardTabControl.Items.Add(tabItem);
        }
        private void AddTabItem()
        {
            TabItem tabItem = new TabItem();
            tabItem.Header = "n/a";
            ProductStandardSettingUserControl.ProductStandardSettingUserControl product;
            product = new ProductStandardSettingUserControl.ProductStandardSettingUserControl(materialTypeId, productTypeId, productStandardSettingUserControls.Count);
            Save += product.Save;
            product.StandardChange += UpdateSelectedProductsStandard;
            product.ProductStandardComboBox.ItemsSource = ProductsStandards;

            productStandardSettingUserControls.Add(product);

            tabItem.Content = product;
            tabItems.Add(tabItem);
            StandardTabControl.Items.Add(tabItem);
        }
    }
}
