using System;

namespace CSS_MagacinControl_App.Models.DboModels
{
    public class UserDbo
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public UserDbo(Guid id, string username, string password, string name, string surname, bool isAdmin)
        {
            Id = id;
            Username = username;
            Password = password;
            Name = name;
            Surname = surname;
            IsAdmin = isAdmin;
        }
    }
}