using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace apteka
{
    public partial class FormLogin : MaterialForm
    {
        public static bool IsLoggedIn { get; set; } = false; // Изменено на get; set;

        private Label labelCapsLock;
        private MaterialCheckBox checkBoxRememberMe;

        public FormLogin()
        {
            InitializeComponent();

            // Инициализация CheckBox для запоминания пользователя
            checkBoxRememberMe = new MaterialCheckBox
            {
                Text = "Запомнить",
                Location = new Point(500, 270)
            };
            this.Controls.Add(checkBoxRememberMe);

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

            // Установка свойства для текстового поля пароля
            textBoxPassword.UseSystemPasswordChar = true;

            // Инициализация Label для Caps Lock
            labelCapsLock = new Label
            {
                Text = "Включен Caps Lock",
                ForeColor = Color.Red,
                Visible = false,
                Location = new Point(500, 100)
            };
            this.Controls.Add(labelCapsLock);

            // Обработчик события KeyUp для проверки Caps Lock
            textBoxUsername.KeyUp += TextBox_KeyUp;
            textBoxPassword.KeyUp += TextBox_KeyUp;

            // Восстанавливаем имя пользователя, если оно сохранено
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Username))
            {
                textBoxUsername.Text = Properties.Settings.Default.Username;
                checkBoxRememberMe.Checked = true;
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            bool isCapsLock = Control.IsKeyLocked(Keys.CapsLock);
            labelCapsLock.Visible = isCapsLock;
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
           
        }

        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            // Логика для обработки изменений в текстовом поле имени пользователя (если необходимо)
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            // Логика для обработки изменений в текстовом поле пароля (если необходимо)
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Логика для обработки закрытия формы (если необходимо)
        }

        private void label1_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;

            DatabaseHelper dbHelper = new DatabaseHelper();
            User user = dbHelper.GetUser(username, password);

            if (user != null)
            {
                MessageBox.Show("Вы успешно вошли в систему!");
                IsLoggedIn = true; // Устанавливаем состояние входа

                if (checkBoxRememberMe.Checked)
                {
                    Properties.Settings.Default.Username = username;
                    Properties.Settings.Default.Save();
                }

                Form1 formMain = new Form1(user); // Передаем пользователя в Form1
                formMain.Show(); // Показываем основную форму
                this.Hide(); // Скрываем форму входа
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.");
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
    }
}