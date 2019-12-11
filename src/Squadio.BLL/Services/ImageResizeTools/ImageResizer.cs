using System;
using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Memory;

namespace Squadio.BLL.Services.ImageResizeTools
{
    public static class ImageResizer
    {
        public static byte[] Resize(byte[] bytes, int size, string format)
        {
            Image<Rgba32> image = null;

            Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithModeratePooling();

            try
            {
                using (image = Image.Load(bytes))
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

                    using (var stream = new MemoryStream())
                    {
                        image.Save(stream, format.Contains("png") ? ImageFormats.Png : ImageFormats.Jpeg);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                image?.Dispose();
                Configuration.Default.MemoryAllocator.ReleaseRetainedResources();
                GC.Collect();
            }
        }
    }
}