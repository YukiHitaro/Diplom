using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CSharpTrainer
{
    public class CompletionToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Реализация конвертера
            return (bool)value
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D3748"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1A202C"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CompletionToBorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Реализация конвертера
            return (bool)value
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF48BB78"))
                : Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CompletionToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Реализация конвертера
            return (bool)value ? Brushes.White : Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CompletionToButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool isCompleted && isCompleted) ? "Повторить" : "Открыть";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}