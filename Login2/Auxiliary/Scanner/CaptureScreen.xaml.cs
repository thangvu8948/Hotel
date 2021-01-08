
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using DarrenLee.Media;
using Login2.Auxiliary.DomainObjects;
using Login2.Auxiliary.Helpers;
using Login2.Auxiliary.WebAPIRequest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Login2.Auxiliary.Scanner
{
    /// <summary>
    /// Interaction logic for CaptureScreen.xaml
    /// </summary>
    public partial class CaptureScreen : Window
    {
        Camera myCam = new Camera();
        IWebApiRequest webApiRequest;
        private TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();
        const int defaultLife = 6;
        public Task<object> Fetch()
        {
            return _tcs.Task;
        }
        private int counter;
        public CaptureScreen(int? lifetime, IWebApiRequest webApi)
        {
            webApiRequest = webApi;
            InitializeComponent();
            counter = lifetime == null ? defaultLife : lifetime.Value;
        }
        private void myCam_OnFrameArrived(object source, FrameArrivedEventArgs e)
        {
            Image img = e.GetFrame();
            pictureBoxLoading.Image = img;

        }

        private void GetInfo()
        {
            var cameraDevices = myCam.GetCameraSources();
            var cameraResolution = myCam.GetSupportedResolutions();
            TypeCamera.Items.Clear();
            foreach (var d in cameraDevices)
            {
                TypeCamera.Items.Add(d);
            }
            Resolution.Items.Clear();
            foreach (var r in cameraResolution)
            {
                Resolution.Items.Add(r);
            }
            TypeCamera.SelectedIndex = 0;
            Resolution.SelectedIndex = 0;
            var resolution = ExtraFunction.getWH(cameraResolution[0]);
            pictureBoxLoading.Width = resolution[0];
            pictureBoxLoading.Height = resolution[1];
            Scan.Visibility = Visibility.Visible;
        }

        private void TypeCamera_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            myCam.ChangeCamera(TypeCamera.SelectedIndex);
        }

        private void Resolution_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            myCam.Start(Resolution.SelectedIndex);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myCam.Stop();
        }

        //public void Capture_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //private void RunTopToBottom()
        //{

        //}
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetInfo();
            myCam.OnFrameArrived += myCam_OnFrameArrived;
            //Start();
        }
        private System.Windows.Forms.Timer timer;
        private void Start()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1000; // 1 second
            timer.Start();
            Countdown.Content = counter.ToString();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            counter--;
            if (counter == 1)
            {
                timer.Stop();
                var filepath = $"{Guid.NewGuid()}.jpg";
                pictureBoxLoading.Image.Save(filepath, ImageFormat.Jpeg);
                //var filepath = @"D:\cmt.jpg";
                var temp = webApiRequest.Post("idr/vnm", filepath);
                var response = JsonConvert.DeserializeObject<dynamic>(temp);
                File.Delete(filepath);
                TryAgian(response.errorMessage.Value, response.errorCode.Value!=0?null: response.data.First);
            }
            Countdown.Content = counter.ToString();
        }
        private void TryAgian(string message,dynamic data)
        {
            double exact = 0;
            if (data!=null)
            {
                exact = (
                    0 
                    + Double.Parse(data.id_prob.Value)*(data.id.Value == null?0:1) 
                    + Double.Parse(data.name_prob.Value) * (data.name.Value == null ? 0 : 1)
                    + Double.Parse(data.dob_prob.Value) * (ExtraFunction.ValidDateTime(data.dob.Value)==false ? 0 : 1)
                    + Double.Parse(data.address_prob.Value) * (data.address.Value == null ? 0 : 1)
                    ) / 4;
            }
           
            //object a = JsonConvert.DeserializeObject<object>((string)param);
          
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"{message} \n Exactly :{exact}% (<75% -> Let's retry) \n Do you Try Again?", "Confirmation", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                counter = defaultLife;
                //Window_Loaded(null, null);
                Start();
            }
            else
            {
                _tcs.SetResult(data);
                myCam.Stop();
                this.Close();
            }
        }

        private void Scan_Click(object sender, RoutedEventArgs e)
        {
            Start();
            Scan.Visibility = Visibility.Hidden;
        }
    }
}