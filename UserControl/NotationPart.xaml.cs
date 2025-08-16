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

        private List<OptionalRule> optionalRules;
        private Dictionary<OptionalRule,CheckBox> optionalRulesToCheckBox = new Dictionary<OptionalRule, CheckBox>();

        private int classificationId;

        public Dictionary<OptionalRule, CheckBox> mainOptionalRulesToCheckBox;

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

        public bool IsSelected
        {
            get
            { 
                return CheckBox.IsChecked == true;
            }
        }
        public Dictionary<int, bool> BindValues { get; set; } = new();

        

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
                    //добавление дополнительных операци из строки материала
                    foreach (var optionalRule in optionalRulesToCheckBox)
                    {
                        if (optionalRule.Value.IsChecked == true)
                            BindValues[optionalRule.Key.OperationTypeId] = true;
                    }
                    foreach (var optionalRule in mainOptionalRulesToCheckBox)
                    {
                        if (optionalRule.Value.IsChecked == true)
                            BindValues[optionalRule.Key.OperationTypeId] = true;
                    }

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
            if (MaterialStampComboBox.SelectedIndex >= 0)
            {
                using (var db = new AppDbContext())
                {
                    int materialId = (MaterialStampComboBox.SelectedItem as MaterialsStamp).Id;
                    materialsAndProductsCombinations = new();
                    materialsAndProductsCombinations =
                        db.MaterialsAndProductsCombinations.Where(m => m.MaterialId == materialId).ToList();
                    UpdateMaterialStandard();
                }

                UpdateOtherRules();
                BindValuesUpdate();
            }
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
                MaterialStandardComboBox.Items.Clear();
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
                ProductStandardComboBox.Items.Clear();
                foreach (var productsStandard in productsStandards)
                {
                    ProductStandardComboBox.Items.Add(productsStandard);
                }

                ProductStandardComboBox.SelectedIndex = -1;
            }
        }
        /// <summary>
        /// Обновление дополнительных правил и кода классификации
        /// </summary>
        /// <param name="classDesignationId"></param>
        public void UpdateDesignationId(int classDesignationId)
        {
            classificationId = classDesignationId;
            BindValuesUpdate();
            UpdateOtherRules();
        }
        /// <summary>
        /// Обновление дополнительных правил
        /// </summary>
        public void UpdateOtherRules()
        {
            if (MaterialStampComboBox.SelectedIndex >= 0)
            {
                using (var db = new AppDbContext())
                {
                    optionalRules = db.OptionalRules.Where(o =>
                        (o.DesignationId == classificationId || o.DesignationId == null) &&
                        o.ForAll == false &&
                        (o.MaterialTypeId == (MaterialStampComboBox.SelectedItem as MaterialsStamp).TypeId ||
                         o.MaterialTypeId == null)).ToList();
                    OptionalRulesMenu.Items.Clear();
                    foreach (var optionalRule in optionalRules)
                    {
                        if (optionalRulesToCheckBox.ContainsKey(optionalRule))
                        {
                            CheckBox checkBox = optionalRulesToCheckBox[optionalRule];
                            checkBox.Content = optionalRule.ToString();
                            checkBox.Checked += UpdateCheckBoxHeader;
                            checkBox.Unchecked += UpdateCheckBoxHeader;
                            OptionalRulesMenu.Items.Add(checkBox);
                            optionalRulesToCheckBox[optionalRule] = checkBox;
                        }
                        else
                        {
                            CheckBox checkBox = new CheckBox();
                            checkBox.Content = optionalRule.ToString();
                            checkBox.Checked += UpdateCheckBoxHeader;
                            checkBox.Unchecked += UpdateCheckBoxHeader;
                            OptionalRulesMenu.Items.Add(checkBox);
                            optionalRulesToCheckBox[optionalRule] = checkBox;
                        }

                        UpdateCheckBoxHeader();
                    }

                    List<OptionalRule> dellList = new();
                    foreach (var optionalRuleToChecBox in optionalRulesToCheckBox)
                    {
                        if (!OptionalRulesMenu.Items.Contains(optionalRuleToChecBox.Value))
                        {
                            dellList.Add(optionalRuleToChecBox.Key);
                        }
                    }

                    foreach (var optionalRule in dellList)
                    {
                        optionalRulesToCheckBox.Remove(optionalRule);
                    }
                }
            }


        }
        /// <summary>
        /// Событие выбора дополнительного правила
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCheckBoxHeader(object sender, RoutedEventArgs e)
        {
            string header = "Выбрано: ";
            foreach (CheckBox checkBox in OptionalRulesMenu.Items)
            {
                if (checkBox.IsChecked == true)
                {
                    header += checkBox.Content + ", ";
                }
            }
            OptionalRulesMenu.Header = header;
            BindValuesUpdate();
        }

        private void UpdateCheckBoxHeader()
        {
            string header = "Выбрано: ";
            foreach (CheckBox checkBox in OptionalRulesMenu.Items)
            {
                if (checkBox.IsChecked == true)
                {
                    header += checkBox.Content + ", ";
                }
            }
            OptionalRulesMenu.Header = header;
            BindValuesUpdate();
        }

        private void ProductStandardComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BindValuesUpdate();
        }
    }
}
