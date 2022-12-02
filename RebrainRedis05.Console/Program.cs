//
// Программа находит последние 100 байт файла и выводит их в консоль.
// Также она сохраняет их в Redis.
// Если программа уже обрабатывала файл - она выводит результат, сохранённый ранее в Redis.
// Байты хранятся в кодировке Base64.
//

using StackExchange.Redis;
using System.Text;


// Путь к файлу
var path = args.Length > 0 ? args[0] : "C:\\Users\\xxdim\\source\\repos\\Litero\\src\\Litero.Users.API\\bin\\Debug\\net7.0\\dapr\\components\\session_storage.yaml";

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
    // Читаем файл
    using (var stream = File.OpenRead(path))
    {
        // Находим количество байт в файле
        var byteCount = stream.Length;

        // Если нужно - смещаемся 
        if (byteCount > 100)
        {
            // Переходим на позицию количество байт - 100
            stream.Seek(byteCount - 100, SeekOrigin.Begin);
        }
        // Пишем в буфер конец файла
        result = new byte[100];
        await stream.ReadAsync(result);


        // Сохраняем в Redis
        var encodedBytes = Convert.ToBase64String(result);
        db.StringSet(path, encodedBytes);
    }
}

// Выводим результат программы
Console.Write(Encoding.UTF8.GetString(result));