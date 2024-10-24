﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dota2Helper.Core.Configuration;

[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(List<TimerOptions>))]
[JsonSerializable(typeof(TimerOptions))]
[JsonSerializable(typeof(Settings))]
[JsonSerializable(typeof(Uri))]
public partial class JsonContext : JsonSerializerContext
{
    public static JsonContext Custom { get; } = new(new JsonSerializerOptions()
        {
            WriteIndented = true,
        }
    );
}