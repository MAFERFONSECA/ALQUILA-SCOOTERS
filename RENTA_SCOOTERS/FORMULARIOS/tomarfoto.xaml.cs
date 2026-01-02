using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RENTA_SCOOTERS.FORMULARIOS
{
    public partial class tomarfoto : Window
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        // Propiedad pública para almacenar la imagen capturada
        public BitmapImage CapturedImage { get; private set; }

        public tomarfoto()
        {
            InitializeComponent();
            CargarDispositivos();
        }

        private void CargarDispositivos()
        {
            // Cargar las cámaras disponibles
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
                videoSource.Start();
            }
            else
            {
                MessageBox.Show("No se encontraron cámaras.");
            }
        }

        private async void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            using (Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone())
            {
                if (bitmap != null)
                {
                    try
                    {
                        BitmapImage bitmapImage = await ConvertirBitmapAImagenAsync(bitmap);
                        await imgPreview.Dispatcher.InvokeAsync(() =>
                        {
                            imgPreview.Source = bitmapImage;
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al mostrar la imagen: {ex.Message}");
                    }
                }
            }
        }

        private Task<BitmapImage> ConvertirBitmapAImagenAsync(Bitmap bitmap)
        {
            return Task.Run(() =>
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    memoryStream.Position = 0;

                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }
                return bitmapImage;
            });
        }

        private void btnTomarFoto_Click(object sender, RoutedEventArgs e)
        {
            if (imgPreview.Source != null)
            {
                // Asigna la imagen capturada a la propiedad pública
                CapturedImage = imgPreview.Source as BitmapImage;

                if (CapturedImage != null)
                {
                    // Cerrar el formulario después de capturar la imagen
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo capturar la imagen.");
                }
            }
            else
            {
                MessageBox.Show("No hay imagen para capturar.");
            }
        }

        private void btnEliminarFoto_Click(object sender, RoutedEventArgs e)
        {
            imgPreview.Source = null;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
            base.OnClosed(e);
        }
    }
}
