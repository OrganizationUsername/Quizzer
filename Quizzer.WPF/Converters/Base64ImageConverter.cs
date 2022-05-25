using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Quizzer.WPF.Converters;

public class Base64ImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is not string s) { return null!; }

        var bi = new BitmapImage();

        bi.BeginInit();
        bi.StreamSource = new MemoryStream(System.Convert.FromBase64String(s));
        bi.EndInit();

        return bi;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}