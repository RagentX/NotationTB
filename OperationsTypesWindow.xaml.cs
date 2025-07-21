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
using Microsoft.EntityFrameworkCore;
using NotationTB.Data;
using NotationTB.Models;

//using NotationTB.SqlTables;

namespace NotationTB
{
    /// <summary>
    /// Логика взаимодействия для OperationsTypesWindow.xaml
    /// </summary>
    public partial class OperationsTypesWindow : Window
    {
        private AppDbContext db;
        public OperationsTypesWindow()
        {
            InitializeComponent();
            Update();

        }

        private void updateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            //db = new NotationTbContext();
            //db.OperationsTypes.Load();
            //operationsTypesGrid.ItemsSource = db.OperationsTypes.Local.ToBindingList();
        }
        private void saveMenuItem_Click(object sender, RoutedEventArgs e)  
        {
            List<string> keyList = new List<string>();
            for (int i = 0; i < operationsTypesGrid.Items.Count; i++)
            {
                //OperationsType operationsType = operationsTypesGrid.Items[i] as OperationsType;
                //if (operationsType != null)
                //{
                //    keyList.Add(operationsType.Id);
                //}
            }
            for (int i = 0; i < operationsTypesGrid.Items.Count; i++)
            {
                OperationsType operationsType = operationsTypesGrid.Items[i] as OperationsType;
                if (operationsType != null)
                {
                    //string key = operationsType.Id;
                    //if (keyList.Where(l => l == key).Count() > 1 || key == null || key == "")
                    //{
                    //    MessageBox.Show("В таблице есть строчки с одинаковыми или пустыми ключами.");
                    //    return;
                    //}
                }
            }
            db.SaveChanges();
            Update();
        }

        private void addMenuItem_Click(object sender, RoutedEventArgs e)
        {
            db.SaveChanges();
            Update();
        }

        private void delMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (operationsTypesGrid.SelectedItems.Count > 0)
            {
                List<OperationsType> operationsTypesDellList = new List<OperationsType>();
                for (int i = 0; i < operationsTypesGrid.SelectedItems.Count; i++)
                {
                    OperationsType operationsType = operationsTypesGrid.SelectedItems[i] as OperationsType;
                    if (operationsType != null)
                    {
                        operationsTypesDellList.Add(operationsType);
                    }
                }

                foreach (OperationsType operationsType in operationsTypesDellList)
                {
                    db.OperationsTypes.Remove(operationsType);
                }
            }
            db.SaveChanges();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }
    }
}
