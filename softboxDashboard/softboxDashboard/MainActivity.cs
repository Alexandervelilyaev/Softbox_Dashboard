using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace softboxDashboard
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        const string CMD_CONNECT = "connect";
        const string CMD_ON = "on";
        const string CMD_OFF = "off";

        bool IsTurnedOn = false;

        TcpClient client;

        SeekBar redBar;
        SeekBar greenBar;
        SeekBar blueBar;

        Switch onOffSwitch;

        TextView redBarText;
        TextView greenBarText;
        TextView blueBarText;

        FloatingActionButton connectButton;

        Spinner ipSpinner;

        int r = 0;
        int g = 0;
        int b = 0;

        private void SendCommand(string command)
        {
            NetworkStream tcpStream = client.GetStream();
            byte[] sendBytes = Encoding.UTF8.GetBytes(command);
            tcpStream.Write(sendBytes, 0, sendBytes.Length);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            onOffSwitch = FindViewById<Switch>(Resource.Id.onOffSwitch);

            redBar = FindViewById<SeekBar>(Resource.Id.seekBarRed);
            greenBar = FindViewById<SeekBar>(Resource.Id.seekBarGreen);
            blueBar = FindViewById<SeekBar>(Resource.Id.seekBarBlue);

            ipSpinner = FindViewById<Spinner>(Resource.Id.ipSpinner);

            client = new TcpClient();

            connectButton = FindViewById<FloatingActionButton>(Resource.Id.connectButton);
            
            connectButton.Click += delegate
            {
                try
                {
                    connectButton.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Orange);
                    client.Connect("192.168.10.7", 80);
                    connectButton.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Green);
                }
                catch (Exception e)
                {

                }
            };

            onOffSwitch.TextOff = "OFF";
            onOffSwitch.TextOn = "ON";
            onOffSwitch.Checked = false;
            onOffSwitch.CheckedChange += onSwitchChanged;

            redBarText = FindViewById<TextView>(Resource.Id.txtViewRed);
            greenBarText = FindViewById<TextView>(Resource.Id.txtViewGreen);
            blueBarText = FindViewById<TextView>(Resource.Id.txtViewBlue);

            redBar.ProgressChanged += delegate
            {
                r = redBar.Progress;
                redBarText.Text = r.ToString();
                SendCommand("red: " + redBar.Progress);

            };
            
            greenBar.ProgressChanged += delegate
            {
                g = greenBar.Progress;
                greenBarText.Text = g.ToString();
                SendCommand("green: " + greenBar.Progress);
            };

            blueBar.ProgressChanged += delegate
            {
                b = blueBar.Progress;
                blueBarText.Text = b.ToString();
                SendCommand("blue: " + blueBar.Progress);
            };
        }

        public void onSwitchChanged(object sender, EventArgs eventArgs)
        {
            NetworkStream tcpStream = client.GetStream();
            byte[] sendBytes = Encoding.UTF8.GetBytes(IsTurnedOn ? CMD_ON : CMD_OFF);
            tcpStream.Write(sendBytes, 0, sendBytes.Length);

            //onOffButton.Text = IsTurnedOn ? "On" : "Off";
            IsTurnedOn = !IsTurnedOn;

            if (IsTurnedOn)
            {
                r = g = b = 255;
                redBar.Progress = greenBar.Progress = blueBar.Progress = 255;
            }
            else
            {
                r = g = b = 0;
                redBar.Progress = greenBar.Progress = blueBar.Progress = 0;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

		public void ScannningNetworkAsync(IPAddress myIp)
        {
            List<String> FoundedAddressesList = new List<String>();
            for (int i = 1; i < 256; i++)
            {
                var ip = IPAddress.Parse("192.168." + Convert.ToString(myIp.GetAddressBytes()[2]) + "." + Convert.ToString(i));
                if (!ip.Equals(myIp))
                {
                    var pingSender = new Ping();


                    //todo: uncomment
                    //pingSender.PingCompleted += Program.EntryForm.pingSender_Complete;
                    pingSender.SendAsync(ip.ToString(), 5, Encoding.ASCII.GetBytes("test"),
                        new PingOptions(5, true));
                }
            }
        }
    }
}

