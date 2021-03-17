using System.IO;
using System.Windows.Media.Imaging;

namespace WPFImgApp.Models
{
    public static class ImageOperations
    {
        public static byte[] imageToBytes(BitmapImage bitmapImage)
        {
            int height = bitmapImage.PixelHeight;
            int width = bitmapImage.PixelWidth;
            int nStride = (bitmapImage.PixelWidth * bitmapImage.Format.BitsPerPixel) / 8;
            byte[] pixelByteArray = new byte[bitmapImage.PixelHeight * nStride];
            bitmapImage.CopyPixels(pixelByteArray, nStride, 0);
            return pixelByteArray;
        }

       
        public static BitmapImage bytesToBitmap(byte[] b, BitmapImage sampleImage)
        {
            int nStride = (sampleImage.PixelWidth * sampleImage.Format.BitsPerPixel) / 8;
            var im = BitmapSource.Create(sampleImage.PixelWidth, sampleImage.PixelHeight, sampleImage.DpiX, sampleImage.DpiY,
                sampleImage.Format, sampleImage.Palette, b, nStride);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            BitmapImage bImg = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(im));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            bImg.BeginInit();
            bImg.StreamSource = memoryStream;
            bImg.EndInit();

            memoryStream.Close();

            return bImg;
        }
    }
}