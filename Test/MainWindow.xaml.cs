using System.Device.Location;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using MetadataExtractor;

namespace Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfiledialog = new OpenFileDialog
            {
                Filter = "图像文件|*.jpg;*.png;*.jpeg;*.bmp;*.gif|所有文件|*.*"
            };
            if ((bool)openfiledialog.ShowDialog())
            {
                photo.Source = new BitmapImage(new Uri(openfiledialog.FileName));
            }
            string filename = openfiledialog.FileName;
            exifmessage.Text = filename;
            StringBuilder sb = new StringBuilder();
            var directories = ImageMetadataReader.ReadMetadata(filename);
            foreach (var directory in directories)
                foreach (var tag in directory.Tags)
                    sb.AppendLine($"{directory.Name} - {tag.Name} = {tag.Description}");
            exifmessage.Text = sb.ToString();
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
