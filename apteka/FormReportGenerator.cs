using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace apteka
{
    public partial class FormReportGenerator : MaterialForm
    {
        private DataTable medicinesData; // Изменено на DataTable
        private User currentUser; // Текущий пользователь
        private FormMedicineHistory historyForm; // Поле для хранения ссылки на форму истории
        private List<Medicine> medicines;
        private SqlConnection connection;

        public FormReportGenerator(List<Medicine> medicines, DataTable medicinesData)
        {
            InitializeComponent();
            this.medicines = medicines;
            this.medicinesData = medicinesData; // Сохраняем данные о лекарствах
            this.historyForm = new FormMedicineHistory();

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

            connection = new SqlConnection("Server=WIN-;Database=AptekaDB;Integrated Security=True;");
            LoadUniqueDates(); // Загружаем уникальные даты

        }

        private void buttonGenerateReport_Click(object sender, EventArgs e)
        {

            ExportToCSV(medicinesData); // Передаем отфильтрованные данные

            // Удаляем данные из базы данных после генерации отчета
            if (medicinesData.Rows.Count > 0)
            {
                // Проверяем, существует ли столбец "WriteOffDate"
                if (medicinesData.Columns.Contains("WriteOffDate"))
                {
                    // Используем TryParse для безопасного преобразования
                    DateTime startDate;
                    DateTime endDate;

                    // Пробуем преобразовать строки в DateTime
                    if (DateTime.TryParse(medicinesData.Rows[0]["WriteOffDate"].ToString(), out startDate) &&
                        DateTime.TryParse(medicinesData.Rows[medicinesData.Rows.Count - 1]["WriteOffDate"].ToString(), out endDate))
                    {
                        // Удаляем данные из базы данных
                        ClearWrittenOffMedicines(startDate, endDate);
                        ClearMedicineHistoryData(startDate, endDate); // Удаляем записи из истории
                        MessageBox.Show("Записи успешно удалены из базы данных.");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось преобразовать даты. Убедитесь, что формат даты правильный.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Столбец 'WriteOffDate' не найден в данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Нет данных для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Обновляем DataGridView в родительской форме
            if (historyForm != null)
            {
                historyForm.LoadMedicineHistory(); // Обновляем данные в DataGridView
            }
        }



        private void ClearWrittenOffMedicines(DateTime startDate, DateTime endDate)
        {
          
                using (SqlConnection connection = new SqlConnection("Server=WIN-;Database=AptekaDB;Integrated Security=True;"))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM WrittenOffMedicines WHERE WriteOffDate >= @startDate AND WriteOffDate <= @endDate", connection);
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate.AddDays(1).AddTicks(-1)); // Увеличиваем конечную дату на 1 день и убираем 1 наносекунду
                    command.ExecuteNonQuery();
                }
            }


            private void ExportToCSV(DataTable medicinesData)
        {
            if (medicinesData == null || medicinesData.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Выход из метода, если данных нет
            }
            string fileName = $"WrittenOffMedicinesReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("ID,MedicineID,Quantity,WriteOffDate,Price"); // Заголовок
                decimal totalPrice = 0;
                foreach (DataRow row in medicinesData.Rows)
                {
                    int quantity = row.Field<int>("Quantity");
                    decimal price = row.Field<decimal>("Price");
                    totalPrice += quantity * price; // Рассчитываем общую цену
                    sw.WriteLine($"{row["ID"]},{row["MedicineID"]},{quantity},{row["WriteOffDate"]},{price}");
                }
                sw.WriteLine($"Total Price:,,{totalPrice}");
            }
            MessageBox.Show("Отчет успешно экспортирован!");
            OpenCsvFile(fileName);
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
            }
        }


        private void OpenCsvFile(string filePath)
        {
            try
            {
                // Запускаем процесс для открытия CSV-файла
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true // Используем оболочку для открытия файла
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть файл: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearMedicineHistoryData(DateTime startDate, DateTime endDate)
        {

            using (SqlConnection connection = new SqlConnection("Server=WIN-;Database=AptekaDB;Integrated Security=True;"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("DELETE FROM WrittenOffMedicines WHERE WriteOffDate >= @startDate AND WriteOffDate < @endDate", connection);
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate.AddDays(1)); // Увеличиваем конечную дату на 1 день
                command.ExecuteNonQuery();
            }
        }


            private void buttonViewHistory_Click(object sender, EventArgs e)
        {
            FormMedicineHistory formHistory = new FormMedicineHistory();
            formHistory.ShowDialog();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрываем текущую форму
            Form1 formMain = new Form1(currentUser);
            formMain.Show(); // Показываем основную форму
        }
    }
}