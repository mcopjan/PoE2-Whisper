# PoE2 Whisper

This is a .NET 8 application that notifies on any incoming PoE whispers. It is published as self-contained hence does not require .NET runtime to be installed on the device.

![Main Window](readme001.png)

At the moment it supports:

*   Windows notifications
    ![Windows notification](readme003.png)
*   Phone push notifications via Pushbullet service
    ![Phone push notification](readme002.png)

### How to setup Pushbullet service

Here's how to set up the Pushbullet service to receive notifications:

1.  **Login with your Google account:** Go to [https://www.pushbullet.com/](https://www.pushbullet.com/) and log in with your Google account.

2.  **Generate a token:**
    *   Navigate to "My Account" -> "Settings" -> "My Tokens".
    *   Click "Create Access Token".
    *   Copy the generated access token.

<div style="display: flex; align-items: flex-start;">
    <img src="readme004.png" alt="Pushbullet Token Generation" width="400">
    <div style="margin-left: 20px;">
        This screenshot shows how to generate a Pushbullet access token.
    </div>
</div>

3.  **Install the Pushbullet app:** Install the Pushbullet app on your phone and log in with the *same* Google account.

4.  **Configure PoE2 Whisper:**
    *   Open the PoE2 Whisper application.
    *   Go to the "Settings" page.
    *   Use the phone's name (found in Pushbullet "My Account" -> "Settings" -> "Devices") together with the generated token in the PoE2 Whisper settings.