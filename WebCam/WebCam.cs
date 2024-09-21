using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;

namespace VideoStreaming
{
    /// <summary>
    /// This class requires following references
    /// 1) AFroge.dll
    /// 2) AForge.Video
    /// 3) AForge.Video.DirectShow;
    /// </summary>
    public class WebCam : IDisposable
    {
        private FilterInfoCollection webCams;
        private PictureBox pictureBox;
        private VideoCaptureDevice webCamSelected;

        /// <summary>
        /// Get or Set the Frame size of webcam
        /// </summary>
        public Size FrameSize { get; set; }


        /// <summary>
        /// Get the list of names of available webcams
        /// </summary>
        public List<string> WebCamsList
        {
            get
            {
                List<string> webCamNames = new List<string>();

                foreach (FilterInfo webCam in webCams)
                {
                    webCamNames.Add(webCam.Name);
                }

                return webCamNames;
            }
            private set { }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pictureBox">Picturebox to display the video stream</param>
        public WebCam(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            this.FrameSize = new Size(960, 720);

            this.RefreshWebCamsList();
        }

        /// <summary>
        /// Refresh the List of Webcams to update the change
        /// </summary>
        public void RefreshWebCamsList()
        {
            webCams = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        /// <summary>
        /// Take snapshot
        /// </summary>
        /// <returns>Image taken as snapshot</returns>
        public Image TakePicture()
        {
            this.Stop();
            return pictureBox.BackgroundImage;
        }

        /// <summary>
        /// Start the video streaming
        /// </summary>
        /// <param name="device">Name of device that will be used for the video source (should be one from the "WebCamsList")</param>
        public void Start(string device)
        {
            int deviceIndex = WebCamsList.IndexOf(device);

            if (deviceIndex > -1)
            {
                try
                {
                    webCamSelected = new VideoCaptureDevice(webCams[deviceIndex].MonikerString);
                    webCamSelected.NewFrame += new NewFrameEventHandler(NewFrame);

                    Stop();

                    webCamSelected.VideoResolution = webCamSelected.VideoCapabilities[2];
                    webCamSelected.Start();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                throw new Exception("Selected Webcam not found.");
            }
        }


        /// <summary>
        /// Stop the video streaming
        /// </summary>
        public void Stop()
        {
            if (webCamSelected != null && webCamSelected.IsRunning)
            {
                webCamSelected.SignalToStop();
                webCamSelected = null;
            }

        }

        private void NewFrame(object sender, NewFrameEventArgs e)
        {
            Bitmap img = (Bitmap)e.Frame.Clone();

            pictureBox.BackgroundImage = img;
        }

        void IDisposable.Dispose()
        {
            Stop();
        }
    }
}
