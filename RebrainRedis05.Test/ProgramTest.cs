using StackExchange.Redis;

namespace RebrainRedis05.Test;

[TestClass]
public class ProgramTest
{
    // ���� � ������. ������, �� ���� ���, ���� �������� �� ���������. � Redis ����. � ������������ ����. 
    // �� �������� ��.
    private string keyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");

    // ���� � ��������� ���������
    [TestMethod, Priority(1)]
    [DoNotParallelize]
    public async Task Test_ExecuteFirst()
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
        IDatabase db = redis.GetDatabase();
        await db.StringGetDeleteAsync(keyPath);

        var result = await Executor.Execute(keyPath);

        Assert.AreEqual("ZwLgbvGFZMHFXqem43wtEfLpFQncwiAm6RzY3Kc54gGMq7hR4M6K2cARp3yjX6pqiqika3V7Abt8YPrzqU7EyG9tjDF5HdLfHXJx", result);
    }

    // ���� � �������������� ���������
    [TestMethod, Priority(2)]
    [DoNotParallelize]
    public async Task Test_ExecuteCached()
    {
        var result = await Executor.Execute(keyPath);

        Assert.AreEqual("ZwLgbvGFZMHFXqem43wtEfLpFQncwiAm6RzY3Kc54gGMq7hR4M6K2cARp3yjX6pqiqika3V7Abt8YPrzqU7EyG9tjDF5HdLfHXJx", result);
    }
}