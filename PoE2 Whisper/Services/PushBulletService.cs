using PushbulletSharp.Models.Requests;
using PushbulletSharp;

namespace PoE2_Whisper.Services
{
    internal class PushBulletService
    {

        public void ShowNotification(string accessToken, string deviceName, string message)
        {
            ThrowIfNullOrEmpty(accessToken, nameof(accessToken));
            ThrowIfNullOrEmpty(deviceName, nameof(deviceName));
            ThrowIfNullOrEmpty(message, nameof(message));

            var client = new PushbulletClient(accessToken);
            var devices = client.CurrentUsersDevices().Devices;
            if (devices.Count.Equals(0)) { throw new Exception("No devices found for provided accessToken!"); }
            var device = devices.FirstOrDefault(d => d.Nickname.Equals(deviceName));

            if (device != null)
            {

                PushNoteRequest request = new PushNoteRequest
                {
                    DeviceIden = device.Iden,
                    Title = "PoE2 Whisper",
                    Body = message
                };

                client.PushNote(request);
            }
            else
            {
                throw new Exception($"Device with name {deviceName} was not found but other devices were detected: {string.Join(',', devices.Select(d => d.Nickname))} ");
            }
        }


        private void ThrowIfNullOrEmpty(string argument, string argumentName)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(argumentName, $"{argumentName} cannot be null or empty.");
            }
        }


    }
}
