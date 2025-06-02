using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace apteka
{
    public partial class FormMedicineHistory : MaterialForm
    {
        private SqlConnection connection;
        private List<Medicine> medicines;
        private User currentUser; // Текущий пользователь

        public FormMedicineHistory()
        {
            InitializeComponent();
            connection = new SqlConnection("Server=WIN-;Database=AptekaDB;Integrated Security=True;");
            LoadMedicineHistory();
            LoadUniqueDates(); // Загружаем уникальные даты
        }

        public void LoadMedicineHistory()
        {
            DataTable dt = new DataTable();
            connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM WrittenOffMedicines", connection);
            adapter.Fill(dt);
            connection.Close();
            dataGridView1.DataSource = dt;

            // Проверяем, что данные загружены
            if (dt.Rows.Count > 0)
            {
                FormatDataGridView();
            }
            else
            {
                MessageBox.Show("Нет данных для отображения.");
            }
        }




        private void LoadUniqueDates()
        {
            using (SqlConnection connection = new SqlConnection("Server=WIN-;Database=AptekaDB;Integrated Security=True;"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT DISTINCT CAST(WriteOffDate AS DATE) AS WriteOffDate FROM WrittenOffMedicines", connection);
                SqlDataReader reader = command.ExecuteReader();
                List<DateTime> uniqueDates = new List<DateTime>();
                while (reader.Read())
                {
                    DateTime date = reader.GetDateTime(0);
                    uniqueDates.Add(date);
                }

                // Заполняем ComboBox уникальными датами
                comboBox1.DataSource = new List<DateTime>(uniqueDates); // Создаем новый список для comboBox1
                comboBox2.DataSource = new List<DateTime>(uniqueDates); // Создаем новый список для comboBox2

                // Устанавливаем значения по умолчанию
                if (uniqueDates.Count > 0)
                {
                    comboBox1.SelectedIndex = 0; // Устанавливаем первую дату как начальную
                    comboBox2.SelectedIndex = uniqueDates.Count - 1; // Устанавливаем последнюю дату как конечную
                }
            }
        }

        public DataTable GetMedicineHistoryData()
        { // Проверяем, что источник данных установлен
            if (dataGridView1.DataSource is DataTable dt)
            {
                return dt; // Возвращаем источник данных
            }
            return null; // Если источник данных не установлен, возвращаем null
        }





        private void FormatDataGridView()
        {
            // Убедитесь, что имена столбцов соответствуют именам в вашей таблице
            dataGridView1.Columns["ID"].Width = 50;
            dataGridView1.Columns["MedicineID"].Width = 100; // Измените на правильное имя
            dataGridView1.Columns["Quantity"].Width = 80; // Измените на правильное имя
            dataGridView1.Columns["WriteOffDate"].Width = 100; // Измените на правильное имя
        }


        private void label2_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрываем текущую форму
            Form1 formMain = new Form1(currentUser); // Предполагается, что у вас есть основная форма
            formMain.Show(); // Показываем основную форму
        }



        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            DataTable medicinesData = GetMedicineHistoryData(); // Получаем данные из DataGridView
            if (medicinesData == null || medicinesData.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для генерации отчета.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FormReportGenerator reportGenerator = new FormReportGenerator(medicines, medicinesData);
            reportGenerator.ShowDialog(); // Открытие формы генерации отчетов
            ClearDataGridView(); // Очищаем DataGridView после генерации отчета
        }

        private void ClearDataGridView()
        {
            dataGridView1.DataSource = null; // Очищаем источник данных
            dataGridView1.Rows.Clear(); // Очищаем строки DataGridView
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


            DateTime startDate = (DateTime)comboBox1.SelectedItem; // Получаем начальную дату
            DateTime endDate = (DateTime)comboBox2.SelectedItem; // Получаем конечную дату

            // Проверяем, что начальная дата меньше или равна конечной
            if (startDate > endDate)
            {
                MessageBox.Show("Начальная дата не может быть больше конечной даты.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Получаем данные из таблицы с учетом фильтрации по дате
            DataTable filteredData = GetWrittenOffMedicinesData(startDate, endDate);

            // Проверяем, есть ли данные для выбранного диапазона
            if (filteredData == null || filteredData.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта в выбранном диапазоне дат.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Выход из метода, если данных нет
            }

            // Вызываем метод для генерации отчета
            FormReportGenerator reportGenerator = new FormReportGenerator(medicines, filteredData);
            reportGenerator.ShowDialog(); // Открытие формы генерации отчетов

            // Обновляем DataGridView после генерации отчета
            LoadMedicineHistory(); // Обновляем данные в DataGridView
        }



        private DataTable GetWrittenOffMedicinesData(DateTime startDate, DateTime endDate)

        {

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection("Server=WIN-;Database=AptekaDB;Integrated Security=True;"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM WrittenOffMedicines WHERE WriteOffDate >= @startDate AND WriteOffDate <= @endDate", connection);
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate.AddDays(1).AddTicks(-1)); // Увеличиваем конечную дату на 1 день и убираем 1 наносекунду
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
            }
            return dt;
        }

        private void materialRaisedButton2_Click_1(object sender, EventArgs e)
        {

            DateTime startDate = (DateTime)comboBox1.SelectedItem; // Получаем начальную дату
            DateTime endDate = (DateTime)comboBox2.SelectedItem; // Получаем конечную дату

            // Проверяем, что начальная дата меньше или равна конечной
            if (startDate > endDate)
            {
                MessageBox.Show("Начальная дата не может быть больше конечной даты.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Получаем данные из таблицы с учетом фильтрации по дате
            DataTable filteredData = GetWrittenOffMedicinesData(startDate, endDate);

            // Проверяем, есть ли данные для выбранного диапазона
            if (filteredData == null || filteredData.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для отображения в выбранном диапазоне дат.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Выход из метода, если данных нет
            }

            // Обновляем DataGridView с отфильтрованными данными
            dataGridView1.DataSource = filteredData;
            FormatDataGridView(); // Форматируем DataGridView
        }
    }
}