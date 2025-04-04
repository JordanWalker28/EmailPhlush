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
    private readonly ServiceQuery _serviceQuery = new();
    
    [Fact]
    public void Execute_Should_ConnectAndAuthenticate()
    {
        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object, _mockEmailService.Object, _emailConfig.Object);
        emailPhlush.Execute(_serviceQuery);

        _mockEmailService.Verify(es => es.ConnectAndAuthenticate(_emailConfig.Object), Times.Once);
    }

    [Fact]
    public void Execute_Should_ScanEmails_When_JobTypeIsScan()
    {
        _emailConfig.Setup(x => x.Method).Returns(JobType.Scan);
        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object, _mockEmailService.Object, _emailConfig.Object);

        emailPhlush.Execute(_serviceQuery);

        _mockEmailService.Verify(es => es.ScanEmails(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public void Execute_Should_HandleException_And_NotThrow()
    {
        _mockEmailService.Setup(es => es.ConnectAndAuthenticate(_emailConfig.Object))
                         .Throws(new InvalidOperationException("Connection failed"));

        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object, _mockEmailService.Object, _emailConfig.Object);

        var exception = Record.Exception(() => emailPhlush.Execute(_serviceQuery));
        Assert.Null(exception);
    }

    [Fact]
    public void Execute_Should_Disconnect_EmailService_AfterExecution()
    {
        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object, _mockEmailService.Object, _emailConfig.Object);
        emailPhlush.Execute(_serviceQuery);

        _mockEmailService.Verify(es => es.Disconnect(), Times.Once);
    }

    [Fact]
    public void Execute_Should_DeleteEmails_When_JobTypeIsDeleteSingle()
    {
        _emailConfig.Setup(x => x.Method).Returns(JobType.DeleteSingle);
        var serviceQuery = new ServiceQuery
        {
            emailSenderToRemove = "sender@example.com" 
        };
        
        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object, _mockEmailService.Object, _emailConfig.Object);
        
        emailPhlush.Execute(serviceQuery);

        _mockEmailService.Verify(es => es.DeleteEmailsFromSender(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void Execute_Should_DeleteAllEmails_When_JobTypeIsDeleteAll()
    {
        _emailConfig.Setup(x => x.Method).Returns(JobType.DeleteAll);
        var serviceQuery = new ServiceQuery
        {
            emailSendersToRemove = ["sender1@example.com", "sender2@example.com"]
        };
        
        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object, _mockEmailService.Object, _emailConfig.Object);
        
        emailPhlush.Execute(serviceQuery);

        _mockEmailService.Verify(es => es.DeleteEmailsFromSender(It.IsAny<List<string>>()), Times.Once);
    }

    [Fact]
    public void Execute_Should_WriteMessage_When_JobTypeIsUnrecognized()
    {
        _emailConfig.Setup(x => x.Method).Returns((JobType)999);
        
        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object, _mockEmailService.Object, _emailConfig.Object);
        
        emailPhlush.Execute(_serviceQuery);

        _writerService.Verify(w => w.Write("Job Type Not Recognised"), Times.Once);
    }

    [Fact]
    public void Execute_Should_LogErrorMessage_When_ExceptionOccurs()
    {
        _mockEmailService.Setup(es => es.ConnectAndAuthenticate(_emailConfig.Object))
                         .Throws(new InvalidOperationException("Connection failed"));

        var emailPhlush = new EmailPhlush.Infrastructure.EmailPhlush(_writerService.Object, _mockEmailService.Object, _emailConfig.Object);
        
        emailPhlush.Execute(_serviceQuery);

        _writerService.Verify(w => w.Write(It.Is<string>(msg => msg.Contains("An error occurred: Connection failed"))), Times.Once);
    }
}