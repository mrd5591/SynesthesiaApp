using System.Collections.Generic;
using Xamarin.Forms;

namespace Synesthesia.Models
{
    public class StimulusResult
    {
        public StimulusResult(string name)
        {
            Name = name;
            Colors = new List<Color>();
        }

        public string Name { get; set; }

        public List<Color> Colors { get; set; }
    }
}
