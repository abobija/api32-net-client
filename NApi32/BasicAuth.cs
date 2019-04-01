using System;

namespace NApi32
{
    public class BasicAuth
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public BasicAuth() { }

        public BasicAuth(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Key() 
            => Convert.ToBase64String(Api32Client.Encoding.GetBytes($"{Username}:{Password}"));
    }
}
