using System;
using System.Collections.Generic;
using EmailPhlush.Infrastructure;
using Moq;
using Xunit;

namespace EmailPhlush.Tests.Infrastructure;

public class EmailPhlushTests
{
    private readonly Mock<IWriter> _writerService = new();
    private readonly Mock<IEmailService> _mockEmailService = new();
    private readonly Mock<IEmailConfig> _emailConfig = new();

    private readonly string _email = "test@example.com";
    private readonly string _password = "password123";

    [Fact]
    public void Execute_Should_ConnectAndAuthenticate()
    {
        var emailPhlush =
            new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object, _mockEmailService.Object,
                _emailConfig.Object);

        emailPhlush.Execute();

        _mockEmailService.Verify(es => es.ConnectAndAuthenticate(_emailConfig.Object), Times.Once);
    }

    [Fact]
    public void Execute_Should_ScanEmails_When_JobTypeIsScan()
    {
        _emailConfig.Setup(x => x.Method).Returns(JobType.Scan);
        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object,_mockEmailService.Object, _emailConfig.Object);

        emailPhlush.Execute();

        _mockEmailService.Verify(es => es.ScanEmails(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public void Execute_Should_NotCallAnyMethod_When_JobTypeIsUnrecognized()
    {
        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object,_mockEmailService.Object, _emailConfig.Object);

        emailPhlush.Execute();

        _mockEmailService.Verify(es => es.ScanEmails(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        _mockEmailService.Verify(es => es.DeleteEmailsFromSender(It.IsAny<List<string>>()), Times.Never);
    }

    [Fact]
    public void Execute_Should_HandleException_And_NotThrow()
    {
        _mockEmailService.Setup(es => es.ConnectAndAuthenticate(_emailConfig.Object))
                         .Throws(new InvalidOperationException("Connection failed"));

        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object,_mockEmailService.Object, _emailConfig.Object);

        var exception = Record.Exception(() => emailPhlush.Execute());
        Assert.Null(exception);
    }

    [Fact]
    public void Execute_Should_Disconnect_EmailService_AfterExecution()
    {
        var emailPhlush =
            new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object, _mockEmailService.Object,
                _emailConfig.Object);
        emailPhlush.Execute();

        _mockEmailService.Verify(es => es.Disconnect(), Times.Once);
    }
}