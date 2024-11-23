# Dota 2 Helper

[![Build](https://github.com/pjmagee/dota2-helper/actions/workflows/build.yaml/badge.svg)](https://github.com/pjmagee/dota2-helper/actions/workflows/build.yaml) ![Latest Release](https://img.shields.io/badge/dynamic/json?label=Latest%20release&query=$.tag_name&url=https%3A%2F%2Fapi.github.com%2Frepos%2Fpjmagee%2Fdota2-helper%2Freleases%2Flatest&style=flat&color=blue&style=for-the-badge) ![Dagger](https://img.shields.io/badge/dynamic/json?url=https://raw.githubusercontent.com/pjmagee/dota2-helper/main/dagger.json&label=Dagger&query=%24.engineVersion&link=https%3A%2F%2Fgithub.com%2Fdagger%2Fdagger&logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIGlkPSJDYWxxdWVfNSIgdmlld0JveD0iMCAwIDIyMS4xMDIgMjIxLjEwMiI%2BPGRlZnM%2BPHN0eWxlPi5jbHMtNHtmaWxsOiNiZTFkNDN9LmNscy01e2ZpbGw6IzEzMTIyNn0uY2xzLTZ7ZmlsbDojNDBiOWJjfTwvc3R5bGU%2BPC9kZWZzPjxjaXJjbGUgY3g9IjExMC41NTEiIGN5PSIxMTAuNTUxIiByPSIxMTAuNTI1IiBjbGFzcz0iY2xzLTUiLz48Y2lyY2xlIGN4PSIxMTAuNTUxIiBjeT0iMTEwLjU1MSIgcj0iOTQuODMzIiBjbGFzcz0iY2xzLTUiIHRyYW5zZm9ybT0icm90YXRlKC00NSAxMTAuNTUgMTEwLjU1MSkiLz48cGF0aCBkPSJNMTcuNTQ4IDkxLjk0NGE5NS4yODggOTUuMjg4IDAgMCAwLTEuNjk5IDIzLjYxNGM1LjgyOC4xMDEgMTAuODUxLTEuMDggMTQuMzQxLTQuMTIyIDE3LjMxNi0xNS4wOTMgNTAuMDg4LTEwLjgxMyA1MC4wODgtMTAuODEzcy0xMC4wNjQtMTcuMzczLTYyLjczLTguNjc5eiIgY2xhc3M9ImNscy02Ii8%2BPHBhdGggZD0iTTI2LjgxOSA2Ni4wMDJjNDkuMDUzLTE1LjA4NyA2MS42MDkgMTAuMTQgNjQuMjM0IDE4LjI4MiAwIDAgMy4wMDMtNDYuMDUyLTM0Ljc3OC01MS41YTk1LjI2NyA5NS4yNjcgMCAwIDAtMjkuNDU2IDMzLjIxOXoiIGNsYXNzPSJjbHMtNCIvPjxwYXRoIGZpbGw9IiNlZjdiMWEiIGQ9Ik0xMTAuMTY3IDcxLjExOHM1LjEzMy0zMi4yODQgMTQuMjI5LTU0LjM5YTk1LjYyOSA5NS42MjkgMCAwIDAtMjguNDE0LjEwNWM5LjA2OSAyMi4xMDQgMTQuMTg1IDU0LjI4NiAxNC4xODUgNTQuMjg2eiIvPjxwYXRoIGQ9Ik0xNjQuNzA0IDMyLjY5OGMtMzguNDU2IDUuMDE3LTM1LjQyMiA1MS41ODYtMzUuNDIyIDUxLjU4NiAyLjY0MS04LjE5MiAxNS4zMzYtMzMuNjg0IDY1LjE1MS0xNy45OTdhOTUuMjggOTUuMjggMCAwIDAtMjkuNzI5LTMzLjU4OXoiIGNsYXNzPSJjbHMtNCIvPjxwYXRoIGQ9Ik0yMDMuNTc5IDkyLjA3NGMtNTMuMzYzLTkuMDAzLTYzLjUyMiA4LjU1LTYzLjUyMiA4LjU1czMyLjc3Mi00LjI4IDUwLjA4OCAxMC44MTNjMy42NDIgMy4xNzQgOC45NTUgNC4zMTkgMTUuMTA5IDQuMDk4YTk1LjMzNyA5NS4zMzcgMCAwIDAtMS42NzUtMjMuNDYxeiIgY2xhc3M9ImNscy02Ii8%2BPHBhdGggZmlsbD0iI2ZjYzAwOSIgZD0iTTExMC41NTEgMjA1LjM4NGMyLjYzNyAwIDUuMjQ4LS4xMTMgNy44My0uMzI0LTYuMjU3LTEwLjg1Ny02LjYwOC0yNy4zODgtNi42MDgtMzcuNjE0IDAgMCA2LjI3MSAyNy44OTggMTMuOTE3IDM2LjcyOSAyNS41NC00LjA5OCA0Ny42NzItMTguMzkgNjIuMDg3LTM4LjU3NS0yLjY0Ny0yLjUyMy04LjQ0Ny01Ljk3MS0xNi4zNTkgMS4yMDUgMCAwIDMuNDgyLTEzLjc2IDE5LjExNS03LjMyOS0yLjg4Ny03LjY2NS0xMC41MzMtMTIuOTk0LTE5LjM1Mi0xMi4zMTgtOS4zNjkuNzE5LTE2Ljk1NSA4LjQyNi0xNy41NTMgMTcuODAzLS4zMSA0Ljg2OCAxLjIyNCA5LjM1NyAzLjk0NSAxMi44ODQtNC45MTcuOTMxLTkuMTU5IDMuNzQ3LTExLjk2MyA3LjY2OS0uNzM2LTQuNTQyLTQuNjY1LTguMDE0LTkuNDE1LTguMDE0YTkuNDg5IDkuNDg5IDAgMCAwLTUuNzIgMS45MTZjLTIuMTExLTEuNzMxLTQuNDEzLTQuNDMyLTYuNzAxLTcuODY0LTYuMDMzLTkuMDQ5LTguNjQ2LTIyLjgyNi05LjY0My0zMC43Mi0uMjUzLTEuOTk4LTEuOTUtMy40OS0zLjk2NC0zLjQ5cy0zLjcxMSAxLjQ5MS0zLjk2NCAzLjQ5Yy0uOTk3IDcuODk0LTQuMzAxIDIxLjY3MS0xMC4zMzUgMzAuNzItNy42OTYgMTEuNTQ0LTEzLjA4MyAxMi42OTktMTYuMjkgMGwtLjA1OS4wMTRjLTIuNjk3LTkuNTUyLTExLjQ1Ny0xNi41Ni0yMS44NzMtMTYuNTYtOS43MTggMC0xNy45NzUgNi4xMDktMjEuMjI3IDE0LjY4NWE5NS4zMjcgOTUuMzI3IDAgMCAwIDEwLjkwMSAxMS41MzFjMTEuODQ3LTEwLjIzNyAyMS44NjggMy42NjkgMjEuODY4IDMuNjY5LTguNTIxLTMuOTEtMTQuMzUzLTIuODUtMTguMTg2LS41MzggMTEuMzgzIDkuMTk2IDI0LjkzNCAxNS44MTIgMzkuNzY5IDE4Ljk2IDYuMTkxLTcuMTE1IDEyLjE1MS0yMC4zNDUgMTIuMTUxLTIwLjM0NSAwIDQuNDgzLTIuNTggMTQuOTAyLTYuNDAxIDIxLjM4MmE5NS41NzMgOTUuNTczIDAgMCAwIDE0LjAyOSAxLjAzNXoiLz48cGF0aCBmaWxsPSIjZmZmIiBkPSJNMTEzLjIwOCA4MS43OTVhNC4wNzggNC4wNzggMCAwIDAtMy4wNC0xLjM1MiA0LjA4MSA0LjA4MSAwIDAgMC0zLjA0IDEuMzUyYy0yMS40ODIgMjMuNzg5LTI2Ljc1MSA1NS43MjgtMjAuNTc1IDU5Ljk3NCA2LjQ1OSA0LjQ0IDE0LjUzMi0xNC45MzIgMjMuMjExLTE1LjEzM2guODA4YzguNjc4LjIwMiAxNi43NTIgMTkuNTc0IDIzLjIxMSAxNS4xMzMgNi4xNzctNC4yNDcuOTA4LTM2LjE4Ni0yMC41NzUtNTkuOTc0em0tMy4wNCAzMS41NjhhNi4zNyA2LjM3IDAgMSAxIDAtMTIuNzQgNi4zNyA2LjM3IDAgMCAxIDAgMTIuNzR6Ii8%2BPC9zdmc%2B) [![Github Latest Releases](https://img.shields.io/github/downloads/pjmagee/dota2-helper/total.svg?label=Total%20downloads)]()

An objective timer tracker with audio notifications for Dota2. This application is designed to help players keep track of important in-game events such as stacking, power runes, bounty runes, and more. The application is designed to be used in conjunction with the Game State Integration feature of Dota2.

## Support

If you find this application useful, please consider supporting me by buying me a coffee.

[![Buy Me A Coffee](https://cdn.buymeacoffee.com/buttons/default-orange.png)](https://www.buymeacoffee.com/pjmagee)

## Platform

I do not own a Mac or Linux machine to test the application, so only Windows is supported at this time.  

## Features

- An optimised timers view to overlay on top of Dota 2
- Customisable behaviour for each timer such as audio, show/hide
- Playing audio notifications for each timer
- Dark & light mode themes (dire and radiant style)
- Manual reset for dynamic objectives (e.g. Tormentors, Roshan)
- Profiles for additional customisation, e.g. support heroes, pos, or role. 
- Fake timers are used when the game is not running for easy configuration
- When the game is running, the timers will automatically start based on the game time
- Automatic installation of the game state integration configuration into your Dota 2 folder

## (GSI) Game State Integration

The application uses the Game State Integration feature of Dota 2 to get the game time and other information.

When you launch the application, it will automatically install the game state integration configuration into your Dota 2 folder. If you navigate to the 'Integration' tab in the settings window, it has options to uninstall/install and open the folder.

You can go to `..\steamapps\common\dota 2 beta\game\dota\cfg\gamestate_integration`:

You should be able to see a file created called `gamestate_integration_dota2_helper.cfg` with the following content:

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
Dota2 will send the game state information to the application when the game is running on the provided URI.

This application runs a small local web server waiting for Dota2 to post the game state information to the helper app, which it uses to calculate the timers.

Valve does not seem to have dedicated GSI documentation for Dota2, but the concept and configuration is similar to Valves CSGO GSI, which is documented [here](https://developer.valvesoftware.com/wiki/Counter-Strike:_Global_Offensive_Game_State_Integration).

## How to?

1. Visit the [releases page](https://github.com/pjmagee/dota2-helper/releases).
2. Download the latest version e.g. `Dota2Helper_v1.0.4_windows_amd64.zip`
3. Extract the zip file which contains the full application.
4. Run the `Dota2Helper.Desktop.exe`


### Preview

https://github.com/user-attachments/assets/505a08f3-24fe-4d41-a8b3-b867670b5910

### Screenshots

<details>
<summary>Profiles</summary>

![profiles](./docs/settings-profiles.png)
</details>

<details>

<summary>Timers</summary>

![timers](./docs/settings-timers.png)
</details>

<details>
<summary>Overlay</summary>

![overlay](./docs/timers-window.png)
</details>
