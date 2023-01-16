using Android.App;
using Android.Util;
using Android.Views;
using Java.Lang;
using SharedLibrary;
using System.Net.WebSockets;
using System.Text;

namespace IdleBallBotClient
{
    public partial class MainPage : ContentPage
    {
        IServiceTest Services;

        public MainPage(IServiceTest Services_)
        {
            InitializeComponent();
            //ToggleAccelerometer();
            Services = Services_;

            Task.Run( async () => {
                Log.Debug("Connect", "Before Delay");
                await Task.Delay(10000);
                Log.Debug("Connect", "After Delay");
                using (var ws = new ClientWebSocket())
                {
                    await ws.ConnectAsync(new Uri("ws://192.168.0.11:6666/ws"), CancellationToken.None);
                    Log.Debug("Connect", "Connected");

                    WebSocketHandler handler = new WebSocketHandler(ws);
                    while (ws.State == WebSocketState.Open)
                    {

                    }

                }
            });
            
        }

        //method to start manually foreground service
        private void OnServiceStartClicked(object sender, EventArgs e)
        {
            Services.Start();
        }

        //method to stop manually foreground service
        private void Button_Clicked(object sender, EventArgs e)
        {
            Services.Stop();
        }

        //method to work with accelerometer
        public void ToggleAccelerometer()
        {
            if (Accelerometer.Default.IsSupported)
            {
                if (!Accelerometer.Default.IsMonitoring)
                {
                    Accelerometer.Default.ReadingChanged += Accelerometer_ReadingChanged;
                    Accelerometer.Default.Start(SensorSpeed.UI);
                }
                else
                {
                    Accelerometer.Default.Stop();
                    Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
                }
            }
        }

        //on accelerometer property change we call our service and it would send a message
        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            Services.Start(); //this will never stop until we made some logic here
        }
    }
}