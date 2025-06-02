using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace apteka
{
    public partial class FormReceiveMedicine : MaterialForm
    {
        private User currentUser; // Текущий пользователь
        private List<Medicine> medicines;
        private DatabaseHelper dbHelper; // Добавляем ссылку на DatabaseHelper
        private Panel titlePanel; // Панель для заголовка
        public FormReceiveMedicine(List<Medicine> medicines)
        {
            InitializeComponent();
            InitializeCustomComponents();
            this.medicines = medicines;
            dbHelper = new DatabaseHelper(); // Инициализируем DatabaseHelper
            LoadComboBoxData();
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
        }

        private void InitializeCustomComponents()
        {
            // Создаем панель для заголовка
            titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.Blue // Синий цвет заголовка
            };


        }
        private void LoadComboBoxData()
        {
            // Заполнение ComboBox с названиями лекарств
            comboBoxName.Items.Clear(); // Очистите ComboBox перед добавлением
            comboBoxName.Items.AddRange(dbHelper.LoadMedicines().Select(m => m.Name).ToArray());
            comboBoxName.SelectedIndex = -1; // Устанавливаем значение по умолчанию (ничего не выбрано)

            // Заполнение ComboBox с симптомами
            comboBoxSymptoms.Items.Clear();
            comboBoxSymptoms.Items.AddRange(dbHelper.LoadSymptoms().ToArray());
            comboBoxSymptoms.SelectedIndex = -1; // Устанавливаем значение по умолчанию (ничего не выбрано)

            // Заполнение ComboBox с активными ингредиентами
            comboBoxActiveIngredient.Items.Clear();
            comboBoxActiveIngredient.Items.AddRange(dbHelper.LoadActiveIngredients().ToArray());
            comboBoxActiveIngredient.SelectedIndex = -1; // Устанавливаем значение по умолчанию (ничего не выбрано)

            // Заполнение ComboBox с противопоказаниями
            comboBoxContraindications.Items.Clear();
            comboBoxContraindications.Items.AddRange(dbHelper.LoadContraindications().ToArray());
            comboBoxContraindications.SelectedIndex = -1; // Устанавливаем значение по умолчанию (ничего не выбрано)

            // Заполнение ComboBox с побочными эффектами
            comboBoxSideEffects.Items.Clear();
            comboBoxSideEffects.Items.AddRange(dbHelper.LoadSideEffects().ToArray());
            comboBoxSideEffects.SelectedIndex = -1; // Устанавливаем значение по умолчанию (ничего не выбрано)
        }


        private void buttonAdd_Click(object sender, EventArgs e)
        {

                // Получаем данные из текстовых полей
                string name = textBox1.Text.Trim(); // Убедитесь, что название не пустое
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Введите название лекарства.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(textBox2.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введите корректное количество.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DateTime expirationDate = dateTimePicker1.Value;

                // Устанавливаем значения по умолчанию для полей
                string symptoms = comboBoxSymptoms.SelectedItem?.ToString() ?? string.Empty;
                string activeIngredient = comboBoxActiveIngredient.SelectedItem?.ToString() ?? string.Empty;
                string contraindications = comboBoxContraindications.SelectedItem?.ToString() ?? string.Empty;
                string sideEffects = comboBoxSideEffects.SelectedItem?.ToString() ?? string.Empty;

                bool isPrescription = checkBoxIsPrescription.Checked; // Получаем статус рецепта из чекбокса

                // Обработка цены
                float price;
                if (!float.TryParse(textBoxPrice.Text, out price))
                {
                    MessageBox.Show("Введите корректную цену.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Проверяем, существует ли препарат
                var existingMedicine = medicines.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (existingMedicine != null)
                {
                    existingMedicine.Quantity += quantity;
                    existingMedicine.ExpirationDate = expirationDate;
                    dbHelper.UpdateMedicine(existingMedicine);
                    MessageBox.Show("Запасы препарата обновлены!");
                }
                else
                {
                    // Создаем новый объект Medicine
                    Medicine newMedicine = new Medicine(0, name, quantity, expirationDate, symptoms, isPrescription, activeIngredient, contraindications, sideEffects, price);
                    dbHelper.AddMedicine(newMedicine);
                    MessageBox.Show("Лекарство добавлено!");
                }

                UpdateParentDataGridView();
                this.Close();
            }


            // Метод для обновления DataGridView в родительской форме
            private void UpdateParentDataGridView()
        {
            // Предполагается, что у вас есть ссылка на родительскую форму
            if (Owner is Form1 form1)
            {
                form1.LoadMedicines(); // Обновляем список лекарств в родительской форме
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрываем текущую форму
            Form1 formMain = new Form1(currentUser);
            formMain.Show(); // Показываем основную форму
        }

        private void FormReceiveMedicine_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
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

        private void comboBoxName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Получаем выбранное название лекарства
            if (comboBoxName.SelectedItem != null)
            {
                string selectedMedicineName = comboBoxName.SelectedItem.ToString();
                Medicine selectedMedicine = dbHelper.GetMedicineByName(selectedMedicineName);
                if (selectedMedicine != null)
                {
                    // Заполняем поля данными из базы
                    textBox1.Text = selectedMedicine.Name; // Название
                    textBox2.Text = selectedMedicine.Quantity.ToString(); // Количество
                    dateTimePicker1.Value = selectedMedicine.ExpirationDate; // Срок годности
                    comboBoxSymptoms.SelectedItem = selectedMedicine.Symptoms; // Симптомы
                    checkBoxIsPrescription.Checked = selectedMedicine.IsPrescription; // Рецепт
                    comboBoxActiveIngredient.SelectedItem = selectedMedicine.ActiveIngredient; // Действующее вещество
                    comboBoxContraindications.SelectedItem = selectedMedicine.Contraindications; // Противопоказания
                    comboBoxSideEffects.SelectedItem = selectedMedicine.SideEffects; // Побочные эффекты
                    textBoxPrice.Text = selectedMedicine.Price.ToString(); // Цена
                }
            }
        }
    }
}