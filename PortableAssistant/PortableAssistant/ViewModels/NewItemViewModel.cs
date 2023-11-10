using PortableAssistant.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PortableAssistant.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        private string name;
        private string publicName;
        private string description;
        private int talkingCoeff;
        private bool includeInForming;

        public NewItemViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(name)
                && !String.IsNullOrWhiteSpace(publicName);
        }

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

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            Person newItem = new Person()
            {
                Name = Name,
                PublicName = PublicName,
                Description = Description,
                TalkingCoef = TalkingCoeff,
                IncludeInForming = IncludeInForming,
            };

            await PersonDataStore.AddItemAsync(newItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
