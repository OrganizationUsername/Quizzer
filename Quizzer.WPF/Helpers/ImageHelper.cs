using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Quizzer.WPF.Helpers;

public static class ImageHelper
{
    public static string ImageToString(byte[] bytes)
    {
        using var resized = new MemoryStream();
        using var image = Image.Load(bytes);

        var width = Math.Min(image.Width, 75);
        var height = Math.Min(image.Height, 75);

        image.Mutate(x => x.Resize(new ResizeOptions() { Mode = ResizeMode.Max, Size = new(width, height) }));
        image.Save(resized, new PngEncoder());

        return Convert.ToBase64String(resized.ToArray());
    }
}