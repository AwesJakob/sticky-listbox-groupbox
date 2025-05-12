using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Transform3DGroup 설정
        Transform3DGroup transformGroup = new Transform3DGroup();
        RotateTransform3D rotateTransform = new RotateTransform3D();
        AxisAngleRotation3D rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
        rotateTransform.Rotation = rotation;
        transformGroup.Children.Add(rotateTransform);

        // 정육면체에 Transform 적용
        Model3DGroup modelGroup = ((ModelVisual3D)MyViewport3D.Children[0]).Content as Model3DGroup;
        if (modelGroup != null)
        {
            GeometryModel3D cube = modelGroup.Children[1] as GeometryModel3D;
            if (cube != null)
            {
                cube.Transform = transformGroup;
            }
        }

        // 애니메이션 설정
        DoubleAnimation rotationAnimation = new DoubleAnimation
        {
            From = 0,
            To = 360,
            Duration = TimeSpan.FromSeconds(2),
            RepeatBehavior = RepeatBehavior.Forever
        };

        // 애니메이션 시작
        rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotationAnimation);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Write(DateTime.Now + " Run Task");
        var t = Task.Run(() =>
        {
            while (true)
            {
                if (TaskQueue.Count > 0 && TaskQueue.TryDequeue(out var task))
                {
                    if (task != null )
                    {
                        Write(DateTime.Now + " TaskQueue Count : " + TaskQueue.Count);
                        if (task.Status == TaskStatus.Running)
                        {
                            continue;
                        }
                        task.RunSynchronously();
                    }
                }
            }
        });
    }

    private Queue<Task> TaskQueue = new();

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        Write(DateTime.Now + " Enqeue Task");
        

        TaskQueue.Enqueue(new Task(() =>
        {
            Write(DateTime.Now + " Run Task!!!!!!!!!!!!!");
            Thread.Sleep(1000);
        }));

        Write(DateTime.Now + " TaskQueue Count : " + TaskQueue.Count);
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        tbx.ScrollToEnd();
    }

    private void Write(string value)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            StringBuilder sb = new StringBuilder(tbx.Text);
            sb.AppendLine(value);
            tbx.Text = sb.ToString();
        });
    }
}