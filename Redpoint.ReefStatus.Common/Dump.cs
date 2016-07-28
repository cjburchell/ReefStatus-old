// <copyright file="Dump.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Mail;
    using System.Reflection;
    using System.Windows.Forms;

    using RedPoint.ReefStatus.Common.UI.ViewModel;

    using UI;

    public class AppExceptionHandler
    {
        /// <summary>
        /// Handles this instance.
        /// </summary>
        public static void Handle()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        }

        public static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                if (
                    new ErrorWindow
                        {
                            Discription =
                                "Sorry but and error was encounted in " + Application.ProductName +
                                " and will need to close.",
                            Details = exception.ToString()
                        }.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var mailClient = new SmtpClient
                            {
                                Host = "plus.smtp.mail.yahoo.com",
                                Port = 25,
                                Credentials = new NetworkCredential("cjburchell", "redpoint")
                            };

                        var message = new MailMessage
                            {
                                From = new MailAddress("cjburchell@yahoo.com", "Reef Status"),
                                Subject =
                                    "Exception Report " + Application.ProductName + " " +
                                    Assembly.GetEntryAssembly().GetName().Version,
                                Body =
                                    string.Format(
                                        "Name: {0}\nE-mail: {1}\n OS: {2} {3}\n\nException:\n {4}",
                                        RegistrationViewModel.Instance.Name,
                                        RegistrationViewModel.Instance.Email,
                                        Environment.OSVersion,
                                        Environment.Is64BitOperatingSystem ? "64 Bit" : "32 Bit",
                                        exception)
                            };

                        message.To.Add("cjburchell@yahoo.com");
                        mailClient.Send(message);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                }
            }
        }
    }
}
