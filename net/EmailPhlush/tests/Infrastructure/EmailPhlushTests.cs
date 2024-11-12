using Moq;
using Xunit;
using EmailPhlush.Infrastructure;
using Assert = Xunit.Assert;

public class EmailPhlushTests
{
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly string _email = "test@example.com";
    private readonly string _password = "password123";

    public EmailPhlushTests()
    {
        _mockEmailService = new Mock<IEmailService>();
    }

    [Fact]
    public void Execute_Should_ConnectAndAuthenticate()
    {
        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_mockEmailService.Object, _email, _password, "scan");

        emailPhlush.Execute();
        
        _mockEmailService.Verify(es => es.ConnectAndAuthenticate(_email, _password), Times.Once);
    }

    [Fact]
    public void Execute_Should_ScanEmails_When_JobTypeIsScan()
    {
        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_mockEmailService.Object, _email, _password, "scan");

        emailPhlush.Execute();

        _mockEmailService.Verify(es => es.ScanEmails(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public void Execute_Should_NotCallAnyMethod_When_JobTypeIsUnrecognized()
    {
        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_mockEmailService.Object, _email, _password, "unknown");

        emailPhlush.Execute();

        _mockEmailService.Verify(es => es.ScanEmails(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        _mockEmailService.Verify(es => es.DeleteEmailsFromSender(It.IsAny<List<string>>()), Times.Never);
    }

    [Fact]
    public void Execute_Should_HandleException_And_NotThrow()
    {
        _mockEmailService.Setup(es => es.ConnectAndAuthenticate(_email, _password))
                         .Throws(new InvalidOperationException("Connection failed"));

        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_mockEmailService.Object, _email, _password, "scan");

        var exception = Record.Exception(() => emailPhlush.Execute());
        Assert.Null(exception);
    }

    [Fact]
    public void Execute_Should_Disconnect_EmailService_AfterExecution()
    {
        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_mockEmailService.Object, _email, _password, "scan");

        emailPhlush.Execute();

        _mockEmailService.Verify(es => es.Disconnect(), Times.Once);
    }
}
