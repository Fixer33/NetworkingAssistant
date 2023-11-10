using PortableAssistant.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PortableAssistant.ViewModels
{
    [QueryProperty(nameof(PersonName), nameof(PersonName))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private string name;
        private string publicName;
        private string description;
        private int talkingCoeff;
        private bool includeInForming;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string PublicName
        {
            get => publicName;
            set => SetProperty(ref publicName, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public int TalkingCoeff
        {
            get => talkingCoeff;
            set => SetProperty(ref talkingCoeff, value);
        }

        public bool IncludeInForming
        {
            get => includeInForming;
            set => SetProperty(ref includeInForming, value);
        }

        public string PersonName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await PersonDataStore.GetItemAsync(itemId);
                Name = item.Name;
                PublicName = item.PublicName;
                Description = item.Description;
                TalkingCoeff = item.TalkingCoef;
                IncludeInForming = item.IncludeInForming;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
