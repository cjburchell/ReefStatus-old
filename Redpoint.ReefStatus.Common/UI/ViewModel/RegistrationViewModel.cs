// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationViewModel.cs" company="Redpoint Apps">
//   2011
// </copyright>
// <summary>
//   The registration view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.UI.ViewModel
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Mail;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Windows.Input;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;
    using Microsoft.Win32;

    using RedPoint.ReefStatus.Common.Settings;

    /// <summary>
    /// The registration view model.
    /// </summary>
    public class RegistrationViewModel : BindableBase
    {
        #region Constants and Fields

        /// <summary>
        /// The do not register command.
        /// </summary>
        private ICommand doNotRegisterCommand;

        /// <summary>
        /// The email.
        /// </summary>
        private string email;

        /// <summary>
        /// The name of the person registerd.
        /// </summary>
        private string name;

        /// <summary>
        /// The register command.
        /// </summary>
        private ICommand registerCommand;

        private static RegistrationViewModel instance;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationViewModel"/> class.
        /// </summary>
        public RegistrationViewModel()
        {
            try
            {
                this.name = (string)Registry.GetValue(ReefStatusSettings.RegistryRootKey + "\\SOFTWARE\\RedPoint\\Reef Status", "RegistrationName", string.Empty);
                this.email = (string)Registry.GetValue(ReefStatusSettings.RegistryRootKey + "\\SOFTWARE\\RedPoint\\Reef Status", "RegistrationEmail", string.Empty);

                var isReg =
                        Registry.GetValue(
                            ReefStatusSettings.RegistryRootKey + "\\SOFTWARE\\RedPoint\\Reef Status",
                            "IsRegisterd",
                            false.ToString()) as string;
                this.isRegisterd = !string.IsNullOrEmpty(isReg) && bool.Parse(isReg);

                var isFinished =
                        Registry.GetValue(
                            ReefStatusSettings.RegistryRootKey + "\\SOFTWARE\\RedPoint\\Reef Status",
                            "IsFinishedRegistration",
                            false.ToString()) as string;
                this.isFinishedRegistration = !string.IsNullOrEmpty(isFinished) && bool.Parse(isFinished);
            }
            catch (System.IO.IOException)
            {
                this.Name = string.Empty;
                this.Email = string.Empty;
                this.IsRegisterd = false;
                this.IsFinishedRegistration = false;
            }
        }

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static RegistrationViewModel Instance
        {
            get
            {
                return instance ?? (instance = new RegistrationViewModel());
            }
        }

        /// <summary>
        /// Gets DoNotRegisterCommand.
        /// </summary>
        public ICommand DoNotRegisterCommand
        {
            get
            {
                return this.doNotRegisterCommand ??
                       (this.doNotRegisterCommand = new DelegateCommand(this.DoNotRegister));
            }
        }

        /// <summary>
        /// Gets or sets Email.
        /// </summary>
        public string Email
        {
            get
            {
                return this.email;
            }

            set
            {
                if (value != this.email)
                {
                    this.email = value;
                    this.OnPropertyChanged(() => this.Email);

                    try
                    {
                        
                        Registry.SetValue(
                            ReefStatusSettings.RegistryRootKey + "\\SOFTWARE\\RedPoint\\Reef Status",
                            "RegistrationEmail",
                            this.email);
                    }
                    catch (System.IO.IOException)
                    {
                    }
                }
            }
        }

        private bool isFinishedRegistration;

        /// <summary>
        /// Gets or sets a value indicating whether IsFinishedRegistration.
        /// </summary>
        public bool IsFinishedRegistration
        {
            get
            {
                return this.isFinishedRegistration;
            }

            set
            {
                this.isFinishedRegistration = value; 
                try
                {
                    Registry.SetValue(
                        ReefStatusSettings.RegistryRootKey + "\\SOFTWARE\\RedPoint\\Reef Status",
                        "IsFinishedRegistration",
                        this.isFinishedRegistration);
                }
                catch (System.IO.IOException)
                {
                }
            }
        }

        private bool isRegisterd;

        /// <summary>
        /// Gets or sets a value indicating whether IsRegisterd.
        /// </summary>
        public bool IsRegisterd
        {
            get
            {
                return this.isRegisterd;
            }

            set
            {
                this.isRegisterd = value;
                try
                {
                    Registry.SetValue(
                        ReefStatusSettings.RegistryRootKey + "\\SOFTWARE\\RedPoint\\Reef Status",
                        "IsRegisterd",
                        this.isRegisterd);
                }
                catch (System.IO.IOException)
                {
                }
            }
        }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.OnPropertyChanged(() => this.Name);

                    try
                    {
                        Registry.SetValue(
                            ReefStatusSettings.RegistryRootKey + "\\SOFTWARE\\RedPoint\\Reef Status",
                            "RegistrationName",
                            this.name);
                    }
                    catch (System.IO.IOException)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Gets RegisterCommand.
        /// </summary>
        public ICommand RegisterCommand
        {
            get
            {
                return this.registerCommand ?? (this.registerCommand = new DelegateCommand(this.Register));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The do not register.
        /// </summary>
        private void DoNotRegister()
        {
            this.IsFinishedRegistration = true;
        }

        /// <summary>
        /// The register.
        /// </summary>
        private void Register()
        {
            this.IsRegisterd = true;
            this.IsFinishedRegistration = true;

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
                            "Registration " + Application.ProductName + " " +
                            Assembly.GetEntryAssembly().GetName().Version,
                        Body =
                            string.Format(
                                "Name: {0}\nE-mail: {1}\n OS: {2} {3}",
                                        this.Name,
                                        this.Email,
                                Environment.OSVersion,
                                Environment.Is64BitOperatingSystem ? "64 Bit" : "32 Bit")
                    };

                message.To.Add("cjburchell@yahoo.com");
                mailClient.Send(message);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        #endregion
    }
}