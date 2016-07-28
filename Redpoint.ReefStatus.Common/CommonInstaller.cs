// <copyright file="CommonInstaller.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common
{
    using System.ComponentModel;
    using System.Configuration.Install;

    /// <summary>
    /// Installer Class
    /// </summary>
    [RunInstaller(true)]
    public partial class CommonInstaller : Installer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonInstaller"/> class.
        /// </summary>
        public CommonInstaller()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When overridden in a derived class, performs the installation.
        /// </summary>
        /// <param name="stateSaver">An <see cref="T:System.Collections.IDictionary"/> used to save information needed to perform a commit, rollback, or uninstall operation.</param>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="stateSaver"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Exception">
        /// An exception occurred in the <see cref="E:System.Configuration.Install.Installer.BeforeInstall"/> event handler of one of the installers in the collection.
        /// -or-
        /// An exception occurred in the <see cref="E:System.Configuration.Install.Installer.AfterInstall"/> event handler of one of the installers in the collection.
        /// </exception>
        /*public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\ReefStatus";
            if (!File.Exists(appDataDir + "\\settings.dat"))
            {
                string userDataDir = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\ReefStatus";
                if (File.Exists(userDataDir + "\\settings.dat"))
                {

                    if (!Directory.Exists(userDataDir))
                    {
                        Directory.CreateDirectory(userDataDir);
                    }

                    string settingsFile = System.IO.File.ReadAllText(userDataDir + "\\settings.dat");
                    File.WriteAllText(appDataDir + "\\settings.dat", settingsFile.Replace(userDataDir, appDataDir));

                    File.Copy(userDataDir + "\\ReefStatus.mdf", appDataDir + "\\ReefStatus.mdf");
                    File.Copy(userDataDir + "\\ReefStatus.ldf", appDataDir + "\\ReefStatus.ldf");
                }
            }
        }*/
    }
}
