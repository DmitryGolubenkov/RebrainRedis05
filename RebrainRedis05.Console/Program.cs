//
// Программа находит последние 100 байт файла и выводит их в консоль.
// Также она сохраняет их в Redis.
// Если программа уже обрабатывала файл - она выводит результат, сохранённый ранее в Redis.
// Байты хранятся в кодировке Base64.
//

using StackExchange.Redis;
using System.Text;


// Путь к файлу
var path = args[0];
byte[] result = null;

// Redis

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
IDatabase db = redis.GetDatabase();

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
    var byteCount = new FileInfo(path).Length;
    // Читаем файл
    using (FileStream stream = File.OpenRead(path))
    {

        while(byteCount - stream.Position > 100)
        {
            stream.ReadByte();
        }


        result = new byte[100];
        await stream.ReadAsync(result);

        // Сохраняем в Redis
        var encodedBytes = Convert.ToBase64String(result);
        db.StringSet(path, encodedBytes);
    }
}

// Выводим результат программы
Console.Write(Encoding.UTF8.GetString(result));