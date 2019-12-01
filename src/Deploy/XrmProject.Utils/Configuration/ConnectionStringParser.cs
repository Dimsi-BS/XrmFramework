// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;

namespace XrmProject.Utils.Configuration
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
                        cs.Password = ExtractParameterValue("Password", parameter);
                        break;
                }
            }

            return cs;
        }

        private static string ExtractParameterValue(string parameterName, string rawValue)
        {
            var value = rawValue.Replace($"{parameterName}=", string.Empty);
            value = value.Trim();
            return value;
        }
    }

    public class ConnectionString
    {
        public string Url {get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
