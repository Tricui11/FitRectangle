using FitRectangle;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Rectangle = FitRectangle.Rectangle;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using System;

public class MainViewModel : ViewModelBase
{
    private Rectangle _mainRectangle;
    private ObservableCollection<Rectangle> _secondaryRectangles;
    private ObservableCollection<Rectangle> _filteredRectangles;
    private ObservableCollection<Shape> _shapes;
    private double _scale;
    private RectangleManager _rectangleManager;
    private bool _IgnoreOutOfBoundsRectangles;
    private Dictionary<string, Color> ColorsDic = new();

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

    public bool IgnoreOutOfBoundsRectangles
    {
        get => _IgnoreOutOfBoundsRectangles;
        set
        {
            _IgnoreOutOfBoundsRectangles = value;
            OnPropertyChanged();
            FilterAndDrawRectangles();
        }
    }

    public ObservableCollection<ColorSettingsViewModel> ColorsSettings { get; } = new();

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
        Scale = 11.6;
        FillColors();
    }

    private void LoadData()
    {
        string filePath = @"C:\Users\Furer\Desktop\FitRectangle\FitRectangleWpf\rectangles.json";
        var root = LoadRectangles(filePath);
        MainRectangle = root.MainRectangle;
        SecondaryRectangles = new ObservableCollection<Rectangle>(root.SecondaryRectangles);
        _rectangleManager = new RectangleManager(MainRectangle, SecondaryRectangles.ToList(), LogAction);
        _rectangleManager.UpdateMainRectangle(IgnoreOutOfBoundsRectangles, ColorsSettings);
        FilterAndDrawRectangles();
    }

    public Root LoadRectangles(string filePath)
    {
        var jsonContent = File.ReadAllText(filePath);
        var jObject = JObject.Parse(jsonContent);

        var mainRectangle = jObject["mainRectangle"].ToObject<Rectangle>();
        mainRectangle.Color = ColorsDic[jObject["mainRectangle"]["color"].ToString()];

        var secondaryRectangles = new List<Rectangle>();
        foreach (var secondaryRectangle in jObject["secondaryRectangles"])
        {
            var rect = secondaryRectangle.ToObject<Rectangle>();
            rect.Color = ColorsDic[secondaryRectangle["color"].ToString()];
            secondaryRectangles.Add(rect);
        }

        return new Root
        {
            MainRectangle = mainRectangle,
            SecondaryRectangles = secondaryRectangles
        };
    }

    private void FillColors()
    {
        List<string> colors = new() { "Red", "Green", "Blue", "Yellow", "Orange", "Purple", "Pink", "Brown", "Black", "Gray" };
        foreach (var color in colors)
        {
            var colorRGB = (Color)ColorConverter.ConvertFromString(color);
            ColorsDic.Add(color, colorRGB);
            ColorsSettings.Add(new ColorSettingsViewModel(colorRGB));
        }
    }

    private void FilterAndDrawRectangles()
    {
        if (_rectangleManager != null)
        {
            FilteredRectangles = new ObservableCollection<Rectangle>(_rectangleManager.FilterRectangles(ColorsDic.Select(x => x.Value)));
            DrawRectangles();
        }
    }

    private void DrawRectangles()
    {
        Shapes.Clear();

        if (MainRectangle != null)
        {
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
        }
    }

    private Brush GetBrushFromColor(Color color)
    {
        return new SolidColorBrush(color);
    }

    private void ZoomIn()
    {
        Scale *= 1.1;
    }

    private void ZoomOut()
    {
        Scale /= 1.1;
    }

    private void LogAction(string message)
    {
        // Implement logging functionality here
        Console.WriteLine(message);
    }
}
