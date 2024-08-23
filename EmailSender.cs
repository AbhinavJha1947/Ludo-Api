using Newtonsoft.Json.Linq;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

public class EmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendEmail(string senderName, string senderEmail, string receiverName, string receiverEmail, string subject, string message)
    {
        var apiKey = _configuration["BrevoApi:ApiKey"];
        Configuration.Default.AddApiKey("api-key", apiKey);

        var apiInstance = new TransactionalEmailsApi();
        SendSmtpEmailSender sender = new SendSmtpEmailSender(senderName, senderEmail);
        SendSmtpEmailTo receiver = new SendSmtpEmailTo(receiverEmail, receiverName);
        List<SendSmtpEmailTo> to = new List<SendSmtpEmailTo> { receiver };

        SendSmtpEmail sendSmtpEmail = new SendSmtpEmail(
            sender: sender,
            to: to,
            subject: subject,
            textContent: message
        );

        try
        {
            CreateSmtpEmail response = apiInstance.SendTransacEmail(sendSmtpEmail);
            Console.WriteLine(response.ToJson());
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception when calling TransactionalEmailsApi.SendTransacEmail: {e.Message}");
        }
    }
}
