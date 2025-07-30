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
        private MaterialsType materialType;
        private MaterialsStandard materialsStandard;
        private ProductsType productType;
        public event Action Save;
        public event Action<ProductsType, MaterialsStandard, int> HeaderUpdate;
        public event Action<int> TabDel;
        public int Id;
        internal List<ProductsStandard> ProductsStandards = new List<ProductsStandard>();
        internal List<ProductsStandard> AllProductsStandards = new List<ProductsStandard>();
        internal List<ProductsStandard> SelectedProductsStandards = new List<ProductsStandard>();
        public ProductsStandardsUserControl(MaterialsType materialsType, int id)
        {
            InitializeComponent();
            this.materialType = materialType;
            this.Id = id;
            AfterInitialize();
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
            tabItems[id].Background = new SolidColorBrush(Colors.Green);
        }
        private void AfterInitialize()
        {
            UpdateProductsStandards();
            using (AppDbContext db = new AppDbContext())
            {
                ProductTypeComboBox.ItemsSource = db.ProductsTypes.ToList();
                MaterialStandardComboBox.ItemsSource = db.MaterialsStandards.ToList();
            }
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

        

        private void MaterialStandardComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (materialsStandard == null)
            { 
                materialsStandard = MaterialStandardComboBox.SelectedItem as MaterialsStandard;
                HeaderUpdate.Invoke(productType, materialsStandard, Id);
            }
        }

        private void ProductTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productType == null)
            {
                productType = ProductTypeComboBox.SelectedItem as ProductsType;
                HeaderUpdate.Invoke(productType, materialsStandard, Id);
            }
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
            product = new ProductStandardSettingUserControl.ProductStandardSettingUserControl(materialType.Id, combinationId);
            Save += product.Save;
            tabItem.Content = product;

            tabItems.Add(tabItem);
            StandardTabControl.Items.Add(tabItem);
        }

        

        private void AddTabItem()
        {
            if (materialType != null && productType != null)
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = "n/a";
                ProductStandardSettingUserControl.ProductStandardSettingUserControl product;
                product = new ProductStandardSettingUserControl.ProductStandardSettingUserControl(materialType.Id,
                    productType.Id, productStandardSettingUserControls.Count);
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
}
