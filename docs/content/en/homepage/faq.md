---
title: 'F.A.Q'

# The "header_menu_title" value will be used as text for header buttons.
# The "title" value will be used if value for "header_menu_title" is not provided.
#header_menu_title: 'Short Menu Title'

# The "navigation_menu_title" value will be used as text for fixed menu items.
# The "title" value will be used if value for "navigation_menu_title" is not provided.
#navigation_menu_title: 'Short Menu Title'

# The "weight" will determine where this section appears on the "homepage".
# A bigger weight will place the content more towards the bottom of the page.
# It's like gravity ;-).
weight: 50

# If "header_menu" is true, then a button linking to this section will be placed
# into the header menu at the top of the homepage.
header_menu: false
---

---


#### Where is the installer?

There is no installer. Unzip the file and run the executable `Dota2Helper.Desktop.exe`

#### Where are the settings stored?

The settings are stored in the file `appsettings.json` in the same folder as the executable.

#### Does it support all platforms?

Only Windows is supported. There are no plans to support other platforms at this time.

#### Is the software free to use?

Yes, the software is free to use.

#### How does GSI work?

GSI is a feature of the DOTA 2 Game Engine that allows third-party applications to receive information about the game you are currently playing. This information is sent to the application in real-time, allowing it to provide additional features to the user. In the case of Dota 2 Helper, GSI is used to track the game timer and provide additional information to the user.

#### I'm not seeing any timers, what's wrong?

Check that the port is not being used by another process, that the -gamestateintegration flag is set in the launch options of DOTA 2 in Steam, and that you have a profile selected with enabled timers.

#### I am only seeing Demo timers, what's wrong?

Open the settings and ensure Timer mode is set to 'Auto' or 'Game', not 'Demo'

#### Can I use my own audio files for the timers?

Yes, you can use your own audio files. mp3 and wav are supported.

#### Will I get banned?

Using this software does not violate the Steam Subscriber Agreement or the Dota 2 terms of service. It does not modify the game in any way. It's like having a coach sitting next to you, providing you with additional information to help you play better.

No, this software does not modify the game in any way. It simply utilises the game engines built in features to provide additional information to the user, using the `-gamestateintegration` feature. Many existing popular tools use this feature, such as Overwolf, SteelSeries GameSense, and others.