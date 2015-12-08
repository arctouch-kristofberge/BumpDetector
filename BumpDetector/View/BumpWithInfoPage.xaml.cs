namespace BumpDetector.View
{
    using BumpDetector.ViewModel;

    using Xamarin.Forms;

    public partial class BumpWithInfoPage : ContentPage
    {
        public BumpWithInfoPage()
        {
            InitializeComponent();

            BindingContext = new BumpWithInfoViewModel();
        }
    }
}