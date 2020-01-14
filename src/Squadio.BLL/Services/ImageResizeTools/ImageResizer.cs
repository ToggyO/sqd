using System;
using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Memory;

namespace Squadio.BLL.Services.ImageResizeTools
{
    public static class ImageResizer
    {
        public static Image GetBaseImage(MemoryStream stream)
        {
            //Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithModeratePooling();

            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                return Image.Load(stream);
            }
            finally
            {
                Configuration.Default.MemoryAllocator.ReleaseRetainedResources();
            }
        }

        public static MemoryStream GetResizedImage(Image baseImage, int size, string format)
        {
            //Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithModeratePooling();

            try
            {
                using (var image = baseImage.CloneAs<Rgba32>())
                {
                    int width, height;
                    if (image.Width > image.Height)
                    {
                        width = size;
                        height = image.Height * size / image.Width;
                    }
                    else
                    {
                        width = image.Width * size / image.Height;
                        height = size;
                    }

                    image.Mutate(x => x
                        .Resize(width, height));

                    var mstream = new MemoryStream();
                    image.Save(mstream,
                        format.Contains("png")
                            ? (IImageFormat) PngFormat.Instance
                            : JpegFormat.Instance);
                    return mstream;
                }
            }
            finally
            {
                Configuration.Default.MemoryAllocator.ReleaseRetainedResources();
            }
        }

        public static MemoryStream Resize(MemoryStream stream, int size, string format)
        {
            Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithModeratePooling();

            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var image = Image.Load(stream))
                {
                    int width, height;
                    if (image.Width > image.Height)
                    {
                        width = size;
                        height = image.Height * size / image.Width;
                    }
                    else
                    {
                        width = image.Width * size / image.Height;
                        height = size;
                    }

                    image.Mutate(x => x
                        .Resize(width, height));

                    var mstream = new MemoryStream();
                    image.Save(mstream,
                        format.Contains("png")
                            ? (IImageFormat) PngFormat.Instance
                            : JpegFormat.Instance);
                    return mstream;
                }
            }
            finally
            {
                Configuration.Default.MemoryAllocator.ReleaseRetainedResources();
            }
        }
    }
}