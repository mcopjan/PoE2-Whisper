using PushbulletSharp.Models.Requests;
using PushbulletSharp;

namespace PoE2_Whisper.Services
{
    internal class PushBulletService
    {

        public void ShowNotification(string apiToken, string deviceName, string message)
        {
            var client = new PushbulletClient(apiToken);

            var device = client.CurrentUsersDevices().Devices.FirstOrDefault(d => d.Nickname.Equals(deviceName));

            if (device != null)
            {
                try
                {
                    PushNoteRequest request = new PushNoteRequest
                    {
                        DeviceIden = device.Iden,
                        Title = "PoE2 Whisper",
                        Body = message
                    };

                    var response = client.PushNote(request);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred:");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        
    }
}
