namespace BrailleDevTools
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";            

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private async void Button1_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("資訊", $"{DateTime.Now}", "知道了");

        }


    }
}