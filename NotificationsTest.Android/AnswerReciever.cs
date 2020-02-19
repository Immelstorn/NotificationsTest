using Android.App;
using Android.Content;
using AndroidApp = Android.App.Application;

namespace NotificationsTest.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Constants.ANDROID_NOTIFICATION_ACTION })]
    public class AnswerReciever : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);
            var result = intent.GetBooleanExtra(Constants.ANDROID_NOTIFICATION_EXTRA_ACTION, false);
            var date = intent.GetStringExtra(Constants.ANDROID_NOTIFICATION_EXTRA_DATE);
            var id = intent.GetIntExtra(Constants.ANDROID_NOTIFICATION_EXTRA_ID, -1);
            manager.Cancel(id);

            //do something with data
        }
    }
}
