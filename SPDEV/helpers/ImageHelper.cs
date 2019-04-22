using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SharePoint.Helpers
{
    public class ImageHelper
    {
        public static string DetectImageExtension(Stream s)
        {
            Image i = Image.FromStream(s);
            ImageFormat f = i.RawFormat;
            if (f.Guid.Equals(ImageFormat.Jpeg.Guid)) return "jpg";
            if (f.Guid.Equals(ImageFormat.Gif.Guid)) return "gif";
            if (f.Guid.Equals(ImageFormat.Png.Guid)) return "png";
            if (f.Guid.Equals(ImageFormat.Bmp.Guid)) return "bmp";
            if (f.Guid.Equals(ImageFormat.Tiff.Guid)) return "tiff";
            if (f.Guid.Equals(ImageFormat.Icon.Guid)) return "icon";
            if (f.Guid.Equals(ImageFormat.Wmf.Guid)) return "wmf";
            if (f.Guid.Equals(ImageFormat.Exif.Guid)) return "exif";
            if (f.Guid.Equals(ImageFormat.Emf.Guid)) return "emf";
            throw new Exception("Unknown Image foramt");
        }
    }
}
