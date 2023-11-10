using PortableAssistant.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace PortableAssistant.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}