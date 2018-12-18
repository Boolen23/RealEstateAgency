using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RealEstateAgency
{
    public static class ImageConverter
    {
        private static MemoryStream StreamFromBitmapSource(BitmapSource writeBmp)
        {
            MemoryStream bmp;
            using (bmp = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(writeBmp));
                enc.Save(bmp);
            }
            return bmp;
        }
        public static byte[] ToByteArray(this string path)
        {
            ImageSource imageSource = new BitmapImage(new Uri(path));
            using (MemoryStream ms = StreamFromBitmapSource(imageSource as BitmapSource))
                return ms.ToArray();
        }
    }
}
