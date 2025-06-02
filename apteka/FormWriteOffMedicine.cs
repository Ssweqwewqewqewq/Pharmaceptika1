using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace apteka
{
    public partial class FormWriteOffMedicine : MaterialForm
    {
        private List<Medicine> medicines;
        private DatabaseHelper dbHelper; // Добавляем ссылку на DatabaseHelper
        private User currentUser; // Текущий пользователь
        public FormWriteOffMedicine(List<Medicine> medicines)
        {
            InitializeComponent();
            this.medicines = medicines;
            dbHelper = new DatabaseHelper(); // Инициализируем DatabaseHelper

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

            // Заполнение ComboBox с лекарствами
            comboBox1.DataSource = medicines;
            comboBox1.DisplayMember = "Name"; // Отображаем название лекарства
        }

        private void buttonWriteOff_Click(object sender, EventArgs e)
        {
           
                var selectedMedicine = (Medicine)comboBox1.SelectedItem;

                if (selectedMedicine != null)
                {
                    if (int.TryParse(textBox1.Text, out int quantityToWriteOff))
                    {
                        if (quantityToWriteOff > 0 && quantityToWriteOff <= selectedMedicine.Quantity)
                        {
                            // Уменьшаем количество лекарства
                            selectedMedicine.Quantity -= quantityToWriteOff;

                            // Записываем данные о списании в новую таблицу
                            RecordWriteOff(selectedMedicine.ID, quantityToWriteOff);

                            if (selectedMedicine.Quantity == 0)
                            {
                                dbHelper.DeleteMedicine(selectedMedicine.ID);
                                MessageBox.Show($"Лекарство '{selectedMedicine.Name}' полностью списано и удалено из базы данных.");
                            }
                            else
                            {
                                dbHelper.UpdateMedicine(selectedMedicine);
                                MessageBox.Show($"Списано {quantityToWriteOff} единиц лекарства '{selectedMedicine.Name}'.");
                            }

                            UpdateParentDataGridView();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Введите корректное количество для списания.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Введите корректное количество для списания.");
                    }
                }
                else
                {
                    MessageBox.Show("Выберите лекарство для списания.");
                }
            }

        private void RecordWriteOff(int medicineId, int quantity)
        {
            // Получаем цену лекарства
            decimal price = GetMedicinePrice(medicineId);

            using (SqlConnection connection = new SqlConnection("Server=WIN-;Database=AptekaDB;Integrated Security=True;"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO WrittenOffMedicines (MedicineID, Quantity, WriteOffDate, Price) VALUES (@medicineId, @quantity, @writeOffDate, @price)", connection);
                command.Parameters.AddWithValue("@medicineId", medicineId);
                command.Parameters.AddWithValue("@quantity", quantity);
                command.Parameters.AddWithValue("@writeOffDate", DateTime.Now);
                command.Parameters.AddWithValue("@price", price); // Добавляем цену
                command.ExecuteNonQuery();
            }
        }

        private decimal GetMedicinePrice(int medicineId)
        {
            decimal price = 0;
            using (SqlConnection connection = new SqlConnection("Server=WIN-;Database=AptekaDB;Integrated Security=True;"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Price FROM Medicines WHERE ID = @medicineId", connection);
                command.Parameters.AddWithValue("@medicineId", medicineId);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    price = Convert.ToDecimal(result);
                }
            }
            return price;
        }



        // Метод для обновления DataGridView в родительской форме
        private void UpdateParentDataGridView()
            {
                // Предполагается, что у вас есть ссылка на родительскую форму
                if (Owner is Form1 form1)
                {
                    form1.UpdateDataGridView(form1.Medicines); // Обновляем DataGridView в родительской форме
                }
            }


            private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (FormLogin.IsLoggedIn)
            {
                // Если пользователь уже вошел, перенаправляем его на форму выхода
                FormLogout formLogout = new FormLogout(); 
                formLogout.ShowDialog();
            }
            else
            {
                // Если пользователь не вошел, открываем форму логина
                FormLogin formLogin = new FormLogin();
                formLogin.ShowDialog();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрываем текущую форму
            Form1 formMain = new Form1(currentUser);
            formMain.Show(); // Показываем основную форму
        }
    }
}