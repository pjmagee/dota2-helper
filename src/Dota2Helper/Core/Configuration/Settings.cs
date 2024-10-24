﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dota2Helper.Core.Configuration;

public class Settings
{
    [JsonPropertyName("Timers")]
    public required List<TimerOptions> Timers { get; set; }
}