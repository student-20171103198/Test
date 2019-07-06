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
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

        }

        int flag = 0;
        double latTag = 0;
        double longTag = 0;

        
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
            //时间输出
            foreach (var directory in directories)
                foreach (var tag in directory.Tags)
                    if ($"{tag.Name}" == "Date/Time")
                    {
                        sb.AppendLine($"[{directory.Name}] {tag.Name} = {tag.Description}");
                        longgitude.AppendLine($"[{directory.Name}] {tag.Name} = {tag.Description}");
                        exifmessage.Text = sb.ToString();
                    }
            //转换经纬度
            double newlatitude = 0;
            double newlonggitude = 0;
            GCJ02_WGS84 gCJ02_WGS84 = new GCJ02_WGS84();
            ConvertBing convertBing = new ConvertBing();
            GPSConvert gPSConvert = new GPSConvert();
            newlatitude = gPSConvert.getlatitude(latitude);
            newlonggitude = gPSConvert.getlonggitude(longgitude);
            convertBing = GCJ02_WGS84.wgs84_To_Gcj02(newlatitude, newlonggitude);
            latitudemessage.Text = "经度：" + convertBing.getLatitude().ToString("#0.000");
            longitudemessage.Text = "纬度：" + convertBing.getLongitude().ToString("#0.000");

            //生成图钉
            Pushpin pushpin = new Pushpin();
            MapLayer mapLayer = new MapLayer();

            pushpin.MouseRightButtonDown += RemovePushpin;
            pushpin.Location = new Location(convertBing.getLatitude(), convertBing.getLongitude());
            this.mapLayer.AddChild(pushpin, pushpin.Location);
            bingMap.Center = new Location(convertBing.getLatitude(), convertBing.getLongitude());
            bingMap.ZoomLevel = 14;

            //图钉标注
            var image = new Image { Width = 200, Height = 200, Margin = new Thickness(8) };
            image.Source = new BitmapImage(new Uri(openfiledialog.FileName));
            ToolTipService.SetToolTip(pushpin,image);

            //生成轨迹
            if (flag > 0)
            {
                MapPolyline polyline = new MapPolyline();
                Location startlocation = new Location(latTag,longTag);
                Location endlocation = new Location(convertBing.getLatitude(), convertBing.getLongitude());
                polyline.Locations = new LocationCollection
                {
                new Location(startlocation),
                new Location(endlocation)
                };

                polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                polyline.StrokeThickness = 5;
                polyline.Opacity = 1;//不透明度
                this.mapLayer.Children.Add(polyline);
                polyline.MouseRightButtonDown += RemovePolyline;
            }
            latTag = convertBing.getLatitude();
            longTag = convertBing.getLongitude();
            flag++;

        }

        private void RemovePolyline(object sender, MouseButtonEventArgs e)
        {
            MapPolyline mapPolyline = (MapPolyline)sender;
            this.mapLayer.Children.Remove(mapPolyline);
        }

        private void RemovePushpin(object sender, MouseButtonEventArgs e)
        {
            Pushpin pushpin = (Pushpin)sender;
            this.mapLayer.Children.Remove(pushpin);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //输出坐标
        }
         
        class   GPSConvert          //坐标转换
        {
            public double getlatitude(StringBuilder latitude)
            {
                double newlatitude = 0;
                string Lat = latitude.ToString();
                string temp = null;
                int i = Lat.IndexOf('°');
                int j = Lat.IndexOf("'");
                int k = Lat.IndexOf('"');
                for (int n = 0; n < Lat.Length; n++)
                {
                    if (Lat[n] >= '1' && Lat[n] <= '9')
                    {
                        temp = Lat.Substring(n, i - n);
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
                newlatitude += double.Parse(temp) / 60;
                for (int n = j; n < Lat.Length; n++)
                {
                    if (Lat[n] >= '1' && Lat[n] <= '9')
                    {
                        temp = Lat.Substring(n, k - n);
                        break;
                    }

                }
                newlatitude += double.Parse(temp) / 3600;
                return newlatitude;
                
            }
            
            public double getlonggitude(StringBuilder longgitude)
            {
                double newlonggitude = 0;
                string Long = longgitude.ToString();
                string temp = null;
                int i = Long.IndexOf('°');
                int j = Long.IndexOf("'");
                int k = Long.IndexOf('"');
                for (int n = 0; n < Long.Length; n++)
                {
                    if (Long[n] >= '1' && Long[n] <= '9')
                    {
                        temp = Long.Substring(n, i - n);
                        break;
                    }

                }
                newlonggitude = double.Parse(temp);
                for (int n = i; n < Long.Length; n++)
                {
                    if (Long[n] >= '1' && Long[n] <= '9')
                    {
                        temp = Long.Substring(n, j - n);
                        break;
                    }

                }
                newlonggitude += double.Parse(temp) / 60;
                for (int n = j; n < Long.Length; n++)
                {
                    if (Long[n] >= '1' && Long[n] <= '9')
                    {
                        temp = Long.Substring(n, k - n);
                        break;
                    }

                }
                newlonggitude += double.Parse(temp) / 3600;
                return newlonggitude;
            }
        }
        
        //右键清除路径
        public void RemoveControl(Pushpin pushpin , MapPolyline polyline)
        {
            MapLayer mapLayer = new MapLayer();
            this.mapLayer.Children.Remove(pushpin);
            this.mapLayer.Children.Remove(polyline);
        }

        //右键清除图钉
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Hide();
            mainWindow.ShowDialog();
        }

        //关闭
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //距离获取
        static public double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double r1 = lat1;
            double r2 =lng1;

            double a = lat2;
            double b = lng2;
            int R = 6378137;
            double s = Math.Acos(Math.Cos(r1) * Math.Cos(a) * Math.Cos(r2 - b) + Math.Sin(r1) * Math.Sin(a)) * R;
                   
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        //渲染
        public class CustomDrawnElement : FrameworkElement
        {
            public static DependencyProperty BackgroundColorProperty;

            static CustomDrawnElement()
            {
                FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(Colors.Yellow);
                metadata.AffectsRender = true;
                BackgroundColorProperty = DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(CustomDrawnElement), metadata);
            }
            public Color BackgroundColor
            {
                get { return (Color)GetValue(BackgroundColorProperty); }
                set  { SetValue(BackgroundColorProperty, value); }
            }
            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                this.InvalidateVisual();
            }
            protected override void OnMouseLeave(MouseEventArgs e)
            {
                base.OnMouseLeave(e);
                this.InvalidateVisual();
            }
            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                /*Rect bounds = new Rect(0, 0, base.ActualWidth, base.ActualHeight);
                drawingContext.DrawRectangle(GetForegroundBrush(), null, bounds);*/
            }
            private Brush GetForegroundBrush()
            {
                if (!IsMouseOver)
                {
                    return new SolidColorBrush(BackgroundColor);
                }
                else
                {
                    RadialGradientBrush brush = new RadialGradientBrush(Colors.White, BackgroundColor);
                    Point absoluteGradientOrigin = Mouse.GetPosition(this);
                    Point relativeGradientOrigin = new Point(absoluteGradientOrigin.X / base.ActualWidth, absoluteGradientOrigin.Y / base.ActualHeight);
                    brush.GradientOrigin = relativeGradientOrigin;
                    brush.Center = relativeGradientOrigin;

                    return brush;
                }
            }
        }
    }
}
