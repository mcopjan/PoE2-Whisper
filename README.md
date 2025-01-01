# PoE2 Whisper

- This is a.net8 application that notifies on any incoming PoE whispers. It is published as self-contained hence does not require .net runtime to be installed on the device
![Main Window](readme001.png)

At the moment it supports
- Windows notifications 
![Windows notification](readme003.png)
- Phone push notifications via Push bullet service 
![Phone push notification](readme002.png)


How to setup Pushbullet service
 - Login with your google account
 - Navigate to My Account -> Settings -> My Tokens and generate a token 
 - Install Pushbullet app in your phone and login with the same google account
 - Use phone name from My Account -> Settings -> Devices together with the token in the Settings page
 ![Settings](readme004.png)