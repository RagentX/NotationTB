//using NotationTB.SqlTables;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace NotationTB.UserControl
{
    /// <summary>
    /// Логика взаимодействия для NotationPart.xaml
    /// </summary>
    public partial class NotationPart : System.Windows.Controls.UserControl
    {
        //private DbSet<MaterialsStamp> materialsStamps;
        //private DbSet<MaterialsStandard> materialsStandards;
        //private DbSet<ProductsStandard> productsStandards;
        //private IQueryable<OtherRule> otherRules;
        private List<CheckBox> checkBoxes = new List<CheckBox>();
        public NotationPart()
        {
            InitializeComponent();
            UpdateMaterialStamp();
            UpdateMaterialStandart();
            UpdateProductStandart();
        }

        public void UpdateSize(double size1, double size2, double size3, double size4, double size5, double size6)
        {
            DetailGrid.ColumnDefinitions[0].Width = new GridLength(size1, GridUnitType.Pixel);
            DetailGrid.ColumnDefinitions[2].Width = new GridLength(size2, GridUnitType.Pixel);
            DetailGrid.ColumnDefinitions[4].Width = new GridLength(size3, GridUnitType.Pixel);
            DetailGrid.ColumnDefinitions[6].Width = new GridLength(size4, GridUnitType.Pixel);
            DetailGrid.ColumnDefinitions[8].Width = new GridLength(size5, GridUnitType.Pixel);
            DetailGrid.ColumnDefinitions[10].Width = new GridLength(size6, GridUnitType.Pixel);

        }
        /// <summary>
        /// Обновление марок материала
        /// </summary>
        public void UpdateMaterialStamp()
        {
            //using (var db = new NotationTbContext())
            //{
            //    materialsStamps = db.MaterialsStamps;
            //    foreach (var materialsStamp in materialsStamps)
            //    {
            //        MaterialStampComboBox.Items.Add(materialsStamp.Name);
            //    }
            //}
        }
        /// <summary>
        /// Обновление стандартов материала
        /// </summary>
        public void UpdateMaterialStandart()
        {
            //using (var db = new NotationTbContext())
            //{
            //    materialsStandards = db.MaterialsStandards;
            //    foreach (var materialsStandard in materialsStandards)
            //    {
            //        MaterialStandartComboBox.Items.Add(materialsStandard.Name);
            //    }
            //}
        }

        /// <summary>
        /// Обновление стандартов изделий и полуфабрикатов
        /// </summary>
        public void UpdateProductStandart()
        {
            //using (var db = new NotationTbContext())
            //{
            //    productsStandards = db.ProductsStandards;
            //    foreach (var productsStandard in productsStandards)
            //    {
            //        ProductStandartComboBox.Items.Add(productsStandard.Name);
            //    }
            //}
        }
        public void UpdateOtherRules(int classDesignationId)
        {
            //test.Items.Clear();
            
            //using (var db = new NotationTbContext())
            //{
            //    otherRules = db.OtherRules.Where(o => o.DesignationId == classDesignationId && o.ForAll == false);

            //    foreach (var otherRule in otherRules)
            //    {
            //        CheckBox checkBox = new CheckBox();
            //        checkBox.Content = otherRule.Name == null ? "" : otherRule.Name;
            //        checkBox.Checked += UpdateCheckBoxHeader;
            //        checkBox.Unchecked += UpdateCheckBoxHeader;
            //        checkBoxes.Add(checkBox);
            //        test.Items.Add(checkBox);
            //    }
            //}
            //using (var db = new NotationTbContext())
            //{
            //    test.Items.Add(new CheckBox());
            //}
        }

        private void UpdateCheckBoxHeader(object sender, RoutedEventArgs e)
        {
            string header = "Выбрано: ";
            foreach (CheckBox checkBox in checkBoxes)
            {
                if (checkBox.IsChecked == true)
                {
                    header += checkBox.Content + ", ";
                }
            }
            test.Header = header;
        }
    }
}
