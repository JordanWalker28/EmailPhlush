using EmailPhlush.Infrastructure;

namespace EmailPhlush;

public interface IEmailConfig
{
    public string Email { get; }
    public string Password { get; }
    public JobType Method { get; }
}

public class EmailConfig(string email, string password, string method) : IEmailConfig
{
    public string Email { get; } = email;
    public string Password { get; } = password;
    public JobType Method { get; } = (JobType)Enum.Parse(typeof(JobType), method);
}