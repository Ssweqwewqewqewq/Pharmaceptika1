using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace apteka
{
    public partial class FormManageUsers : MaterialForm
    {
        private DatabaseHelper dbHelper; // Ссылка на класс для работы с БД
        private User currentUser; // Текущий пользователь

        public FormManageUsers(User user)
        {
            InitializeComponent();
            currentUser = user; // Сохраняем текущего пользователя
            dbHelper = new DatabaseHelper(); // Инициализируем DatabaseHelper
            LoadUsers(); // Загружаем пользователей
            InitializeRoleComboBox(); // Инициализируем комбобокс с ролями

            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для доступа к этой форме.");
                this.Close();
            }
        }

        private void LoadUsers()
        {
            // Получаем список пользователей из базы данных
            List<User> users = dbHelper.GetAllUsers();
            listBoxUsers.DataSource = users;
            listBoxUsers.DisplayMember = "Username"; // Отображаем имя пользователя
        }

        private void InitializeRoleComboBox()
        {
            comboBoxRole.Items.Clear(); // Очищаем комбобокс
            comboBoxRole.Items.Add("Admin"); // Добавляем роль администратора
            comboBoxRole.Items.Add("User "); // Добавляем роль пользователя
            comboBoxRole.SelectedIndex = 0; // Устанавливаем выбранный элемент по умолчанию
        }

        private void buttonAddUser_Click(object sender, EventArgs e)
        {
            // Логика для добавления нового пользователя
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;
            string role = comboBoxRole.SelectedItem.ToString();

            User newUser = new User(0, username, password, role);
            dbHelper.AddUser(newUser);
            LoadUsers(); // Обновляем список пользователей
            MessageBox.Show("Пользователь добавлен.");
        }

        private void buttonEditUser_Click(object sender, EventArgs e)
        {
            // Логика для редактирования выбранного пользователя
            User selectedUser = (User)listBoxUsers.SelectedItem;
            if (selectedUser != null)
            {
                selectedUser.Username = textBoxUsername.Text;
                selectedUser.Password = textBoxPassword.Text;
                selectedUser.Role = comboBoxRole.SelectedItem.ToString();
                dbHelper.UpdateUser(selectedUser);
                LoadUsers(); // Обновляем список пользователей
                MessageBox.Show("Пользователь обновлен.");
            }
        }

        private void buttonDeleteUser_Click(object sender, EventArgs e)
        {
            // Логика для удаления выбранного пользователя
            User selectedUser = (User)listBoxUsers.SelectedItem;
            if (selectedUser != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этого пользователя?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    dbHelper.DeleteUser(selectedUser.Id); // Предполагается, что у User есть свойство Id
                    LoadUsers(); // Обновляем список пользователей
                    MessageBox.Show("Пользователь удален.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для удаления.");
            }
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

        private void label2_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрываем текущую форму
            Form1 formMain = new Form1(currentUser); // Предполагается, что у вас есть основная форма
            formMain.Show(); // Показываем основную форму
        }

        private void listBoxUsers_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Загружаем данные выбранного пользователя в текстовые поля
            User selectedUser = (User)listBoxUsers.SelectedItem;
            if (selectedUser != null)
            {
                textBoxUsername.Text = selectedUser.Username;
                textBoxPassword.Text = selectedUser.Password;
                comboBoxRole.SelectedItem = selectedUser.Role;
            }
        }
    }
}