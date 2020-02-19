using Android.Content;
using Xamarin.Forms;

namespace NotificationsTest.Droid
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            DependencyService.Get<INotificationManager>().ScheduleNotification(Constants.NOTIFICATION_TITLE, Constants.NOTIFICATION_TEXT);
        }
    }
}
