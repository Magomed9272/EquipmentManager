using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EquipmentAccountingSystem
{
    // ======================== БИБЛИОТЕКА КЛАССОВ ========================

    // Модель оборудования
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; }           // Название
        public string Model { get; set; }          // Модель
        public string SerialNumber { get; set; }   // Серийный номер
        public string Type { get; set; }           // Тип (компьютер, принтер, сканер и т.д.)
        public string Location { get; set; }       // Местоположение
        public string Status { get; set; }         // Статус (в работе, в ремонте, списано)
        public DateTime PurchaseDate { get; set; } // Дата покупки
        public decimal Price { get; set; }         // Цена
        public string ResponsiblePerson { get; set; } // Ответственное лицо
        public string Notes { get; set; }          // Примечания

        public Equipment()
        {
            PurchaseDate = DateTime.Now;
            Status = "В работе";
        }
    }

    // Класс для работы с данными (хранение)
    public class DataStorage
    {
        private string _filePath = "equipment.txt";

        // Загрузка данных из файла
        public List<Equipment> Load()
        {
            if (!File.Exists(_filePath))
                return new List<Equipment>();

            var list = new List<Equipment>();
            var lines = File.ReadAllLines(_filePath);

            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 11)
                {
                    list.Add(new Equipment
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Model = parts[2],
                        SerialNumber = parts[3],
                        Type = parts[4],
                        Location = parts[5],
                        Status = parts[6],
                        PurchaseDate = DateTime.Parse(parts[7]),
                        Price = decimal.Parse(parts[8]),
                        ResponsiblePerson = parts[9],
                        Notes = parts[10]
                    });
                }
            }
            return list;
        }

        // Сохранение данных в файл
        public void Save(List<Equipment> equipmentList)
        {
            var lines = equipmentList.Select(e =>
                $"{e.Id}|{e.Name}|{e.Model}|{e.SerialNumber}|{e.Type}|{e.Location}|{e.Status}|{e.PurchaseDate}|{e.Price}|{e.ResponsiblePerson}|{e.Notes}");

            File.WriteAllLines(_filePath, lines);
        }
    }

    // Бизнес-логика (сервис для работы с оборудованием)
    public class EquipmentService
    {
        private DataStorage _storage = new DataStorage();
        private List<Equipment> _equipmentList;

        public EquipmentService()
        {
            _equipmentList = _storage.Load();
        }

        public List<Equipment> GetAll() => _equipmentList.ToList();

        public List<Equipment> GetByType(string type) => _equipmentList.Where(e => e.Type == type).ToList();

        public List<Equipment> GetByStatus(string status) => _equipmentList.Where(e => e.Status == status).ToList();

        public List<Equipment> GetByLocation(string location) => _equipmentList.Where(e => e.Location == location).ToList();

        public Equipment GetById(int id) => _equipmentList.FirstOrDefault(e => e.Id == id);

        public void Add(Equipment equipment)
        {
            equipment.Id = _equipmentList.Count > 0 ? _equipmentList.Max(e => e.Id) + 1 : 1;
            _equipmentList.Add(equipment);
            _storage.Save(_equipmentList);
        }

        public void Update(Equipment equipment)
        {
            var existing = GetById(equipment.Id);
            if (existing != null)
            {
                existing.Name = equipment.Name;
                existing.Model = equipment.Model;
                existing.SerialNumber = equipment.SerialNumber;
                existing.Type = equipment.Type;
                existing.Location = equipment.Location;
                existing.Status = equipment.Status;
                existing.PurchaseDate = equipment.PurchaseDate;
                existing.Price = equipment.Price;
                existing.ResponsiblePerson = equipment.ResponsiblePerson;
                existing.Notes = equipment.Notes;
                _storage.Save(_equipmentList);
            }
        }

        public void Delete(int id)
        {
            var equipment = GetById(id);
            if (equipment != null)
            {
                _equipmentList.Remove(equipment);
                _storage.Save(_equipmentList);
            }
        }

        public List<string> GetTypes() => _equipmentList.Select(e => e.Type).Distinct().ToList();
        public List<string> GetStatuses() => _equipmentList.Select(e => e.Status).Distinct().ToList();
        public List<string> GetLocations() => _equipmentList.Select(e => e.Location).Distinct().ToList();
    }

    // ======================== ПОЛЬЗОВАТЕЛЬСКИЙ ИНТЕРФЕЙС ========================

    // Форма для добавления/редактирования оборудования
    public class EquipmentForm : Form
    {
        public Equipment EquipmentResult;

        private TextBox txtName, txtModel, txtSerial, txtType, txtLocation, txtResponsible, txtNotes;
        private ComboBox cboStatus;
        private DateTimePicker dtpPurchaseDate;
        private NumericUpDown numPrice;
        private Button btnSave, btnCancel;

        public EquipmentForm(Equipment equipment = null)
        {
            EquipmentResult = equipment ?? new Equipment();
            InitializeForm();
            LoadData();
        }

        private void InitializeForm()
        {
            this.Text = EquipmentResult.Id == 0 ? "Добавление оборудования" : "Редактирование оборудования";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 20;
            int spacing = 35;

            // Название
            Controls.Add(new Label { Text = "Название:", Location = new Point(20, y), Size = new Size(120, 25) });
            txtName = new TextBox { Location = new Point(150, y), Size = new Size(300, 25) };
            Controls.Add(txtName);
            y += spacing;

            // Модель
            Controls.Add(new Label { Text = "Модель:", Location = new Point(20, y), Size = new Size(120, 25) });
            txtModel = new TextBox { Location = new Point(150, y), Size = new Size(300, 25) };
            Controls.Add(txtModel);
            y += spacing;

            // Серийный номер
            Controls.Add(new Label { Text = "Серийный номер:", Location = new Point(20, y), Size = new Size(120, 25) });
            txtSerial = new TextBox { Location = new Point(150, y), Size = new Size(300, 25) };
            Controls.Add(txtSerial);
            y += spacing;

            // Тип
            Controls.Add(new Label { Text = "Тип:", Location = new Point(20, y), Size = new Size(120, 25) });
            txtType = new TextBox { Location = new Point(150, y), Size = new Size(300, 25) };
            Controls.Add(txtType);
            y += spacing;

            // Местоположение
            Controls.Add(new Label { Text = "Местоположение:", Location = new Point(20, y), Size = new Size(120, 25) });
            txtLocation = new TextBox { Location = new Point(150, y), Size = new Size(300, 25) };
            Controls.Add(txtLocation);
            y += spacing;

            // Статус
            Controls.Add(new Label { Text = "Статус:", Location = new Point(20, y), Size = new Size(120, 25) });
            cboStatus = new ComboBox { Location = new Point(150, y), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboStatus.Items.AddRange(new[] { "В работе", "В ремонте", "Списано", "Резерв" });
            cboStatus.SelectedIndex = 0;
            Controls.Add(cboStatus);
            y += spacing;

            // Дата покупки
            Controls.Add(new Label { Text = "Дата покупки:", Location = new Point(20, y), Size = new Size(120, 25) });
            dtpPurchaseDate = new DateTimePicker { Location = new Point(150, y), Size = new Size(150, 25) };
            Controls.Add(dtpPurchaseDate);
            y += spacing;

            // Цена
            Controls.Add(new Label { Text = "Цена:", Location = new Point(20, y), Size = new Size(120, 25) });
            numPrice = new NumericUpDown { Location = new Point(150, y), Size = new Size(150, 25), Maximum = 9999999, DecimalPlaces = 2, ThousandsSeparator = true };
            Controls.Add(numPrice);
            y += spacing;

            // Ответственный
            Controls.Add(new Label { Text = "Ответственный:", Location = new Point(20, y), Size = new Size(120, 25) });
            txtResponsible = new TextBox { Location = new Point(150, y), Size = new Size(300, 25) };
            Controls.Add(txtResponsible);
            y += spacing;

            // Примечания
            Controls.Add(new Label { Text = "Примечания:", Location = new Point(20, y), Size = new Size(120, 25) });
            txtNotes = new TextBox { Location = new Point(150, y), Size = new Size(300, 60), Multiline = true };
            Controls.Add(txtNotes);
            y += 70;

            // Кнопки
            btnSave = new Button { Text = "Сохранить", Location = new Point(150, y + 20), Size = new Size(100, 35), BackColor = Color.LightGreen };
            btnSave.Click += BtnSave_Click;
            Controls.Add(btnSave);

            btnCancel = new Button { Text = "Отмена", Location = new Point(270, y + 20), Size = new Size(100, 35), BackColor = Color.LightGray };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancel);
        }

        private void LoadData()
        {
            if (EquipmentResult.Id != 0)
            {
                txtName.Text = EquipmentResult.Name;
                txtModel.Text = EquipmentResult.Model;
                txtSerial.Text = EquipmentResult.SerialNumber;
                txtType.Text = EquipmentResult.Type;
                txtLocation.Text = EquipmentResult.Location;
                cboStatus.SelectedItem = EquipmentResult.Status;
                dtpPurchaseDate.Value = EquipmentResult.PurchaseDate;
                numPrice.Value = EquipmentResult.Price;
                txtResponsible.Text = EquipmentResult.ResponsiblePerson;
                txtNotes.Text = EquipmentResult.Notes;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название оборудования!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            EquipmentResult.Name = txtName.Text;
            EquipmentResult.Model = txtModel.Text;
            EquipmentResult.SerialNumber = txtSerial.Text;
            EquipmentResult.Type = txtType.Text;
            EquipmentResult.Location = txtLocation.Text;
            EquipmentResult.Status = cboStatus.SelectedItem.ToString();
            EquipmentResult.PurchaseDate = dtpPurchaseDate.Value;
            EquipmentResult.Price = numPrice.Value;
            EquipmentResult.ResponsiblePerson = txtResponsible.Text;
            EquipmentResult.Notes = txtNotes.Text;

            DialogResult = DialogResult.OK;
            Close();
        }
    }

    // Главная форма
    public class MainForm : Form
    {
        private EquipmentService _service;
        private DataGridView grid;
        private ComboBox cboFilterType, cboFilterStatus, cboFilterLocation;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;
        private Label lblStats;
        private TabControl tabControl;

        public MainForm()
        {
            _service = new EquipmentService();
            InitializeForm();
            RefreshGrid();
            UpdateStats();
        }

        private void InitializeForm()
        {
            this.Text = "Информационная система учёта оборудования";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Панель фильтров
            int yFilter = 12;

            Controls.Add(new Label { Text = "Тип:", Location = new Point(12, yFilter), Size = new Size(40, 25) });
            cboFilterType = new ComboBox { Location = new Point(55, yFilter), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboFilterType.Items.Add("Все");
            cboFilterType.SelectedIndex = 0;
            cboFilterType.SelectedIndexChanged += (s, e) => RefreshGrid();
            Controls.Add(cboFilterType);

            Controls.Add(new Label { Text = "Статус:", Location = new Point(220, yFilter), Size = new Size(50, 25) });
            cboFilterStatus = new ComboBox { Location = new Point(275, yFilter), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboFilterStatus.Items.Add("Все");
            cboFilterStatus.SelectedIndex = 0;
            cboFilterStatus.SelectedIndexChanged += (s, e) => RefreshGrid();
            Controls.Add(cboFilterStatus);

            Controls.Add(new Label { Text = "Местоположение:", Location = new Point(410, yFilter), Size = new Size(100, 25) });
            cboFilterLocation = new ComboBox { Location = new Point(515, yFilter), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboFilterLocation.Items.Add("Все");
            cboFilterLocation.SelectedIndex = 0;
            cboFilterLocation.SelectedIndexChanged += (s, e) => RefreshGrid();
            Controls.Add(cboFilterLocation);

            Controls.Add(new Label { Text = "Поиск:", Location = new Point(680, yFilter), Size = new Size(50, 25) });
            txtSearch = new TextBox { Location = new Point(735, yFilter), Size = new Size(200, 25) };
            txtSearch.TextChanged += (s, e) => RefreshGrid();
            Controls.Add(txtSearch);

            btnRefresh = new Button { Text = "Обновить", Location = new Point(950, yFilter), Size = new Size(100, 25), BackColor = Color.LightGray };
            btnRefresh.Click += (s, e) => { UpdateFilters(); RefreshGrid(); };
            Controls.Add(btnRefresh);

            // Таблица оборудования
            grid = new DataGridView
            {
                Location = new Point(12, 50),
                Size = new Size(1160, 500),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            Controls.Add(grid);

            // Кнопки действий
            btnAdd = new Button { Text = "➕ Добавить", Location = new Point(12, 565), Size = new Size(120, 40), BackColor = Color.LightGreen };
            btnAdd.Click += (s, e) => AddEquipment();
            Controls.Add(btnAdd);

            btnEdit = new Button { Text = "✏ Редактировать", Location = new Point(142, 565), Size = new Size(120, 40), BackColor = Color.LightBlue };
            btnEdit.Click += (s, e) => EditEquipment();
            Controls.Add(btnEdit);

            btnDelete = new Button { Text = "🗑 Удалить", Location = new Point(272, 565), Size = new Size(120, 40), BackColor = Color.LightCoral };
            btnDelete.Click += (s, e) => DeleteEquipment();
            Controls.Add(btnDelete);

            // Статистика
            lblStats = new Label { Location = new Point(12, 620), Size = new Size(800, 30), Font = new Font("Arial", 10, FontStyle.Bold) };
            Controls.Add(lblStats);
        }

        private void UpdateFilters()
        {
            var types = _service.GetTypes();
            cboFilterType.Items.Clear();
            cboFilterType.Items.Add("Все");
            cboFilterType.Items.AddRange(types.ToArray());
            cboFilterType.SelectedIndex = 0;

            var statuses = _service.GetStatuses();
            cboFilterStatus.Items.Clear();
            cboFilterStatus.Items.Add("Все");
            cboFilterStatus.Items.AddRange(statuses.ToArray());
            cboFilterStatus.SelectedIndex = 0;

            var locations = _service.GetLocations();
            cboFilterLocation.Items.Clear();
            cboFilterLocation.Items.Add("Все");
            cboFilterLocation.Items.AddRange(locations.ToArray());
            cboFilterLocation.SelectedIndex = 0;
        }

        private void RefreshGrid()
        {
            var list = _service.GetAll();

            // Фильтр по типу
            if (cboFilterType.SelectedIndex > 0)
                list = list.Where(e => e.Type == cboFilterType.SelectedItem.ToString()).ToList();

            // Фильтр по статусу
            if (cboFilterStatus.SelectedIndex > 0)
                list = list.Where(e => e.Status == cboFilterStatus.SelectedItem.ToString()).ToList();

            // Фильтр по местоположению
            if (cboFilterLocation.SelectedIndex > 0)
                list = list.Where(e => e.Location == cboFilterLocation.SelectedItem.ToString()).ToList();

            // Поиск
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string search = txtSearch.Text.ToLower();
                list = list.Where(e => e.Name.ToLower().Contains(search) ||
                                       e.Model.ToLower().Contains(search) ||
                                       e.SerialNumber.ToLower().Contains(search) ||
                                       e.ResponsiblePerson.ToLower().Contains(search)).ToList();
            }

            grid.DataSource = list.Select(e => new
            {
                e.Id,
                Название = e.Name,
                Модель = e.Model,
                Серийный_номер = e.SerialNumber,
                Тип = e.Type,
                Местоположение = e.Location,
                Статус = e.Status,
                Дата_покупки = e.PurchaseDate.ToShortDateString(),
                Цена = $"{e.Price:N2} ₽",
                Ответственный = e.ResponsiblePerson,
                Примечания = e.Notes
            }).ToList();

            // Раскраска строк по статусу
            foreach (DataGridViewRow row in grid.Rows)
            {
                string status = row.Cells["Статус"].Value.ToString();
                if (status == "Списано")
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                else if (status == "В ремонте")
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                else if (status == "Резерв")
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
            }

            UpdateStats();
        }

        private void UpdateStats()
        {
            var all = _service.GetAll();
            int total = all.Count;
            int inWork = all.Count(e => e.Status == "В работе");
            int inRepair = all.Count(e => e.Status == "В ремонте");
            int writtenOff = all.Count(e => e.Status == "Списано");
            decimal totalPrice = all.Sum(e => e.Price);

            lblStats.Text = $"📊 Статистика: Всего единиц: {total} | В работе: {inWork} | В ремонте: {inRepair} | Списано: {writtenOff} | Общая стоимость: {totalPrice:N2} ₽";
        }

        private void AddEquipment()
        {
            var form = new EquipmentForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                _service.Add(form.EquipmentResult);
                UpdateFilters();
                RefreshGrid();
            }
        }

        private void EditEquipment()
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите оборудование для редактирования!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            var equipment = _service.GetById(id);
            var form = new EquipmentForm(equipment);

            if (form.ShowDialog() == DialogResult.OK)
            {
                _service.Update(form.EquipmentResult);
                UpdateFilters();
                RefreshGrid();
            }
        }

        private void DeleteEquipment()
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите оборудование для удаления!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Удалить выбранное оборудование?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = (int)grid.SelectedRows[0].Cells["Id"].Value;
                _service.Delete(id);
                UpdateFilters();
                RefreshGrid();
            }
        }
    }

    // ======================== ТОЧКА ВХОДА ========================
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}