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
using NotationTB.UserControl;

namespace NotationTB
{
    /// <summary>
    /// Логика взаимодействия для AddNewMatWindow.xaml
    /// </summary>
    public partial class MaterialWindow : Window
    {
        private List<TabItem> tabItems = new List<TabItem>();
        private List<TextBlock> tabItemsHeaderTextBlocks = new List<TextBlock>
        {
            new TextBlock {Text = "Листы"},
            new TextBlock {Text = "Поковки"},
            new TextBlock {Text = "Крепёжные изделия"},
            new TextBlock {Text = "Сортовой прокат"},
            new TextBlock {Text = "Отливки"},
        };
        public MaterialWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 5; i++)
            {
                ProductsStandardsUserControl productsStandardsUserControl = new ProductsStandardsUserControl(i);
                //тут бинды на события
                TabItem tabItem = new TabItem
                {
                    Header = tabItemsHeaderTextBlocks[i],
                    Content = productsStandardsUserControl
                };
                tabItems.Add(tabItem);
                ProductsStandardsTabControl.Items.Add(tabItem);

            }
        }
    }
}
