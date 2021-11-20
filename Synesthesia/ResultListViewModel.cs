using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Synesthesia.Models;
using Xamarin.Forms;

namespace Synesthesia
{
    public class ResultListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<StimulusGroup> groups;
        public List<StimulusGroup> Groups
        {
            get { return groups; }

            set
            {
                groups = value;
                OnNotifyPropertyChanged(nameof(StimulusGroup));
            }
        }

        private Thickness marg;
        public Thickness Marg
        {
            get { return marg; }

            set
            {
                marg = value;
                OnNotifyPropertyChanged();
            }
        }

        private void OnNotifyPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
