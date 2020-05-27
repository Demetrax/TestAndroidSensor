using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using _07_XamarinAndroid_SeFaireLesDents.Models;
using System.Threading.Tasks;
using System;
using Android.Media;
using Android.Icu.Text;

namespace _07_XamarinAndroid_SeFaireLesDents
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextView tvAppName;


        private TextView tvAccX;
        private TextView tvAccY;
        private TextView tvAccZ;

        private TextView quaw;
        private TextView quax;
        private TextView quay;
        private TextView quaz;

        private TextView pitch;
        private TextView roll;

        private TextView gyroX;
        private TextView gyroY;
        private TextView gyroZ;

        MediaPlayer mediaPlayer;
        

        Button btPlaySound;

        Button btPlayStream;

        private AccelerometerReader accelerometerReader = new AccelerometerReader();

        private OrientationReader orientationReader = new OrientationReader();

        private GyroscopeReader gyroscopeReader = new GyroscopeReader();

        private int nbShock;

        private double pitchVal, rollVal;

        private bool rollListening;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            nbShock = 0;

            rollListening = false;

            tvAppName = FindViewById<TextView>(Resource.Id.tvAppName);
            tvAppName.Text = "Génial ça marche !";

            tvAccX = FindViewById<TextView>(Resource.Id.tvAccX);
            tvAccY = FindViewById<TextView>(Resource.Id.tvAccY);
            tvAccZ = FindViewById<TextView>(Resource.Id.tvAccZ);

            quaw = FindViewById<TextView>(Resource.Id.quaw);
            quax = FindViewById<TextView>(Resource.Id.quax);
            quay = FindViewById<TextView>(Resource.Id.quay);
            quaz = FindViewById<TextView>(Resource.Id.quaz);

            pitch = FindViewById<TextView>(Resource.Id.pitch);
            roll = FindViewById<TextView>(Resource.Id.roll);

            gyroX = FindViewById<TextView>(Resource.Id.gyroX);
            gyroY = FindViewById<TextView>(Resource.Id.gyroY);
            gyroZ = FindViewById<TextView>(Resource.Id.gyroZ);

            accelerometerReader.ToggleAccelerometer();
            orientationReader.ToggleOrientationSensor();
            gyroscopeReader.ToggleGyroscope();

            Delay5s();

            tvAccX.Text = accelerometerReader.accX.ToString();
            tvAccY.Text = accelerometerReader.accY.ToString();
            tvAccZ.Text = accelerometerReader.accZ.ToString();
            CalculPitchAndRoll();
            //accelerometerReader.ToggleAccelerometer();

            quaw.Text = orientationReader.oriW.ToString();
            quax.Text = orientationReader.oriX.ToString();
            quay.Text = orientationReader.oriY.ToString();
            quaz.Text = orientationReader.oriZ.ToString();

            gyroX.Text = gyroscopeReader.gyroX.ToString();
            gyroY.Text = gyroscopeReader.gyroY.ToString();
            gyroZ.Text = gyroscopeReader.gyroZ.ToString();


            mediaPlayer = MediaPlayer.Create(this, Resource.Raw.Voice01_01);


            btPlaySound = FindViewById<Button>(Resource.Id.btPlaySound);
            btPlaySound.Click += BtPlaySound_Click;
            
            btPlayStream = FindViewById<Button>(Resource.Id.btPlayStream);
            btPlayStream.Click += BtPlayStream_Click;

            startTimer();
        }

        private void CalculPitchAndRoll()
        {
            pitchVal = 180 * Math.Atan(accelerometerReader.accX / Math.Sqrt(accelerometerReader.accY * accelerometerReader.accY + accelerometerReader.accZ * accelerometerReader.accZ)) / Math.PI;
            rollVal = 180 * Math.Atan(accelerometerReader.accY / Math.Sqrt(accelerometerReader.accX * accelerometerReader.accX + accelerometerReader.accZ * accelerometerReader.accZ)) / Math.PI;
            roll.Text = rollVal.ToString();
            pitch.Text = pitchVal.ToString();

            if ((Math.Abs(pitchVal) + Math.Abs(rollVal) > 45) && !rollListening )
            {
                rollListening = true;
                calculateRoll();
            }

        }

        private void calculateRoll()
        {
            Console.WriteLine("startTimerRoll()");

            double initialRoll = (Math.Abs(pitchVal) + Math.Abs(rollVal));
            Console.WriteLine(" initialRoll ======= " + initialRoll);
            DateTime startTime = DateTime.Now;
            System.Timers.Timer TimerRoll = new System.Timers.Timer();
            TimerRoll.Start();
            TimerRoll.Interval = 1000;
            TimerRoll.Enabled = true;
            TimerRoll.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                RunOnUiThread(() => {

                    pitchVal = 180 * Math.Atan(accelerometerReader.accX / Math.Sqrt(accelerometerReader.accY * accelerometerReader.accY + accelerometerReader.accZ * accelerometerReader.accZ)) / Math.PI;
                    rollVal = 180 * Math.Atan(accelerometerReader.accY / Math.Sqrt(accelerometerReader.accX * accelerometerReader.accX + accelerometerReader.accZ * accelerometerReader.accZ)) / Math.PI;

                    double newRoll = (Math.Abs(pitchVal) + Math.Abs(rollVal)) + 30;

                    Console.WriteLine("newRoll ==== " + newRoll + " initialRoll ======= " + initialRoll);
                    if (newRoll < initialRoll)
                    {
                        DateTime stopTime = e.SignalTime;
                        TimeSpan nbTime = stopTime - startTime;
                        TimerRoll.Stop();
                        
                        Console.WriteLine(nbTime.TotalSeconds + " secondes");
                        Console.WriteLine(newRoll +" < "+ initialRoll + "----------------------------------------------- Stop Roll -----------------------------------------");
                        Console.WriteLine("_______________________________________________________________________________________________________________________________________________");
                        Console.WriteLine("_______________________________________________________________________________________________________________________________________________");
                        TimerRoll.Dispose();
                        rollListening = false;
                    } else
                    {
                        //initialRoll = newRoll;
                    }
                
                
                });
            };
         }

        private void BtPlayStream_Click(object sender, EventArgs e)
        {
            var mp3TestFile = "https://archive.org/download/testmp3testfile/mpthreetest.mp3";
            MediaPlayer mediaPlayer2 = new MediaPlayer();
            mediaPlayer2.SetAudioStreamType(Stream.Music);
            mediaPlayer2.SetDataSource(ApplicationContext, Android.Net.Uri.Parse(mp3TestFile));
            mediaPlayer2.Prepare();
            mediaPlayer2.Start();
        }

        private void BtPlaySound_Click(object sender, EventArgs e)
        {
            mediaPlayer.Start();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public async void Delay5s()
        {
            Console.WriteLine("Delay5s()");
            await Task.Delay(3000);
            
        }

        public void DetectedShack()
        {
            mediaPlayer.Start();
            nbShock++;
            accelerometerReader.shacked = false;

            if (nbShock == 1)
            {
                Console.WriteLine("-----------------shock 1-----------------------");
            } else if (nbShock == 2)
            {
                Console.WriteLine("------------------shock 2-----------------------");
            } else if (nbShock >= 3)
            {
                Console.WriteLine("------------------shock 3-----------------------");
            }
        }

        public void startTimer()
        {
            Console.WriteLine("startTimer()");
            System.Timers.Timer Timer1 = new System.Timers.Timer();
            Timer1.Start();
            Timer1.Interval = 300;
            Timer1.Enabled = true;
            Timer1.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                RunOnUiThread(() =>
                {
                    //Console.WriteLine("RunOnUiThread()");
                    if (!accelerometerReader.isStarted)
                    {
                        accelerometerReader.ToggleAccelerometer();
                    }

                    if (!orientationReader.isStarted)
                    {
                        orientationReader.ToggleOrientationSensor();
                    }

                    if (!gyroscopeReader.isStarted)
                    {
                        gyroscopeReader.ToggleGyroscope();
                    }

                    tvAccX.Text = accelerometerReader.accX.ToString();
                    tvAccY.Text = accelerometerReader.accY.ToString();
                    tvAccZ.Text = accelerometerReader.accZ.ToString();

                    CalculPitchAndRoll();

                    quaw.Text = orientationReader.oriW.ToString();
                    quax.Text = orientationReader.oriX.ToString();
                    quay.Text = orientationReader.oriY.ToString();
                    quaz.Text = orientationReader.oriZ.ToString();

                    gyroX.Text = gyroscopeReader.gyroX.ToString();
                    gyroY.Text = gyroscopeReader.gyroY.ToString();
                    gyroZ.Text = gyroscopeReader.gyroZ.ToString();

                    if (accelerometerReader.shacked)
                    {
                        DetectedShack();
                    }
                    //accelerometerReader.ToggleAccelerometer();
                });
            };
        }
    }
}