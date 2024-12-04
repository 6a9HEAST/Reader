using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Diagnostics;
using Android.OS;
using Android.Provider;
using Android.Content;
using Microsoft.Maui.Controls.Compatibility;
//using Reader.Controls;
using Topten.RichTextKit;

namespace Reader
{

    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            AskPermission();
        }

        private async Task<bool> AskPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

            if (status == PermissionStatus.Granted)
            {
                System.Diagnostics.Debug.WriteLine("Разрешение предоставлено.");
                return true;
            }

            if (status == PermissionStatus.Denied && Permissions.ShouldShowRationale<Permissions.StorageRead>())
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Необходимо разрешение",
                    "Приложению нужно разрешение на чтение файлов для работы.",
                    "ОК"
                );
            }

            if (!HasManageExternalStoragePermission())
            {
                RequestManageExternalStoragePermission();
            }

            status = await Permissions.RequestAsync<Permissions.StorageRead>();

            if (status == PermissionStatus.Granted)
            {
                System.Diagnostics.Debug.WriteLine("Разрешение предоставлено.");
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Разрешение отклонено.");
                return false;
            }

            
        }

        public static bool HasManageExternalStoragePermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                return Android.OS.Environment.IsExternalStorageManager;
            }
            return true; // Для версий ниже Android 11
        }

        public static void RequestManageExternalStoragePermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                var intent = new Intent(Settings.ActionManageAllFilesAccessPermission);
                intent.SetData(Android.Net.Uri.Parse("package:" + Android.App.Application.Context.PackageName));
                Android.App.Application.Context.StartActivity(intent);
            }
        }
    }
}
