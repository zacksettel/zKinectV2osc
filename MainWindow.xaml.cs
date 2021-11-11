namespace zKinectV2OSC
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using Model.Drawing;
    using Model.Network;
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    /// 



    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DrawingImage imageSource;
        private KinectSensor kinectSensor;
        private BodyFrameReader bodyFrameReader;
        private Body[] bodies;
        private FrameTimer timer;
        private KinectCanvas kinectCanvas;
        private BodySender bodySender;
        private Double _lastFrameSecs = 0;
        private int oscMessUpdateMS = 80;   // OSC messagess no faster than this
        private float depthClip = 4.5f; // the kinect will refuse to handle bodies further back that this value
        private bool jointTX = true;  // toggles oscTX of joint data
 
        public event PropertyChangedEventHandler PropertyChanged;

        public ImageSource ImageSource
        {
            get { return this.imageSource; }
        }

        private string framesText;
        public string FramesText
        {
            get { return this.framesText; }
            set
            {
                this.framesText = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("FramesText"));
                }
            }
        }

        private string uptimeText;
        public string UptimeText
        {
            get { return this.uptimeText; }
            set
            {
                this.uptimeText = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("UptimeText"));
                }
            }
        }

        private string oscText;
        public string OscText
        {
            get { return this.oscText; }
            set
            {
                this.oscText = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("OscText"));
                }
            }
        }

        // Define a class to receive parsed values
        class Options
        {
            [Option('p', "port", DefaultValue = "54321",
              HelpText = "oscTX port")]
            public string Port { get; set; }

            [Option('i', "ip", DefaultValue ="",
              HelpText = "oscTX address")]
            public string IpAddr { get; set; }

            [Option('r', "oscUpdateRateMs", DefaultValue = 50,
              HelpText = "osc update rate im milliseconds")]
            public int OscUpdateRateMs { get; set; }

            [Option('d', "depthClip", DefaultValue = 4.5f,
              HelpText = "kinect depth clip in meters")]
            public float DepthClip { get; set; }

            [Option('j', "jointTX", DefaultValue = 1,
             HelpText = "enable/disable joint data oscTX ")]
            public int JointTX { get; set; }

            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public MainWindow()
        {
            string ipAddress;
            string port;
            String[] args = Environment.GetCommandLineArgs();

            //System.Diagnostics.Debug.WriteLine("\n\n\n\n\nGetCommandLineArgs: {0}", String.Join(", ", args));

            // get dafault values from app resources
            this.oscMessUpdateMS = int.Parse(zKinectV2OSC.Properties.Resources.oscUpdateRateMs);
            //ipAddress = ReadIpAddressCsv();
            ipAddress = GetLocalIPAddress();
            string[] values = ipAddress.Split('.');
            ipAddress = values[0] + "." + values[1] + "." + values[2] + ".255";
            port = zKinectV2OSC.Properties.Resources.PortNumber;
            //Console.Write("**************** ip: " + ipAddress);
            System.Diagnostics.Debug.WriteLine("\n\n\n\n\n my IPaddress:"+ipAddress+"\n");
            var options = new Options();

            // user provided flags, update values
            if ( Parser.Default.ParseArguments(args, options) )
            {
                // Values are available here
                port = options.Port;
                if (String.Empty != options.IpAddr)   
                    ipAddress = options.IpAddr;
                this.depthClip = options.DepthClip;
                this.jointTX =  (options.JointTX == 0) ? false:true;
                this.oscMessUpdateMS = options.OscUpdateRateMs;

                System.Diagnostics.Debug.WriteLine("\n\t\t\t ipAddress:port =  {0}:{1}", options.IpAddr, options.Port);
                System.Diagnostics.Debug.WriteLine("\n\t\t\t oscUpdateRateMS =  {0}", options.OscUpdateRateMs);
            }


            this.timer = new FrameTimer();
            this.InitKinect();
            this.InitNetwork(ipAddress, port);
            this.InitWindowObjectAsViewModel();
        }

        private void InitKinect()
        {
            Size displaySize = new Size(0, 0);
            this.kinectSensor = KinectSensor.GetDefault();

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Open();

                var frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;
                displaySize.Width= frameDescription.Width;
                displaySize.Height = frameDescription.Height;

                this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

                this.UptimeText = zKinectV2OSC.Properties.Resources.InitializingStatusTextFormat;
            }
            else
            {
                this.UptimeText = zKinectV2OSC.Properties.Resources.NoSensorFoundText;
            }

            this.kinectCanvas = new KinectCanvas(this.kinectSensor, displaySize);
        }

        private void InitNetwork(string ipAddress, string port)
        {
            this.bodySender = new BodySender(ipAddress, port);
            this.bodySender.jointTX = jointTX;
            this.bodySender.depthClip = depthClip;
        }

        private void InitWindowObjectAsViewModel()
        {
            this.imageSource = this.kinectCanvas.GetDrawingImage();
            this.DataContext = this;
            this.InitializeComponent();
        }

        private string ReadIpAddressCsv()
        {
            string ipAddressCsv;
            try
            {
                System.IO.TextReader file = new StreamReader(zKinectV2OSC.Properties.Resources.IpAddressFileName);
                ipAddressCsv = file.ReadLine();
                file.Close();
                file = null;
            }
            catch(Exception)
            {
                ipAddressCsv = zKinectV2OSC.Properties.Resources.DefaultIpAddressCsv;
            }
            return ipAddressCsv;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            var frameReference = e.FrameReference;

            try
            {
                var frame = frameReference.AcquireFrame();

                if (frame != null)
                {
                    using (frame)
                    {
                        Double delta = 1000 * (frame.RelativeTime.TotalSeconds - _lastFrameSecs);
                        
                        this.timer.AddFrame(frameReference);
                        this.setStatusText();
                        this.updateBodies(frame);
                        this.kinectCanvas.Draw(this.bodies);


                        // throttle down OSC message sending
                        if (delta >= oscMessUpdateMS)
                        {
                            _lastFrameSecs = frame.RelativeTime.TotalSeconds;
                            this.bodySender.Send(this.bodies);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Frame exception encountered...");
            }
        }

        private void setStatusText()
        {
            var framesPerSecond = timer.GetFramesPerSecond();
            var runningTime = timer.GetRunningTime();
            this.FramesText = string.Format(zKinectV2OSC.Properties.Resources.StandardFramesTextFormat, framesPerSecond);
            this.UptimeText = string.Format(zKinectV2OSC.Properties.Resources.StandardUptimeTextFormat, runningTime);
            this.OscText = bodySender.GetStatusText() + " OSCtxRateMS: " 
                + oscMessUpdateMS + "\n depthClip: " + depthClip + "\n JointTX: "+jointTX;
        }

        private void updateBodies(BodyFrame frame)
        {
            if (this.bodies == null)
            {
                this.bodies = new Body[frame.BodyCount];
            }

            // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
            // As long as those body objects are not disposed and not set to null in the array,
            // those body objects will be re-used.
            frame.GetAndRefreshBodyData(this.bodies);
        }
    }
}
