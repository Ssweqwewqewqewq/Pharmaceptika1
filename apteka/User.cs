﻿public class User
{
    public int Id { get; set; } // Добавьте это свойство
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public User(int id, string username, string password, string role)
    {
        Id = id;
        Username = username;
        Password = password;
        Role = role;
    }
}