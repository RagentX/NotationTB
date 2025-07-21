using NotationTB.UserControl.ProductStandardSettingUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NotationTB.Data;
using Microsoft.EntityFrameworkCore;

namespace NotationTB.UserControl.ProductStandardsSettingUserControl
{
    class OperationTableViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;

        public ObservableCollection<OperationColumn> OperationColumns { get; set; } = new();
        public ObservableCollection<OperationViewModel> Rows { get; set; } = new();

        public OperationTableViewModel()
        {
            _context = new AppDbContext(); // или передавай через DI
            LoadOperations();
            LoadSampleRows(); // Временно заглушка
        }

        private void LoadOperations()
        {
            var operations = _context.OperationsTypes
                .AsNoTracking()
                .OrderBy(o => o.Id)
                .ToList();

            OperationColumns = new ObservableCollection<OperationColumn>(
                operations.Select(o => new OperationColumn
                {
                    Number = o.Id,
                    FullName = o.Name,
                    IsVisible = true
                }));

            foreach (var col in OperationColumns)
            {
                col.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(OperationColumn.IsVisible))
                        OnPropertyChanged(nameof(OperationColumns));
                };
            }

            OnPropertyChanged(nameof(OperationColumns));
        }

        // Примерные данные
        private void LoadSampleRows()
        {
            Rows = new ObservableCollection<OperationViewModel>
        {
            new OperationViewModel
            {
                Classification = "Класс A",
                OperationValues = new Dictionary<int, string>
                {
                    { 1, "✓" },
                    { 2, "✗" }
                }
            },
            new OperationViewModel
            {
                Classification = "Класс B",
                OperationValues = new Dictionary<int, string>
                {
                    { 1, "✗" },
                    { 2, "✓" }
                }
            }
        };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}