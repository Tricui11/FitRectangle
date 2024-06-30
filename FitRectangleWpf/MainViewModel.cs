using FitRectangle;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Rectangle = FitRectangle.Rectangle;
using Point = FitRectangle.Point;

public class MainViewModel : ViewModelBase
{
    private Rectangle _mainRectangle;
    private ObservableCollection<Rectangle> _secondaryRectangles;
    private ObservableCollection<Rectangle> _filteredRectangles;
    private ObservableCollection<Shape> _shapes;
    private double _scale;

    public Rectangle MainRectangle
    {
        get => _mainRectangle;
        set
        {
            _mainRectangle = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Rectangle> SecondaryRectangles
    {
        get => _secondaryRectangles;
        set
        {
            _secondaryRectangles = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Rectangle> FilteredRectangles
    {
        get => _filteredRectangles;
        set
        {
            _filteredRectangles = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Shape> Shapes
    {
        get => _shapes;
        set
        {
            _shapes = value;
            OnPropertyChanged();
        }
    }

    public double Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            OnPropertyChanged();
            DrawRectangles();
        }
    }

    public ICommand LoadCommand { get; }
    public ICommand ZoomInCommand { get; }
    public ICommand ZoomOutCommand { get; }

    public MainViewModel()
    {
        LoadCommand = new RelayCommand(LoadData);
        ZoomInCommand = new RelayCommand(ZoomIn);
        ZoomOutCommand = new RelayCommand(ZoomOut);
        SecondaryRectangles = new ObservableCollection<Rectangle>();
        FilteredRectangles = new ObservableCollection<Rectangle>();
        Shapes = new ObservableCollection<Shape>();
        Scale = 18;
    }

    private void LoadData()
    {
        string filePath = @"C:\Users\Furer\Desktop\FitRectangle\FitRectangleWpf\rectangles.json";
        var root = LoadRectangles(filePath);
        MainRectangle = root.MainRectangle;
        SecondaryRectangles = new ObservableCollection<Rectangle>(root.SecondaryRectangles);
        AdjustMainRectangle(MainRectangle, SecondaryRectangles);
        FilteredRectangles = new ObservableCollection<Rectangle>(FilterRectangles(MainRectangle, SecondaryRectangles, new List<string> { "green" }));
        DrawRectangles();
    }

    public Root LoadRectangles(string filePath)
    {
        var jsonContent = File.ReadAllText(filePath);
        var root = JsonConvert.DeserializeObject<Root>(jsonContent);
        return root;
    }

    public void AdjustMainRectangle(Rectangle mainRectangle, ObservableCollection<Rectangle> secondaryRectangles)
    {
        var minX = secondaryRectangles.Min(r => r.BotLeft.X);
        var minY = secondaryRectangles.Min(r => r.BotLeft.Y);
        var maxX = secondaryRectangles.Max(r => r.TopRight.X);
        var maxY = secondaryRectangles.Max(r => r.TopRight.Y);

        mainRectangle.BotLeft = new Point(minX, minY);
        mainRectangle.TopRight = new Point(maxX, maxY);
    }

    public List<Rectangle> FilterRectangles(Rectangle mainRectangle, ObservableCollection<Rectangle> secondaryRectangles, List<string> colors)
    {
        var filtered = secondaryRectangles
       //     .Where(r => colors.Contains(r.Color))
            .Where(r => r.BotLeft.X >= mainRectangle.BotLeft.X && r.BotLeft.Y >= mainRectangle.BotLeft.Y)
            .Where(r => r.TopRight.X <= mainRectangle.TopRight.X && r.TopRight.Y <= mainRectangle.TopRight.Y)
            .ToList();
        return filtered;
    }

    private void DrawRectangles()
    {
        Shapes.Clear();

        if (MainRectangle != null)
        {
            // Draw main rectangle
            var mainRect = new System.Windows.Shapes.Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Width = (MainRectangle.TopRight.X - MainRectangle.BotLeft.X) * Scale,
                Height = (MainRectangle.TopRight.Y - MainRectangle.BotLeft.Y) * Scale
            };
            Canvas.SetLeft(mainRect, MainRectangle.BotLeft.X * Scale);
            Canvas.SetTop(mainRect, MainRectangle.BotLeft.Y * Scale);
            Shapes.Add(mainRect);

            // Draw secondary rectangles
            foreach (var rect in FilteredRectangles)
            {
                var shape = new System.Windows.Shapes.Rectangle
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = GetBrushFromColor(rect.Color),
                    Width = (rect.TopRight.X - rect.BotLeft.X) * Scale,
                    Height = (rect.TopRight.Y - rect.BotLeft.Y) * Scale
                };
                Canvas.SetLeft(shape, rect.BotLeft.X * Scale);
                Canvas.SetTop(shape, rect.BotLeft.Y * Scale);
                Shapes.Add(shape);
            }
        }
    }

    private Brush GetBrushFromColor(string color)
    {
        return color.ToLower() switch
        {
            "red" => Brushes.Red,
            "green" => Brushes.Green,
            "blue" => Brushes.Blue,
            _ => Brushes.Gray,
        };
    }

    private void ZoomIn()
    {
        Scale *= 1.1;
    }

    private void ZoomOut()
    {
        Scale /= 1.1;
    }
}
