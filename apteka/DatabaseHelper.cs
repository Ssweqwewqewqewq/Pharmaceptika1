using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace apteka
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=WIN-;Database=AptekaDB;Integrated Security=True;"; // Укажите ваш сервер
        public void AddUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role)", connection);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.ExecuteNonQuery();
            }
        }
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Users", connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new User(
                        reader.GetInt32(0), // ID
                        reader.GetString(1), // Username
                        reader.GetString(2), // Password
                        reader.GetString(3)  // Role
                    ));
                }
            }
            return users;
        }
        public List<string> LoadSymptoms()
        {
            List<string> symptoms = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT DISTINCT Symptoms FROM Medicines", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Проверяем, является ли значение NULL
                    if (!reader.IsDBNull(0))
                    {
                        symptoms.Add(reader.GetString(0));
                    }
                    else
                    {
                        // Если нужно, можно добавить пустую строку или пропустить
                        // symptoms.Add(""); // Если хотите добавлять пустую строку
                    }
                }
            }
            return symptoms;
        }

        public Medicine GetMedicineByName(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(
                    "SELECT ID, Name, Quantity, ExpirationDate, Symptoms, IsPrescription, ActiveIngredient, Contraindications, SideEffects, Price FROM Medicines WHERE Name = @Name",
                    connection);
                command.Parameters.AddWithValue("@Name", name);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Medicine(
                        reader.GetInt32(0), // ID
                        reader.GetString(1), // Name
                        reader.GetInt32(2), // Quantity
                        reader.GetDateTime(3), // ExpirationDate
                        reader.IsDBNull(4) ? null : reader.GetString(4), // Symptoms
                        reader.GetBoolean(5), // IsPrescription
                        reader.IsDBNull(6) ? null : reader.GetString(6), // ActiveIngredient
                        reader.IsDBNull(7) ? null : reader.GetString(7), // Contraindications
                        reader.IsDBNull(8) ? null : reader.GetString(8), // SideEffects
                        reader.IsDBNull(9) ? 0f : (float)reader.GetDouble(9) // Price (используйте тип float, если в БД float)
                    );
                }
            }
            return null; // Если препарат не найден
        }

        public List<string> LoadActiveIngredients()
        {
            List<string> activeIngredients = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT DISTINCT ActiveIngredient FROM Medicines", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Проверяем, является ли значение NULL
                    if (!reader.IsDBNull(0))
                    {
                        activeIngredients.Add(reader.GetString(0));
                    }
                }
            }
            return activeIngredients;
        }

        public List<string> LoadContraindications()
        {
            List<string> contraindications = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT DISTINCT Contraindications FROM Medicines", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Проверяем, является ли значение NULL
                    if (!reader.IsDBNull(0))
                    {
                        contraindications.Add(reader.GetString(0));
                    }
                }
            }
            return contraindications;
        }

        public List<string> LoadSideEffects()
        {
            List<string> sideEffects = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT DISTINCT SideEffects FROM Medicines", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Проверяем, является ли значение NULL
                    if (!reader.IsDBNull(0)) // Если значение не NULL
                    {
                        sideEffects.Add(reader.GetString(0));
                    }
                }
            }
            return sideEffects;
        }



        public void UpdateUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("UPDATE Users SET Username = @Username, Password = @Password, Role = @Role WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@ID", user.Id);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.ExecuteNonQuery();
            }
        }
        public User GetUser(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE Username = @Username AND Password = @Password", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new User(
                        reader.GetInt32(0), // ID
                        reader.GetString(1), // Username
                        reader.GetString(2), // Password
                        reader.GetString(3)  // Role
                    );
                }
            }
            return null; // Пользователь не найден
        }
        public void DeleteUser(int userId)
        {
            // Замените строку подключения на вашу
            string connectionString = "Server=WIN-;Database=AptekaDB;Integrated Security=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Users WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", userId);
                command.ExecuteNonQuery();
            }
        }
        public List<Medicine> LoadMedicines()
        {
            List<Medicine> medicines = new List<Medicine>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ID, Name, Quantity, ExpirationDate, Symptoms, IsPrescription, ActiveIngredient, Contraindications, SideEffects, Price FROM Medicines";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var medicine = new Medicine(
                                reader.GetInt32(0), // ID
                                reader.GetString(1), // Name
                                reader.GetInt32(2), // Quantity
                                reader.GetDateTime(3), // ExpirationDate
                                reader.IsDBNull(4) ? null : reader.GetString(4), // Symptoms
                                reader.GetBoolean(5), // IsPrescription
                                reader.IsDBNull(6) ? null : reader.GetString(6), // ActiveIngredient
                                reader.IsDBNull(7) ? null : reader.GetString(7), // Contraindications
                                reader.IsDBNull(8) ? null : reader.GetString(8), // SideEffects
                                reader.IsDBNull(9) ? 0.0f : (float)reader.GetDouble(9) // Price, если тип double
                            );

                            medicines.Add(medicine);
                        }
                    }
                }
            }
            return medicines;
        }

        public void UpdateMedicine(Medicine medicine)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("UPDATE Medicines SET Quantity = @Quantity WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@ID", medicine.ID);
                command.Parameters.AddWithValue("@Quantity", medicine.Quantity);
                command.ExecuteNonQuery();
            }
        }
        public void DeleteMedicine(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Удаляем связанные записи из WrittenOffMedicines
                SqlCommand deleteWrittenOffCommand = new SqlCommand("DELETE FROM WrittenOffMedicines WHERE MedicineID = @MedicineID", connection);
                deleteWrittenOffCommand.Parameters.AddWithValue("@MedicineID", id);
                deleteWrittenOffCommand.ExecuteNonQuery();

                // Теперь удаляем само лекарство
                SqlCommand deleteMedicineCommand = new SqlCommand("DELETE FROM Medicines WHERE ID = @ID", connection);
                deleteMedicineCommand.Parameters.AddWithValue("@ID", id);
                deleteMedicineCommand.ExecuteNonQuery();
            }
        }


        public void AddMedicine(Medicine medicine)
        {


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Medicines (Name, Quantity, ExpirationDate, Symptoms, IsPrescription, ActiveIngredient, Contraindications, SideEffects, Price) " +
                               "VALUES (@Name, @Quantity, @ExpirationDate, @Symptoms, @IsPrescription, @ActiveIngredient, @Contraindications, @SideEffects, @Price)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", medicine.Name);
                    command.Parameters.AddWithValue("@Quantity", medicine.Quantity);
                    command.Parameters.AddWithValue("@ExpirationDate", medicine.ExpirationDate);
                    command.Parameters.AddWithValue("@Symptoms", string.IsNullOrEmpty(medicine.Symptoms) ? string.Empty : medicine.Symptoms);
                    command.Parameters.AddWithValue("@IsPrescription", medicine.IsPrescription);
                    command.Parameters.AddWithValue("@ActiveIngredient", string.IsNullOrEmpty(medicine.ActiveIngredient) ? string.Empty : medicine.ActiveIngredient);
                    command.Parameters.AddWithValue("@Contraindications", string.IsNullOrEmpty(medicine.Contraindications) ? string.Empty : medicine.Contraindications);
                    command.Parameters.AddWithValue("@SideEffects", string.IsNullOrEmpty(medicine.SideEffects) ? string.Empty : medicine.SideEffects);
                    command.Parameters.AddWithValue("@Price", medicine.Price);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

