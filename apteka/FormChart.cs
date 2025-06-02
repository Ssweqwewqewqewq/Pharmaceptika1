
using LiveCharts;
using LiveCharts.WinForms; // Используем только WinForms
using LiveCharts.Defaults;
using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;


namespace apteka
{
    public partial class FormChart : Form
    {
     

        public FormChart(DataTable data)
        {
            InitializeComponent();
            InitializeChart(); // Инициализируем график
            CreateChart(data); // Создаем график
        }

        private void InitializeChart()
        {
         
        }

        private void CreateChart(DataTable data)
        {
            // Создаем коллекцию для данных
            var values = new ChartValues<ObservableValue>();
            var labels = new List<string>(); // Список для меток оси X

            foreach (DataRow row in data.Rows)
            {
                // Предполагается, что у вас есть столбец "Quantity" и "MedicineName" в DataTable
                if (row["Quantity"] != DBNull.Value && row["MedicineName"] != DBNull.Value) // Проверка на NULL
                {
                    values.Add(new ObservableValue(Convert.ToDouble(row["Quantity"])));
                    labels.Add(row["MedicineName"].ToString()); // Добавляем название лекарства в метки
                }
            }

            
        }
    }
}
