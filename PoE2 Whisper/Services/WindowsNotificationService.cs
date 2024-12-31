using Microsoft.Toolkit.Uwp.Notifications;


namespace Poe2Notify.Services
{
    internal class WindowsNotificationService
    {



        public void ShowNotification(string title, string message)
        {
            new ToastContentBuilder()
            .AddArgument("action", "viewConversation")
            .AddArgument("conversationId", 9813)
            .AddText("PoE2 Whisper")
            .AddAudio(new ToastAudio())
            .AddText(message)
            .Show();
        }



    }
}
