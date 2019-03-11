using System;

namespace XrmConnectionTooling
{
    public static class ConnectionStringParser
    {
        public static ConnectionString Parse(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var parameters = connectionString.Split(';');

            var cs = new ConnectionString();

            foreach (var parameter in parameters)
            {
                var keyValue = parameter.Split('=');
                if (string.IsNullOrEmpty(keyValue[0])) continue;

                switch (keyValue[0].Trim())
                {
                    case "Url":
                        cs.Url = keyValue[1].Trim();
                        break;
                    case "Username":
                        cs.Username = keyValue[1].Trim();
                        break;
                    case "Password":
                        cs.Password = keyValue[1].Trim();
                        break;
                }
            }

            return cs;
        }
    }

    public class ConnectionString
    {
        public string Url {get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
