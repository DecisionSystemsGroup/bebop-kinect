using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Kinect;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace WpfApplication3
{
    public partial class MainWindow : Window
    {
        


        #region variables
        CameraMode _mode = CameraMode.Color;

        KinectSensor _sensor;
        Skeleton[] _bodies = new Skeleton[0];
        Skeleton[] _totalbodies = new Skeleton[6];
        Boolean headJump = false;
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ManualResetEvent oSignalEvent = new ManualResetEvent(false);


        #endregion
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.KinectSensors.Where(s => s.Status == KinectStatus.Connected).FirstOrDefault();

            if (_sensor != null)
            {
                _sensor.ColorStream.Enable();
                _sensor.DepthStream.Enable();
                _sensor.SkeletonStream.Enable();
                _sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(_sensor_SkeletonFrameReady);
                _sensor.AllFramesReady += Sensor_AllFramesReady;
                Thread connection = new Thread(LoopConnect);
                connection.Priority = ThreadPriority.Normal;
                connection.Start();
                _sensor.Start();
            }
            else
            {
                MessageBox.Show("Camera Not Connected");
                Application.Current.Shutdown();
            }
        }
        #endregion
        #region Skeleton

        void Sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            // Color
            using (var frame = e.OpenColorImageFrame())
            {
                if (frame != null)
                {
                    if (_mode == CameraMode.Color)
                    {
                        camera.Source = frame.ToBitmap();
                    }
                }
            }

            // Depth
            using (var frame = e.OpenDepthImageFrame())
            {
                if (frame != null)
                {
                    if (_mode == CameraMode.Depth)
                    {
                        camera.Source = frame.ToBitmap();
                    }
                }
            }

            // Body
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    canvas.Children.Clear();

                    frame.CopySkeletonDataTo(_totalbodies);

                    foreach (var body in _totalbodies)
                    {
                        if (body.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            // COORDINATE MAPPING
                            foreach (Joint joint in body.Joints)
                            {
                                // 3D coordinates in meters
                                SkeletonPoint skeletonPoint = joint.Position;

                                // 2D coordinates in pixels
                                Point point = new Point();

                                if (_mode == CameraMode.Color)
                                {
                                    // Skeleton-to-Color mapping
                                    ColorImagePoint colorPoint = _sensor.CoordinateMapper.MapSkeletonPointToColorPoint(skeletonPoint, ColorImageFormat.RgbResolution640x480Fps30);

                                    point.X = colorPoint.X;
                                    point.Y = colorPoint.Y;
                                }
                                else if (_mode == CameraMode.Depth) // Remember to change the Image and Canvas size to 320x240.
                                {
                                    // Skeleton-to-Depth mapping
                                    DepthImagePoint depthPoint = _sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution320x240Fps30);

                                    point.X = depthPoint.X;
                                    point.Y = depthPoint.Y;
                                }

                                // DRAWING...
                                Ellipse ellipse = new Ellipse
                                {
                                    Fill = Brushes.LightBlue,
                                    Width = 20,
                                    Height = 20
                                };

                                Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
                                Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

                                canvas.Children.Add(ellipse);
                            }
                        }
                    }
                }
            }

        }
        void _sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame SkelFrame = e.OpenSkeletonFrame())
            {
                if (SkelFrame == null)
                {
                    return;
                }
                _bodies = new Skeleton[SkelFrame.SkeletonArrayLength];
                //making a thread for connection tries to the server
                

                SkelFrame.CopySkeletonDataTo(_bodies);
                SkelFrame.CopySkeletonDataTo(_totalbodies);
                Skeleton firstSkel = (from trackskeleton in _totalbodies
                                      where trackskeleton.TrackingState == SkeletonTrackingState.Tracked
                                      select trackskeleton).FirstOrDefault();

                if (firstSkel == null)
                {
                    return;
                }

                if ((firstSkel.Joints[JointType.HandRight].TrackingState )   == JointTrackingState.Tracked)
                {
                    // Thread t1 = new Thread(() => this.MapJointwithUIElement(firstSkel));
                    //t1.Start();
                    this.MapJointwithUIElement(firstSkel);
                }



            }
        }
        #endregion
        #region Joint coordinates 

        private void MapJointwithUIElement(Skeleton skeleton)
        {



            test = 1;



            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
             //   Boolean headJump = false;
                Boolean temp = false;
                Boolean jumped = false;
                byte[] buffer = null;
                // byte[] buffer = null;
                // StreamWriter coordinatesWriter = new StreamWriter(@"SkeletonData.txt", true);

                // Thread t5 = new Thread(() => LoopConnect());
                //t5.Start();
                //LoopConnect();


                //    Boolean headJump = false;



                foreach (Skeleton skelt in _bodies)
                {
                    #region enalaktikos tropos
                    /*  
                     var x = skeleton.Joints[JointType.HandRight].Position.X;
                     var y = skeleton.Joints[JointType.HandRight].Position.Y;
                     var z = skeleton.Joints[JointType.HandRight].Position.Z;

                     RightHand.Content = string.Format("X = {0}, Y = {1}, Z = {2}", x,y, z);
                     coordinatesWriter.WriteLine(x + "   -   " + y + "   -   " + z);
                     */
                    #endregion
                    
                    var position = skeleton.Joints[JointType.HandRight].Position.Z;
                    var positionHead = skeleton.Joints[JointType.Head].Position.Z;
                    var positionLeftHand = skeleton.Joints[JointType.HandLeft].Position.Z;
 
                    Point mappedPoint = this.Position_Skeleton(skeleton.Joints[JointType.HandRight].Position);//right hand
                    Point mappedPointHead = this.Position_Skeleton(skeleton.Joints[JointType.Head].Position);//Head
                    Point mappedPointLeftHand = this.Position_Skeleton(skeleton.Joints[JointType.HandLeft].Position);//left hand

                    //Head coordinates
                    var xHead = mappedPointHead.X;
                    var yHead = mappedPointHead.Y;
                    var zHead = positionHead;

                    //right hand coordinates
                    var x = mappedPoint.X;
                    var y = mappedPoint.Y;
                    var z = position;

                    //left hand coordinates
                    var xLeftHand = mappedPointLeftHand.X;
                    var yLeftHand = mappedPointLeftHand.Y;
                    var zLeftHand = positionLeftHand;
                    //Un-Comment to see coordinates
                    //RightHand.Content = string.Format("X = {0}, Y = {1}, Z = {2}", x, y, position);
                    // Head.Content = string.Format("X = {0}, Y = {1}, Z = {2}", xres, yres, position);
                       RightHand.Content = string.Format("X = {0}, Y = {1}, Z = {2}", xHead, yHead, zHead);
                    //RightHand.Content = string.Format("X = {0}, Y = {1}, Z = {2}", xLeftHand, yLeftHand, zLeftHand);
                    // RightHand.Content = string.Format("X = {0}, Y = {1}, Z = {2}", xSpine, ySpine, zSpine);
                    
                  //  RightHand.Content = string.Format(headJump.ToString());
                         if (yHead <= 12 & headJump == false ) {
                        
                        buffer = Encoding.ASCII.GetBytes("U");
                        _clientSocket.Send(buffer);
                        headJump = true;
                       // buffer = null;
                        }

                        else if (yHead >= 150 & headJump == true){

                             buffer = Encoding.ASCII.GetBytes("V");
                             _clientSocket.Send(buffer);
                            //Land
                            headJump = false;
                        }

                        else if (y < 140 & x > 600 & headJump == true){

                           buffer = Encoding.ASCII.GetBytes("R");
                            //Go right
                            _clientSocket.Send(buffer);
                        }
                         
                        else if (yLeftHand < 90 & headJump == true){

                            buffer = Encoding.ASCII.GetBytes("L");
                            //Go Left
                            _clientSocket.Send(buffer);
                        }

                        else if (xLeftHand > 350 & yLeftHand < 300 & headJump == true){

                            buffer = Encoding.ASCII.GetBytes("B");
                            //Go Back
                            _clientSocket.Send(buffer);
                        }

                        else if (x < 400 & y < 270 & headJump == true){

                            buffer = Encoding.ASCII.GetBytes("F");
                            //Go Forward
                            _clientSocket.Send(buffer);
                        }

                        else if (z > 1.63 & headJump == true){

                           buffer = Encoding.ASCII.GetBytes(">");
                            //Rotate Right
                            _clientSocket.Send(buffer);
                        }

                        else if (yLeftHand < 300 & zLeftHand > 1.45 & headJump == true){

                         buffer = Encoding.ASCII.GetBytes("<");
                            //Rotate Left
                            _clientSocket.Send(buffer);
                        }
                    }
            }
        }
        

        

        private Point Position_Skeleton(SkeletonPoint skeletonPoint)
        {
            DepthImagePoint depthPoint = _sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
        #endregion        
        #region angle
        private void SetSensorAngle(int Anglevalue)
        {
            if (_sensor != null)
            {
                if (Anglevalue > _sensor.MinElevationAngle || Anglevalue < _sensor.MaxElevationAngle)
                {
                    try
                    {
                        _sensor.ElevationAngle = Anglevalue;
                    }
                    catch (Exception ex) {
                      //  txtErrors = ex.Message;
                    }
                }

            }

        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            this.SetSensorAngle(Int32.Parse(e.NewValue.ToString()));
        }

        #endregion
        #region Seated Mode
        private void CheckBoxSeatedModeChanged(object sender, RoutedEventArgs e)
        {
            if (_sensor != null)
            {
                if (this.checkBoxSeatedMode.IsChecked.GetValueOrDefault())
                {
                    _sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                }
                else
                {
                    _sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                }
            }
        }

        enum CameraMode
        {
            Color,
            Depth
        }
        #endregion
        #region stop
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_sensor != null)
            {
                _sensor.Stop();
            }
        }
        #endregion
        #region socket communication
        
        private static void LoopConnect() { 

            int attempts = 0;
            while (!_clientSocket.Connected) { 
            
                try{
                    attempts++;
                    // _clientSocket.Connect(IPAddress.Loopback, 100);
                    //Drone IP
                    // _clientSocket.Connect(IPAddress.Parse("192.168.42.54"), 1100);
                     //Local IP
                    _clientSocket.Connect(IPAddress.Parse("127.0.0.1"), 1100);
                



                }
                catch (SocketException){}
            }
        }


        #endregion



    }
}