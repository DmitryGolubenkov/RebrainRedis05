namespace RebrainRedis05.Test;

[TestClass]
public class ProgramTest
{
    [TestMethod]
    public async Task Test_Execute()
    {
        var result = await Executor.Execute(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt"));

        Assert.AreEqual("ZwLgbvGFZMHFXqem43wtEfLpFQncwiAm6RzY3Kc54gGMq7hR4M6K2cARp3yjX6pqiqika3V7Abt8YPrzqU7EyG9tjDF5HdLfHXJx", result);
    }
}