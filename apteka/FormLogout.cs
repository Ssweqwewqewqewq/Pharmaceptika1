using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace apteka
{
    public partial class FormLogout : MaterialForm
    {
        private Label labelUsername; // Лейбл для отображения имени пользователя
        private User currentUser; // Текущий пользователь
        public FormLogout()
        {
            InitializeComponent();

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

            // Инициализация лейбла для имени пользователя
            labelUsername = new Label
            {
                Text = $"Вы вошли как: {Properties.Settings.Default.Username}", // Отображаем имя пользователя
                ForeColor = Color.Black,
                Location = new Point(350, 150), // Установите нужные координаты
                AutoSize = true // Автоматическая подстройка размера
            };
            this.Controls.Add(labelUsername);
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            // Запрос подтверждения выхода
            var result = MessageBox.Show("Вы точно хотите выйти?", "Подтверждение выхода", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Сбрасываем состояние входа
                FormLogin.IsLoggedIn = false;

                // Скрываем текущую форму
                this.Hide();

                // Создаем и показываем форму логина
                FormLogin formLogin = new FormLogin();
                formLogin.ShowDialog(); // Показываем форму логина как диалог
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрываем текущую форму
            Form1 formMain = new Form1(currentUser); // Предполагается, что у вас есть основная форма
            formMain.Show(); // Показываем основную форму
        }
    }
}