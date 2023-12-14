# Game State Integration

Go to `..\steamapps\common\dota 2 beta\game\dota\cfg\gamestate_integration` and add the following file: 

`gamestate_integration_timers.cfg`

```
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

# Run

Run the `Dota2Helper.Desktop.exe`