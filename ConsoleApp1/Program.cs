using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager
{
    class Program
    {
        static Dictionary<string, (string Login, string EncryptedPassword)> passwordStorage = new();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n--- Меню ---");
                Console.WriteLine("1. Добавить запись");
                Console.WriteLine("2. Удалить запись");
                Console.WriteLine("3. Просмотреть записи");
                Console.WriteLine("4. Выход");
                Console.Write("Выберите действие: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        AddRecord();
                        break;
                    case "2":
                        DeleteRecord();
                        break;
                    case "3":
                        ViewRecords();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор, попробуйте снова.");
                        break;
                }
            }
        }

        static void AddRecord()
        {
            Console.Write("Введите название сайта: ");
            string site = Console.ReadLine() ?? string.Empty;

            if (passwordStorage.ContainsKey(site))
            {
                Console.WriteLine("Запись для этого сайта уже существует.");
                return;
            }

            Console.Write("Введите логин: ");
            string login = Console.ReadLine() ?? string.Empty;

            Console.Write("Введите пароль: ");
            string password = Console.ReadLine() ?? string.Empty;

            string encryptedPassword = EncryptPassword(password);
            passwordStorage[site] = (login, encryptedPassword);

            Console.WriteLine("Запись добавлена.");
        }

        static void DeleteRecord()
        {
            Console.Write("Введите название сайта для удаления: ");
            string site = Console.ReadLine() ?? string.Empty;

            if (passwordStorage.Remove(site))
            {
                Console.WriteLine("Запись удалена.");
            }
            else
            {
                Console.WriteLine("Запись не найдена.");
            }
        }

        static void ViewRecords()
        {
            if (passwordStorage.Count == 0)
            {
                Console.WriteLine("Нет сохранённых записей.");
                return;
            }

            Console.WriteLine("\n--- Список записей ---");
            foreach (var record in passwordStorage)
            {
                string decryptedPassword = DecryptPassword(record.Value.EncryptedPassword);
                Console.WriteLine($"Сайт: {record.Key}, Логин: {record.Value.Login}, Пароль: {decryptedPassword}");
            }
        }

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

        static byte[] GetEncryptionKey()
        {
            // Генерация статического ключа на основе строки (это упрощённый подход, можно улучшить)
            string key = "your-encryption-key";
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        }
    }
}
