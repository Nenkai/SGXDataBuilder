using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SGXLib.Shared.Project
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
