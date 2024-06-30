using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;

namespace FitRectangle
{
    public class MainViewModel : ViewModelBase
    {
        private Root _root;
        private double _scale;
        private RectangleManager _rectangleManager;
        private bool _IgnoreOutOfBoundsRectangles;
        private Dictionary<string, Color> _colorsDic = new();
        private Dictionary<Color, Brush> _brushesDic = new();

        public MainViewModel()
        {
            SelectFileCommand = new RelayCommand(SelectFile);
            CalculateCommand = new RelayCommand(Calculate);
            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            Scale = 5.6;
            FillColors();
        }

        public Root Root
        {
            get => _root;
            set
            {
                _root = value;
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
            }
        }
        public ObservableCollection<ColorSettingsViewModel> ColorsSettings { get; } = new();
        public ObservableCollection<Shape> Shapes { get; } = new();

        public ICommand SelectFileCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }

        private void SelectFile()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Select JSON File",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    Root = LoadRectangles(filePath);
                    DrawRectangles();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading rectangles: {ex.Message}");
            }
        }

        private void Calculate()
        {
            try
            {
                _rectangleManager = new RectangleManager(Root, LogAction);
                _rectangleManager.UpdateMainRectangle(IgnoreOutOfBoundsRectangles, ColorsSettings);
                DrawRectangles();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating main rectangle: {ex.Message}");
            }
        }

        public Root LoadRectangles(string filePath)
        {
            var jsonContent = File.ReadAllText(filePath);
            var jObject = JObject.Parse(jsonContent);

            var mainRectangle = jObject["mainRectangle"].ToObject<Rectangle>();
            mainRectangle.Color = _colorsDic[jObject["mainRectangle"]["color"].ToString()];

            var secondaryRectangles = new List<Rectangle>();
            foreach (var secondaryRectangle in jObject["secondaryRectangles"])
            {
                var rect = secondaryRectangle.ToObject<Rectangle>();
                rect.Color = _colorsDic[secondaryRectangle["color"].ToString()];
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
                _colorsDic.Add(color, colorRGB);
                ColorsSettings.Add(new ColorSettingsViewModel(colorRGB));
                _brushesDic.Add(colorRGB, new SolidColorBrush(colorRGB));
            }
        }

        private void DrawRectangles()
        {
            try
            {
                Shapes.Clear();

                if (Root != null)
                {
                    Root.SecondaryRectangles.ForEach(x => Shapes.Add(CreateRectangleShape(x)));
                    Shapes.Add(CreateRectangleShape(Root.MainRectangle, true));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error drawing rectangles: {ex.Message}");
            }
        }

        private Shape CreateRectangleShape(Rectangle rect, bool isMainRectangle = false)
        {
            return new System.Windows.Shapes.Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = isMainRectangle ? 2 : 1,
                Fill = isMainRectangle ? default : _brushesDic[rect.Color],
                Width = (rect.TopRight.X - rect.BotLeft.X) * Scale,
                Height = (rect.TopRight.Y - rect.BotLeft.Y) * Scale
            }
            .SetCanvasPosition(rect.BotLeft.X * Scale, rect.BotLeft.Y * Scale);
        }

        private void ZoomIn() => Scale *= 1.1;

        private void ZoomOut() => Scale /= 1.1;

        private void LogAction(string message)
        {
            Console.WriteLine(message);
        }
    }
}
