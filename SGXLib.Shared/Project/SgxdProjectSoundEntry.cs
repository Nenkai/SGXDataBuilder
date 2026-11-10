//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/SGXDataBuilder
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace SGXLib.Project
{
    public class SgxdProjectSoundEntry
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("loop_sample_start")]
        public int SampleStart { get; set; }

        [JsonPropertyName("loop_sample_end")]
        public int SampleEnd { get; set; }
    }
}
