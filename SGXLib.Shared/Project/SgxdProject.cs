using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SGXLib.Shared.Project
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
