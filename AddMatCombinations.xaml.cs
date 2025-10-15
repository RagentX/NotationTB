using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using NotationTB.BusinessLogic;
using NotationTB.BusinessLogic.Object;
using NotationTB.Data;
using System.ComponentModel;

namespace NotationTB
{
    /// <summary>
    /// Логика взаимодействия для AddMatCombinations.xaml
    /// </summary>
    public partial class AddMatCombinations : Window, INotifyPropertyChanged
    {
        public ObservableCollection<ImportRow> Rows { get; } = new();
        private string _status = "Готово";
        public string Status
        {
            get => _status;
            set
            {
                if (_status == value) return;
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

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
        }

        private async void Validate_Click(object sender, RoutedEventArgs e)
        {
            if (Rows.Count == 0) return;
            Status = "Проверка…";

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

        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Rows.Count == 0) return;
            Status = "Сохранение…";

            var count = await _uploader.SaveAsync(Rows, CancellationToken.None);
            Status = $"Сохранено строк: {count}. Дубликатов: {Rows.Count(r => r.IsDuplicate)}";
        }

    }
}
