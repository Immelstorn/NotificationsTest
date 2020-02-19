using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using Java.Lang;
using Xamarin.Forms;

namespace NotificationsTest.Droid
{
    [Activity(Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        AnswerReciever receiver;
        public static long reminderInterval = /*24 * 60 * */30 * 1000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            CreateNotificationFromIntent(Intent);
            receiver = new AnswerReciever();

            //trying to schedule notification for specific time of the day
            var alarmIntent = new Intent(this, typeof(AlarmReceiver));
            var pending = PendingIntent.GetBroadcast(this, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = GetSystemService(AlarmService).JavaCast<AlarmManager>();
            alarmManager.SetInexactRepeating(AlarmType.RtcWakeup, FirstReminder(), reminderInterval, pending);
        }

        public static long FirstReminder()
        {
            var dt = DateTime.Now;
            //dt = dt.Date.AddHours(22).AddMinutes(00).AddSeconds(0);
            //TODO set from user settings

            var timestamp = (long)(dt.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            return timestamp;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            CreateNotificationFromIntent(intent);
        }

        protected override void OnResume()
        {
            try
            {
                UnregisterReceiver(receiver);
            }
            catch (IllegalArgumentException) { }

            RegisterReceiver(receiver, new IntentFilter(Constants.ANDROID_NOTIFICATION_ACTION));

            base.OnResume();
        }

        protected override void OnDestroy()
        {
            UnregisterReceiver(receiver);
            base.OnDestroy();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.Extras.GetString(AndroidNotificationManager.TitleKey);
                string message = intent.Extras.GetString(AndroidNotificationManager.MessageKey);
                DependencyService.Get<INotificationManager>().ReceiveNotification(title, message);
            }
        }
    }
}