using System.Linq;
using System.Collections.Generic;
using Synesthesia.Models;
using Xamarin.Forms;
using Xamarin.Forms.MultiSelectListView;

namespace Synesthesia
{
    public partial class StartPage : ContentPage
    {
        public MultiSelectObservableCollection<GroupItem> Groups { get; }

        public StartPage()
        {
            InitializeComponent();

            Groups = new MultiSelectObservableCollection<GroupItem>();

            GroupItem item = new GroupItem();
            item.Name = "Letters";
            Groups.Add(item);

            item = new GroupItem();
            item.Name = "Numbers";
            Groups.Add(item);

            item = new GroupItem();
            item.Name = "Days of the Week";
            Groups.Add(item);

            item = new GroupItem();
            item.Name = "Months";
            Groups.Add(item);

            BindingContext = this;
        }

        async void StartTestButton_Clicked(System.Object sender, System.EventArgs e)
        {
            bool letters = false;
            bool numbers = false;
            bool dow = false;
            bool months = false;

            foreach(SelectableItem<GroupItem> item in Groups)
            {
                if(item.IsSelected)
                {
                    string name = ((GroupItem)item.Data).Name;
                    switch(name)
                    {
                        case "Letters":
                            letters = true;
                            break;

                        case "Numbers":
                            numbers = true;
                            break;

                        case "Days of the Week":
                            dow = true;
                            break;

                        case "Months":
                            months = true;
                            break;
                    }
                }
            }

            if(letters || numbers || dow || months)
            {
                await Navigation.PushAsync(new MainPage(letters, numbers, dow, months));
            } else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Select at least one stimulus group", "OK");
            }
        }
    }
}
