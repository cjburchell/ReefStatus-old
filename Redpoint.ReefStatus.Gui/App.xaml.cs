namespace RedPoint.ReefStatus.Gui
{
    using System;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Windows;
    using Common.Localization;
    using RedPoint.ReefStatus.Common;
    using RedPoint.ReefStatus.Common.UI.ViewModel;
    using RedPoint.ReefStatus.Gui.ViewModels;
    using RedPoint.ReefStatus.Gui.Views;

    /// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
       [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
       protected override void OnStartup(StartupEventArgs e)
       {
           AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

           AppExceptionHandler.Handle();
           base.OnStartup(e);

           try
           {
               ResourceLoader.LoadFileResources(Current.Resources.MergedDictionaries, CultureInfo.CurrentCulture.Name);
           }
           catch (Exception)
           {
               // TODO: Log the exception
           }

           var registration = new RegistrationViewModel();
           if (!registration.IsFinishedRegistration)
           {
               (new RegistrationView { DataContext = registration }).Show();
           }
       }

       static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
       {
           if (args.Name.StartsWith("System.Data.SQLite, ", StringComparison.OrdinalIgnoreCase))
           {
               string realName = IntPtr.Size == 8 ? "x64/System.Data.SQLite.dll" : "System.Data.SQLite.dll";
               return System.Reflection.Assembly.LoadFile(realName);
           }

           return null;
       }

       private void Application_Exit(object sender, ExitEventArgs e)
       {
       }

       private void Application_Startup(object sender, StartupEventArgs e)
       {
           System.Windows.Forms.Application.EnableVisualStyles();
           System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
       }
   }
}