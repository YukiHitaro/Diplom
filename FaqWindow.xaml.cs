// FaqWindow.xaml.cs
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace CSharpTrainer
{
    public partial class FaqWindow : Window
    {
        public FaqWindow()
        {
            InitializeComponent();
        }

        private void OpenPdf(string pdfFileName)
        {
            try
            {
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                string baseDirectory = Path.GetDirectoryName(assemblyLocation);

                // Предполагается, что PDF-файлы находятся в корневой папке выходного каталога (например, bin/Debug)
                string pdfPath = Path.Combine(baseDirectory, pdfFileName);

                if (File.Exists(pdfPath))
                {
                    // Использование ProcessStartInfo с UseShellExecute = true для открытия файла системным просмотрщиком PDF
                    ProcessStartInfo psi = new ProcessStartInfo(pdfPath)
                    {
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                else
                {
                    MessageBox.Show($"Файл '{pdfFileName}' не найден по пути: {pdfPath}\n\nУбедитесь, что файл добавлен в проект, его 'Действие при сборке' установлено как 'Содержимое', а 'Копировать в выходной каталог' как 'Копировать, если новее' или 'Всегда копировать'.",
                                    "Ошибка открытия файла", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть PDF файл '{pdfFileName}': {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenFaqPdfButton_Click(object sender, RoutedEventArgs e)
        {
            OpenPdf("faq.pdf");
        }

        private void OpenManualPdfButton_Click(object sender, RoutedEventArgs e)
        {
            OpenPdf("manual.pdf");
        }
    }
}