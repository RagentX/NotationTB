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
using Microsoft.EntityFrameworkCore.Internal;
using NotationTB.Data;
using NotationTB.Models;

namespace NotationTB.UserControl
{
    /// <summary>
    /// Логика взаимодействия для NotationPart.xaml
    /// </summary>
    public partial class NotationPart : System.Windows.Controls.UserControl
    {
        private List<MaterialsStamp> materialsStamps;
        private List<MaterialsStandard> materialsStandards;
        private List<ProductsStandard> productsStandards;
        private List<MaterialsAndProductsCombination> materialsAndProductsCombinations;

        private int classificationId;

        public string DetailName
        {
            get
            {
                return NameTextBox.Text;
            }
        }
        public string PlanName
        {
            get
            {
                return PlanNameTextBox.Text;
            }
        }

        public int MatStandardId
        {
            get
            {
                if(MaterialStandardComboBox.SelectedIndex >= 0)
                    return (MaterialStandardComboBox.SelectedItem as MaterialsStandard).Id;
                return 0;
            }
        }

       public int MaterialsStampId
        {
            get
            {
                if (MaterialStampComboBox.SelectedIndex >= 0)
                    return (MaterialStampComboBox.SelectedItem as MaterialsStamp).Id;
                return 0;
            }
        }

        public int ProductStandardId
        {
            get
            {
                if (ProductStandardComboBox.SelectedIndex >= 0)
                    return (ProductStandardComboBox.SelectedItem as ProductsStandard).Id;
                return 0;
            }
        }
        public int СlassificationId
        {
            get
            {
                return classificationId;
            }
        }
        public string MaterialStamp
        {
            get
            {
                if (MaterialStampComboBox.SelectedIndex >= 0)
                    return (MaterialStampComboBox.SelectedItem as MaterialsStamp).Name;
                return "";
            }
        }

        public string MaterialStandard
        {
            get
            {
                if (MaterialStandardComboBox.SelectedIndex >= 0)
                    return (MaterialStandardComboBox.SelectedItem as MaterialsStandard).Name;
                return "";
            }
        }

        public string ProductStandard
        {
            get
            {
                if (ProductStandardComboBox.SelectedIndex >= 0)
                    return (ProductStandardComboBox.SelectedItem as ProductsStandard).Name;
                return "";
            }
        }

        public Dictionary<int, bool> BindValues { get; set; } = new();

        

        private List<CheckBox> checkBoxes = new List<CheckBox>();
        public NotationPart()
        {
            InitializeComponent();
            UpdateMaterialStamp();
        }

        public void BindValuesUpdate()
        {
            
            if (MaterialsStampId > 0 && MatStandardId > 0 && ProductStandardId > 0 && classificationId > 0)
                using (AppDbContext db = new AppDbContext())
                {
                    BindValues = new Dictionary<int, bool>();
                    var materialStamp = db.MaterialsStamps.Where(c => c.Id == MaterialsStampId).First();
                    var productStandart = db.ProductsStandards.Where(c => c.Id == ProductStandardId).First();
                    var combination = db.MaterialsAndProductsCombinations.Where(c =>
                        c.MatStandardId == MatStandardId &&
                        c.MaterialId == MaterialsStampId &&
                        c.ProStandardId == ProductStandardId).First();
                    var baseRules = db.BasesRulesOperations.Where(b =>
                        b.ProductTypeId == productStandart.TypeId &&
                        b.MaterialTypeId == materialStamp.TypeId &&
                        b.DesignationId == classificationId).ToList();
                    foreach (var baseRule in baseRules) BindValues[baseRule.OperationTypeId] = baseRule.Value;

                    var exceptionRules = db.ExceptionRulesOperations.Where(e =>
                        e.CombinationId == combination.Id).ToList();
                    foreach (var exceptionRule in exceptionRules)
                        BindValues[exceptionRule.OperationTypeId] = exceptionRule.Value;
                }
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
            using (var db = new AppDbContext())
            {
                materialsStamps = db.MaterialsStamps.ToList();
                foreach (var materialsStamp in materialsStamps)
                {
                    MaterialStampComboBox.Items.Add(materialsStamp);
                }
            }
        }
        private void MaterialStampComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                int materialId = (MaterialStampComboBox.SelectedItem as MaterialsStamp).Id;
                materialsAndProductsCombinations =
                    db.MaterialsAndProductsCombinations.Where(m => m.MaterialId == materialId).ToList();
                UpdateMaterialStandard();
            }

            BindValuesUpdate();
        }
        /// <summary>
        /// Обновление стандартов материала
        /// </summary>
        public void UpdateMaterialStandard()
        {
            using (var db = new AppDbContext())
            {
                materialsStandards = new List<MaterialsStandard>();
                foreach (var materialsAndProductsCombination in materialsAndProductsCombinations)
                {
                    materialsStandards.Add(db.MaterialsStandards.Where(m => m.Id == materialsAndProductsCombination.MatStandardId).First());
                }

                materialsStandards = materialsStandards.Distinct().ToList();
                foreach (var materialsStandard in materialsStandards)
                {
                    MaterialStandardComboBox.Items.Add(materialsStandard);
                }

                MaterialStandardComboBox.SelectedIndex = -1;
            }
        }
        private void MaterialStandardComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MaterialStandardComboBox.SelectedIndex >= 0)
                using (var db = new AppDbContext())
                {
                    int materialStandardId = (MaterialStandardComboBox.SelectedItem as MaterialsStandard).Id;
                    materialsAndProductsCombinations =
                        materialsAndProductsCombinations.Where(m => m.MatStandardId == materialStandardId).ToList();
                    UpdateProductStandard();
                }

            BindValuesUpdate();
        }
        /// <summary>
        /// Обновление стандартов изделий и полуфабрикатов
        /// </summary>
        public void UpdateProductStandard()
        {
            using (var db = new AppDbContext())
            {
                productsStandards = new List<ProductsStandard>();
                foreach (var materialsAndProductsCombination in materialsAndProductsCombinations)
                {
                    productsStandards.Add(db.ProductsStandards.Where(p => p.Id == materialsAndProductsCombination.ProStandardId).First());
                }
                productsStandards = productsStandards.Distinct().ToList();
                foreach (var productsStandard in productsStandards)
                {
                    ProductStandardComboBox.Items.Add(productsStandard);
                }

                ProductStandardComboBox.SelectedIndex = -1;
            }
        }

        public void UpdateOtherRules(int classDesignationId)
        {
            classificationId = classDesignationId;
            BindValuesUpdate();
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

        private void ProductStandardComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BindValuesUpdate();
        }
    }
}
