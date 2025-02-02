---
title: "Game State Integration"
weight: 5
header_menu: false
---

---

### {{< icon name="file-lines" >}} Configuration file

The application uses the Game State Integration feature of Dota 2 to get the game time and other information.

When you launch the application, it will automatically install the game state integration configuration into your Dota 2 folder. 

If you run into issues, you can verify by navigating to the 'Integration' tab in the settings window, it has options to uninstall/install and open the folder.

It will open a file directory window at `..\steamapps\common\dota 2 beta\game\dota\cfg\gamestate_integration`:

You should see a file called `gamestate_integration_dota2_helper.cfg` with the following content:

```plaintext
"Dota 2 Integration Configuration"
{
    "uri"           "http://localhost:4001/"
    "timeout"       "5.0"
    "buffer"        "0.1"
    "throttle"      "0.1"
    "heartbeat"     "1.0"
    "data"
    {
        "provider"      "0"
        "map"           "1"
        "player"        "0"
        "hero"          "0"
        "abilities"     "0"
        "items"         "0"
    }
}
```
**Do not enable other properties, like hero, abilities, items - They're ignored by this tool**

Dota2 will send the game state information to the application when the game is running on the provided URI.

This application runs a small local web server waiting for Dota2 to post the game state information to the helper app, which it uses to calculate the timers.

Valve does not seem to have dedicated GSI documentation for Dota2, but the concept and configuration is similar to Valves CSGO GSI, which is documented [here](https://developer.valvesoftware.com/wiki/Counter-Strike:_Global_Offensive_Game_State_Integration).
