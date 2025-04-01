namespace EmailPhlush.Infrastructure;

public class Writer : IWriter
{
    public void Write(string content) => CreateUserMessage(content);
    private static void CreateUserMessage(string message)
    {
        Console.WriteLine(message);
    }
}