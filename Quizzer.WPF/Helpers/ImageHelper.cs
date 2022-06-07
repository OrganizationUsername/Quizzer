using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Quizzer.WPF.Helpers;

public static class ImageHelper
{
    public static string ImageToString(byte[] bytes, int width, int height)
    {
        using var resized = new MemoryStream();
        using var image = Image.Load(bytes);

        width = Math.Min(image.Width, width);
        height = Math.Min(image.Height, height);

        image.Mutate(x => x.Resize(new ResizeOptions() { Mode = ResizeMode.Max, Size = new(width, height) }));
        image.Save(resized, new PngEncoder());

        return Convert.ToBase64String(resized.ToArray());
    }
}