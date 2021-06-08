using System;
using System.Collections.Generic;

namespace Synesthesia.Models
{
    public class SubmitJsonPackage
    {
        public Dictionary<string, ColorJson> colors { get; set; }
        public string email { get; set; }
        public string fileType { get; set; }
    }
}
