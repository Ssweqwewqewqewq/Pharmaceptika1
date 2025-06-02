using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace apteka
{
    public partial class Form1 : MaterialForm
    {

        private User currentUser; // Текущий пользователь

        private List<Medicine> medicines;
        private List<Medicine> filteredMedicines; // Список для хранения отфильтрованных лекарств
        private Panel titlePanel; // Панель для заголовка
        private Label titleLabel; // Лейбл для заголовка

        public Form1(User user)
        {
            currentUser = user; // Сохраняем текущего пользователя
                                // Остальная инициализация
            InitializeComponent();
            InitializeCustomComponents();

            // Инициализация MaterialSkinManager
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue400,
                Primary.Blue500,
                Primary.Blue500,
                Accent.LightBlue200,
                TextShade.WHITE
            );

            medicines = new List<Medicine>();
            LoadMedicines();
        }

        public List<Medicine> Medicines
        {
            get { return medicines; }
        }

        private void InitializeCustomComponents()
        {
            titlePanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 650,
                BackColor = Color.Green // Фон формы
            };

            // Добавляем панель заголовка на форму
            this.Controls.Add(titlePanel);

            // Создаем ComboBox для фильтрации по назначению

            //{
            //    Dock = DockStyle.Top,
            //    DropDownStyle = ComboBoxStyle.DropDownList,
            //    Width = 150, // Устанавливаем фиксированную ширину
            //    Height = 30 // Устанавливаем фиксированную высоту
            //};

            // Добавляем значения в ComboBox
            comboBoxSymptoms.Items.Add("Все");
            comboBoxSymptoms.Items.Add("Антигистаминное средство(Против Аллергии)");
            comboBoxSymptoms.Items.Add("Обезболивающее");
            comboBoxSymptoms.Items.Add("Противовоспалительное");
            comboBoxSymptoms.Items.Add("Жаропонижающее");

            comboBoxSymptoms.SelectedIndex = 0; // Устанавливаем "Все" по умолчанию

            // Добавляем обработчик события для фильтрации
            comboBoxSymptoms.SelectedIndexChanged += (s, e) =>
            {
                FilterMedicines(comboBoxSymptoms.SelectedItem.ToString());
            };

            // Добавляем ComboBox на форму
            this.Controls.Add(comboBoxSymptoms);
        }

        public void LoadMedicines()
        {
          
                DatabaseHelper dbHelper = new DatabaseHelper();
                medicines = dbHelper.LoadMedicines(); // Загружаем лекарства из базы данных
                UpdateDataGridView(medicines); // Обновляем DataGridView

        }


        public void UpdateDataGridView(List<Medicine> medicines)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("ID");
            dataTable.Columns.Add("Название");
            dataTable.Columns.Add("Количество");
            dataTable.Columns.Add("Срок годности");
            dataTable.Columns.Add("Назначение препарата");
            dataTable.Columns.Add("Рецептурный");
            dataTable.Columns.Add("Действующее вещество");
            dataTable.Columns.Add("Противопоказания");
            dataTable.Columns.Add("Побочные эффекты");
            dataTable.Columns.Add("Цена");
            foreach (var medicine in medicines)
            {
                string expirationStatus = medicine.ExpirationDate < DateTime.Now ? "Истек срок годности" : "";
                dataTable.Rows.Add(
                    medicine.ID,
                    medicine.Name,
                    medicine.Quantity,
                    medicine.ExpirationDate.ToShortDateString() + (expirationStatus != "" ? $" ({expirationStatus})" : ""),
                    medicine.Symptoms,
                    medicine.IsPrescription ? "Да" : "Нет",
                    medicine.ActiveIngredient,
                    medicine.Contraindications,
                    medicine.SideEffects,
                    medicine.Price.ToString("F2")
                );
            }

            dataGridView2.DataSource = dataTable;

            // Установка цвета текста в DataGridView
            dataGridView2.DefaultCellStyle.ForeColor = Color.White; // Цвет текста
            dataGridView2.DefaultCellStyle.BackColor = Color.Green; // Цвет фона ячеек
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; // Цвет текста заголовков
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue; // Цвет фона заголовков

            // Настройка шрифта
            dataGridView2.DefaultCellStyle.Font = new Font("Arial", 7); // Установка компактного шрифта

            // Настройка ширины столбцов
            dataGridView2.RowTemplate.Height = 40; // Увеличение высоты строки
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // Автоматическая подгонка высоты строк
            dataGridView2.DefaultCellStyle.WrapMode = DataGridViewTriState.True; // Включить перенос текста

            dataGridView2.Columns["ID"].Width = 40; // Узкая ширина для ID
            dataGridView2.Columns["Название"].Width = 150; // Ширина для названия
            dataGridView2.Columns["Количество"].Width = 70; // Узкая ширина для Количества
            dataGridView2.Columns["Срок годности"].Width = 70; // Широкая ширина для срока годности
            dataGridView2.Columns["Назначение препарата"].Width = 200;
            dataGridView2.Columns["Рецептурный"].Width = 80; // Широкая ширина для статуса
            dataGridView2.Columns["Действующее вещество"].Width = 120; // Ширина для действующего вещества
            dataGridView2.Columns["Противопоказания"].Width = 120; // Ширина для противопоказаний
            dataGridView2.Columns["Побочные эффекты"].Width = 120; // Ширина для побочных эффектов
            dataGridView2.Columns["Цена"].Width = 70; // Ширина для цены
        }
        private void FilterMedicines(string symptom)
        {
            if (symptom == "Все")
            {
                UpdateDataGridView(medicines); // Показываем все лекарства
            }
            else
            {
                filteredMedicines = medicines
                    .Where(m => m != null && m.Symptoms != null && m.Symptoms.ToLower().Contains(symptom.ToLower())) // Проверяем на null
                    .ToList();
                UpdateDataGridView(filteredMedicines); // Обновляем DataGridView с отфильтрованными данными
            }
        }



        private void buttonReceive_Click(object sender, EventArgs e)
        {
            FormReceiveMedicine formReceive = new FormReceiveMedicine(medicines);
            formReceive.ShowDialog();
            UpdateDataGridView(medicines);
        }

        private void buttonWriteOff_Click(object sender, EventArgs e)
        {
            FormWriteOffMedicine formWriteOff = new FormWriteOffMedicine(medicines);
            formWriteOff.ShowDialog();
            UpdateDataGridView(medicines);
        }




        private void pictureBoxLogin_Click(object sender, EventArgs e)
        {
            if (FormLogin.IsLoggedIn)
            {
                // Если пользователь уже вошел, перенаправляем его на форму выхода
                FormLogout formLogout = new FormLogout(); // Создайте эту форму
                formLogout.ShowDialog();
            }
            else
            {
                // Если пользователь не вошел, открываем форму логина
                FormLogin formLogin = new FormLogin();
                formLogin.ShowDialog();
            }
        }
        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            FormReceiveMedicine formReceive = new FormReceiveMedicine(medicines);
            formReceive.ShowDialog();
            UpdateDataGridView(medicines);
        }
        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрываем текущую форму
            Form1 formMain = new Form1(currentUser); // Предполагается, что у вас есть основная форма
            formMain.Show(); // Показываем основную форму
        }
        private void buttonManageUsers_Click(object sender, EventArgs e)
        {
            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для доступа к управлению пользователями.");
                return; // Exit the method if the user is not an admin
            }

            FormManageUsers formManageUsers = new FormManageUsers(currentUser); // Передаем текущего пользователя
            formManageUsers.ShowDialog(); // Открываем форму управления пользователями
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрываем текущую форму
            Form1 formMain = new Form1(currentUser); // Предполагается, что у вас есть основная форма
            formMain.Show(); // Показываем основную форму
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
                string searchTerm = textBox1.Text.ToLower(); // Получаем текст из текстового поля
                filteredMedicines = medicines
                    .Where(m => m != null && // Проверяем, что объект не равен null
                                (m.Name != null && m.Name.ToLower().Contains(searchTerm) || // Проверяем на null
                                 (m.Symptoms != null && m.Symptoms.ToLower().Contains(searchTerm))) // Проверяем на null
                    )
                    .ToList();
                UpdateDataGridView(filteredMedicines); // Обновляем DataGridView с отфильтрованными данными
            }

        

        private void textBox1_KeyDown_1(object sender, KeyEventArgs e)
        {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchTerm = textBox1.Text.ToLower();
                    filteredMedicines = medicines
                        .Where(m => m != null &&
                                    (m.Name != null && m.Name.ToLower().Contains(searchTerm) ||
                                     (m.Symptoms != null && m.Symptoms.ToLower().Contains(searchTerm)) ||
                                     (m.ActiveIngredient != null && m.ActiveIngredient.ToLower().Contains(searchTerm))))
                        .ToList();
                    UpdateDataGridView(filteredMedicines);
                    e.SuppressKeyPress = true;
                }
            }
            private void materialLabel1_Click(object sender, EventArgs e)
        {

        }

        private DataTable GetMedicinesDataTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("ID");
            dataTable.Columns.Add("Название");
            dataTable.Columns.Add("Количество");
            dataTable.Columns.Add("Срок годности");
            dataTable.Columns.Add("Назначение препарата");
            dataTable.Columns.Add("Рецептурный");
            dataTable.Columns.Add("Действующее вещество");
            dataTable.Columns.Add("Противопоказания");
            dataTable.Columns.Add("Побочные эффекты");
            dataTable.Columns.Add("Цена");
            foreach (var medicine in medicines)
            {
                dataTable.Rows.Add(
                    medicine.ID,
                    medicine.Name,
                    medicine.Quantity,
                    medicine.ExpirationDate.ToShortDateString(),
                    medicine.Symptoms,
                    medicine.IsPrescription ? "Да" : "Нет",
                    medicine.ActiveIngredient,
                    medicine.Contraindications,
                    medicine.SideEffects,
                    medicine.Price.ToString("F2")
                );
            }
            return dataTable;
        }
        private void materialRaisedButton7_Click(object sender, EventArgs e)
        {
            DataTable medicinesData = GetMedicinesDataTable(); // Получаем данные из DataGridView
            if (medicinesData == null || medicinesData.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для генерации отчета.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FormReportGenerator reportGenerator = new FormReportGenerator(medicines, medicinesData);
            reportGenerator.ShowDialog(); // Открытие формы генерации отчетов

        }

        private void materialRaisedButton8_Click(object sender, EventArgs e)
        {
            FormMedicineHistory historyForm = new FormMedicineHistory();
            historyForm.ShowDialog(); // Открытие формы истории изменений
        }

    }
    public class Medicine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Symptoms { get; set; }
        public bool IsPrescription { get; set; }
        public string ActiveIngredient { get; set; }
        public string Contraindications { get; set; }
        public string SideEffects { get; set; }
        public float Price { get; set; } // Изменено на float

        public Medicine(int id, string name, int quantity, DateTime expirationDate, string symptoms, bool isPrescription, string activeIngredient, string contraindications, string sideEffects, float price) // Изменено на float
        {
            ID = id;
            Name = name;
            Quantity = quantity;
            ExpirationDate = expirationDate;
            Symptoms = symptoms;
            IsPrescription = isPrescription;
            ActiveIngredient = activeIngredient;
            Contraindications = contraindications;
            SideEffects = sideEffects;
            Price = price; // Инициализация цены
        }
    }
}
