using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin.Controls;
using MaterialSkin;

namespace apteka
{
    public partial class FormMedicineDetails : MaterialForm
    {
        DataTable dt = new DataTable();
        SqlConnection connection;
        private User currentUser; // Текущий пользователь
        public FormMedicineDetails()
        {
            InitializeComponent();
            // Инициализация соединения с базой данных
            connection = new SqlConnection("Server=WIN-RE8GDN7E046;Database=AptekaDB;Integrated Security=True;");

            // Инициализация MaterialSkinManager
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT; // Установите тему
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue400, // Основной цвет
                Primary.Blue500, // Цвет акцента
                Primary.Blue500, // Цвет фона
                Accent.LightBlue200, // Цвет акцента
                TextShade.WHITE // Цвет текста
            );

            LoadMedications(); // Загрузка медикаментов при инициализации формы
        }

        private void LoadMedications()
        {
            dt.Clear();
            connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM [Medicines]", connection);
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            connection.Close();
            FormatDataGridView();
        }

        private void FormatDataGridView()
        {
            dataGridView1.Columns["Name"].Width = 150;
            dataGridView1.Columns["ActiveIngredient"].Width = 150;
            dataGridView1.Columns["Contraindications"].Width = 200;
            dataGridView1.Columns["Symptoms"].Width = 300;
            dataGridView1.Columns["Quantity"].Width = 100;
            dataGridView1.Columns["ExpirationDate"].Width = 100;
            dataGridView1.Columns["IsPrescription"].Width = 100;
            dataGridView1.Columns["Price"].Width = 100;

            // Установка цвета текста и фона в DataGridView
            dataGridView1.DefaultCellStyle.ForeColor = System.Drawing.Color.White; // Цвет текста
            dataGridView1.DefaultCellStyle.BackColor = System.Drawing.Color.Green; // Цвет фона ячеек
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White; // Цвет текста заголовков
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Blue; // Цвет фона заголовков
        }

       
        private void label2_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрываем текущую форму
            Form1 formMain = new Form1(currentUser); // Предполагается, что у вас есть основная форма
            formMain.Show(); // Показываем основную форму
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string filter = textBox1.Text.ToLower();
            DataView dv = new DataView(dt);
            dv.RowFilter = string.Format("Name LIKE '%{0}%' OR ActiveIngredient LIKE '%{0}%' OR Contraindications LIKE '%{0}%' OR SideEffects LIKE '%{0}%'", filter);
            dataGridView1.DataSource = dv;
        }
    }
}