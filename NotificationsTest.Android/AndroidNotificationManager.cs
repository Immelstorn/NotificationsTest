using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(NotificationsTest.Droid.AndroidNotificationManager))]
namespace NotificationsTest.Droid
{
    public class AndroidNotificationManager : INotificationManager
    {
        const int mainActivityPendingIntentId = 0;
        const int firstActionPendingIntentId = 1;
        const int secondActionPendingIntentId = 2;

        bool channelInitialized = false;
        int messageId = -1;
        NotificationManager manager;

        public static string TitleKey => "Title";
        public static string MessageKey => "Message";

        public event EventHandler NotificationReceived;

        public void Initialize()
        {
            CreateNotificationChannel();
        }

        public int ScheduleNotification(string title, string message)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            messageId++;

            var mainActivityIintent = new Intent(AndroidApp.Context, typeof(MainActivity));

            var mainActivityPendingIntent = PendingIntent.GetActivity(AndroidApp.Context, mainActivityPendingIntentId, mainActivityIintent, PendingIntentFlags.OneShot);

            var firstActionIntent = new Intent(Constants.ANDROID_NOTIFICATION_ACTION);
            firstActionIntent.PutExtra(Constants.ANDROID_NOTIFICATION_EXTRA_ACTION, false);
            firstActionIntent.PutExtra(Constants.ANDROID_NOTIFICATION_EXTRA_DATE, DateTime.Now.Date.ToString("s"));
            firstActionIntent.PutExtra(Constants.ANDROID_NOTIFICATION_EXTRA_ID, messageId);

            var pFirstActionIntent = PendingIntent.GetBroadcast(AndroidApp.Context, firstActionPendingIntentId, firstActionIntent, PendingIntentFlags.OneShot);

            var secondActionIntent = new Intent(Constants.ANDROID_NOTIFICATION_ACTION);
            secondActionIntent.PutExtra(Constants.ANDROID_NOTIFICATION_EXTRA_ACTION, true);
            secondActionIntent.PutExtra(Constants.ANDROID_NOTIFICATION_EXTRA_DATE, DateTime.Now.Date.ToString("s"));
            secondActionIntent.PutExtra(Constants.ANDROID_NOTIFICATION_EXTRA_ID, messageId);

            var pSeconActionIntent = PendingIntent.GetBroadcast(AndroidApp.Context, secondActionPendingIntentId, secondActionIntent, PendingIntentFlags.OneShot);


            var builder = new NotificationCompat.Builder(AndroidApp.Context, Constants.ANDROID_NOTIFICATION_DEFAULT_CHANNEL_ID)
                .SetContentIntent(mainActivityPendingIntent)
                .SetContentTitle(title)
                .SetContentText(message)
                .AddAction(Resource.Drawable.navigation_empty_icon, Constants.ANDROID_NOTIFICATION_ANSWER_NO, pFirstActionIntent)
                .AddAction(Resource.Drawable.navigation_empty_icon, Constants.ANDROID_NOTIFICATION_ANSWER_YES, pSeconActionIntent)
                .SetAutoCancel(true)
                .SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.navigation_empty_icon))
                .SetSmallIcon(Resource.Drawable.navigation_empty_icon)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

            var notification = builder.Build();
            manager.Notify(messageId, notification);

            return messageId;
        }

        public void ReceiveNotification(string title, string message)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Message = message,
            };
            NotificationReceived?.Invoke(null, args);
        }

        void CreateNotificationChannel()
        {
            manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(Constants.ANDROID_NOTIFICATION_DEFAULT_CHANNEL_NAME);

                var channel = new NotificationChannel(Constants.ANDROID_NOTIFICATION_DEFAULT_CHANNEL_ID, channelNameJava, NotificationImportance.Default)
                {
                    Description = Constants.ANDROID_NOTIFICATION_DEFAULT_CHANNEL_DESC
                };
                manager.CreateNotificationChannel(channel);
            }

            channelInitialized = true;
        }
    }
}
