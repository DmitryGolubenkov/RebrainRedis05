﻿//
// Программа находит последние 100 байт файла и выводит их в консоль.
// Также она сохраняет их в Redis.
// Если программа уже обрабатывала файл - она выводит результат, сохранённый ранее в Redis.
// Байты хранятся в кодировке Base64.
//

using StackExchange.Redis;
using System.Text;

public static class Executor
{
    public static async Task<string> Execute(string path)
    {
        // Redis
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
        IDatabase db = redis.GetDatabase();
        byte[] result = null;
        // Пробуем получить значение из Redis
        var cachedValue = await db.StringGetAsync(path);
        if (cachedValue.HasValue)
        {
            // Если есть - декодируем и пишем в результат
            result = Convert.FromBase64String(cachedValue.ToString());
        }
        else
        {
            // Находим количество байт в файле
            List<byte> bytes = new List<byte>();
            // Читаем файл
            using (FileStream stream = File.OpenRead(path))
            {
                int byteRead;
                while ((byteRead = stream.ReadByte()) != -1)
                {
                    if (bytes.Count == 100)
                    {
                        bytes.RemoveAt(0);
                    }

                    bytes.Add((byte)byteRead);
                }

                // Сохраняем в Redis
                var encodedBytes = Convert.ToBase64String(bytes.ToArray());
                db.StringSet(path, encodedBytes);

                result = bytes.ToArray();
            }
        }

        return Encoding.UTF8.GetString(result);
    }
}