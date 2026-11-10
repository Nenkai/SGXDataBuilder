//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/SGXDataBuilder
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace SGXLib.Project
{
    public class SgxdProject
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("split_body")]
        public bool SplitBody { get; set; }

        [JsonPropertyName("entries")]
        public List<SgxdProjectSoundEntry> SgxdProjectSoundEntries { get; set; } = new List<SgxdProjectSoundEntry>();
    }
}
