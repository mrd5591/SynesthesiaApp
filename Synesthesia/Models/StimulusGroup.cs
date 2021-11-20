using System;
using System.Collections.Generic;

namespace Synesthesia.Models
{
    public class StimulusGroup
    {
        public StimulusGroup(string name)
        {
            Name = name;
            Stimuli = new List<StimulusResult>();
        }

        public string Name { get; set; }

        public List<StimulusResult> Stimuli { get; set; }

        public double Score { get; set; }
    }
}
