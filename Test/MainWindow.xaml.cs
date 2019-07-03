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
            //打开图片
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
            StringBuilder latitude = new StringBuilder();
            StringBuilder longgitude = new StringBuilder();
            //经度输出
            foreach (var directory in directories)
                foreach (var tag in directory.Tags)
                    if ($"{tag.Name}" == "GPS Latitude")
                    {
                        sb.AppendLine($"[{directory.Name}] {tag.Name} = {tag.Description}");
                        latitude.AppendLine($"[{directory.Name}] {tag.Name} = {tag.Description}");
                        exifmessage.Text = sb.ToString();
                    }
            //纬度输出
            foreach (var directory in directories)
                foreach (var tag in directory.Tags)
                    if ($"{tag.Name}" == "GPS Longitude")
                    {
                        sb.AppendLine($"[{directory.Name}] {tag.Name} = {tag.Description}");
                        longgitude.AppendLine($"[{directory.Name}] {tag.Name} = {tag.Description}");
                        exifmessage.Text = sb.ToString();
                    }
            //转换经纬度
            double newlatitude = 0;
            double newlonggitude = 0;
            string Lat= latitude.ToString();
            string Long = longgitude.ToString();
            string temp = null;
            int i = Lat.IndexOf('°');
            int j = Lat.IndexOf("'");
            int k = Lat.IndexOf('"');
            for (int n = 0; n < Lat.Length; n++)
            {
                if (Lat[n]>='1' && Lat[n]<='9')
                {
                    temp = Lat.Substring(n,i-n);
                    break;
                }

            }
            newlatitude = double.Parse(temp);
            for (int n = i; n < Lat.Length; n++)
            {
                if (Lat[n] >= '1' && Lat[n] <= '9')
                {
                    temp = Lat.Substring(n, j - n);
                    break;
                }

            }
            newlatitude += double.Parse(temp)/60;
            for (int n = j; n < Lat.Length; n++)
            {
                if (Lat[n] >= '1' && Lat[n] <= '9')
                {
                    temp = Lat.Substring(n, k - n);
                    break;
                }

            }
            newlatitude += double.Parse(temp)/3600;

            int i2 = Long.IndexOf('°');
            int j2 = Long.IndexOf("'");
            int k2 = Long.IndexOf('"');
            for (int n = 0; n < Long.Length; n++)
            {
                if (Long[n] >= '1' && Long[n] <= '9')
                {
                    temp =Long.Substring(n, i2 - n);
                    break;
                }

            }
            newlonggitude = double.Parse(temp);
            for (int n = i2; n < Long.Length; n++)
            {
                if (Long[n] >= '1' && Long[n] <= '9')
                {
                    temp = Long.Substring(n, j2 - n);
                    break;
                }

            }
            newlonggitude += double.Parse(temp) / 60;
            for (int n = j2; n < Long.Length; n++)
            {
                if (Long[n] >= '1' && Long[n] <= '9')
                {
                    temp = Long.Substring(n, k2 - n);
                    break;
                }

            }
            newlonggitude += double.Parse(temp) / 3600;
            latitudemessage.Text = "经度：" + newlatitude.ToString("#0.000");
            longitudemessage.Text = "纬度：" + newlonggitude.ToString("#0.000");

            //生成图钉
            Pushpin pushpin = new Pushpin();
            MapLayer mapLayer = new MapLayer();

            pushpin.Location = new Location(newlatitude, newlonggitude);
            this.mapLayer.AddChild(pushpin, pushpin.Location);
            //生成路线
            var start = 'Seattle, wa'; var end = 'Portland, OR';
            
                
                
            })
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //输出坐标
        }
    }
}
