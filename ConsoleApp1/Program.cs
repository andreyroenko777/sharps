static string EncryptPassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            using Aes aes = Aes.Create();
            aes.Key = GetEncryptionKey();
            aes.IV = new byte[16]; // Нулевой IV для простоты

            using var encryptor = aes.CreateEncryptor();
            byte[] encryptedBytes = encryptor.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);
            return Convert.ToBase64String(encryptedBytes);
        }

        static string DecryptPassword(string encryptedPassword)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
            using Aes aes = Aes.Create();
            aes.Key = GetEncryptionKey();
            aes.IV = new byte[16]; // Нулевой IV для простоты

            using var decryptor = aes.CreateDecryptor();
            byte[] passwordBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(passwordBytes);
        }