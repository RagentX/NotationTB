using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using NotationTB.BusinessLogic;
using NotationTB.BusinessLogic.Object;
using NotationTB.Data;

namespace NotationTB
{
    /// <summary>
    /// Логика взаимодействия для AddMatCombinations.xaml
    /// </summary>
    public partial class AddMatCombinations : Window
    {
        public ObservableCollection<ImportRow> Rows { get; } = new();
        public string Status { get; set; } = "Готово";

        private readonly IRowValidator _validator;
        private readonly IPgRepository _repo;
        private readonly Uploader _uploader;
        public AppDbContext cs;
        public AddMatCombinations()
        {
            InitializeComponent();
            DataContext = this;

            // TODO: поместите строку подключения в настройки/секреты
            cs = new AppDbContext();
            _validator = new RowValidator();
            _repo = new PgRepository(cs);
            _uploader = new Uploader(_validator, _repo, cs);
        }

        private void OpenExcel_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Excel (*.xlsx)|*.xlsx" };
            if (dlg.ShowDialog() != true) return;

            Rows.Clear();
            foreach (var r in ExcelReader.Read(dlg.FileName))
                Rows.Add(r);

            Status = $"Загружено строк: {Rows.Count}";
            OnPropertyChanged(nameof(Status));
        }

        private async void Validate_Click(object sender, RoutedEventArgs e)
        {
            if (Rows.Count == 0) return;
            Status = "Проверка…";
            OnPropertyChanged(nameof(Status));

            await _uploader.ValidateAsync(Rows, CancellationToken.None);

            // Подсветка статусов в колонке Error
            foreach (var r in Rows)
            {
                if (!r.IsValid)
                    r.Error = string.IsNullOrEmpty(r.Error) ? "Ошибка" : r.Error;
                else if (r.IsDuplicate)
                    r.Error = "Дубль — будет пропущено";
                else
                    r.Error = "OK";
            }
            Status = "Проверка завершена";
            OnPropertyChanged(nameof(Status));
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Rows.Count == 0) return;
            Status = "Сохранение…";
            OnPropertyChanged(nameof(Status));

            var count = await _uploader.SaveAsync(Rows, CancellationToken.None);
            Status = $"Сохранено строк: {count}. Дубликатов: {Rows.Count(r => r.IsDuplicate)}";
            OnPropertyChanged(nameof(Status));
        }

        // INotifyPropertyChanged (либо замените на MVVM Toolkit)
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
}
