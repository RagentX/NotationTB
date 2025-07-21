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

namespace NotationTB.UserControl
{
    /// <summary>
    /// Логика взаимодействия для ProductsStandardsUserControl.xaml
    /// </summary>
    public partial class ProductsStandardsUserControl : System.Windows.Controls.UserControl
    {
        private List<TabItem> tabItems = new List<TabItem>();
        private int standardType;
        public ProductsStandardsUserControl(int standardType)
        {
            InitializeComponent();
            this.standardType = standardType;
        }

        private void AddStandardButton_Click(object sender, RoutedEventArgs e)
        {
            AddTabItem(0);
        }
        /// <summary>
        /// Добавление новой вкладки с описанием стандарта полуфабриката
        /// </summary>
        /// <param name="id">код стандарта полуфабриката</param>
        private void AddTabItem(int id)
        {
            TabItem tabItem = new TabItem();
            if (id == 0)
            {
                tabItem.Header = "n/a";
                tabItem.Content = new ProductStandartSettingUserControl();
            }
            else
            {
                //вывод названия из БД
            }
            tabItems.Add(tabItem);
            StandardTabControl.Items.Add(tabItem);
        }
    }
}
