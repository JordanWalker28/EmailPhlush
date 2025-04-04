namespace EmailPhlush.Infrastructure;

public interface IEmailPhlush
{
    void Execute(ServiceQuery serviceQuery);
}