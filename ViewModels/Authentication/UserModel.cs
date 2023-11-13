using System;

namespace CSS_MagacinControl_App.ViewModels.Authentication
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public UserModel(Guid id, string username, string password, bool isAdmin, string name, string surname)
        {
            Id = id;
            Username = username;
            Password = password;
            IsAdmin = isAdmin;
            Name = name;
            Surname = surname;
        }

        public UserModel() { }
    }
}