using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace graphic_editor
{
    public class ColorPicker : Control
    {
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                nameof(SelectedColor),
                typeof(Color),
                typeof(ColorPicker),
                new FrameworkPropertyMetadata(
                    Colors.Black,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedColorChanged));

        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged;

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker picker = (ColorPicker)d;
            picker.SelectedColorChanged?.Invoke(picker,
                new RoutedPropertyChangedEventArgs<Color>((Color)e.OldValue, (Color)e.NewValue));
        }

        private Button _colorButton;
        private Popup _colorPopup;
        private UniformGrid _colorGrid;

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker),
                new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _colorButton = GetTemplateChild("PART_ColorButton") as Button;
            _colorPopup = GetTemplateChild("PART_ColorPopup") as Popup;
            _colorGrid = GetTemplateChild("PART_ColorGrid") as UniformGrid;

            if (_colorButton != null)
            {
                _colorButton.Click += (sender, e) =>
                {
                    if (_colorPopup != null)
                    {
                        _colorPopup.IsOpen = !_colorPopup.IsOpen;
                    }
                };

                _colorButton.Background = new SolidColorBrush(SelectedColor);
            }

            if (_colorGrid != null)
            {
                CreateColorButtons();
            }
        }

        private void CreateColorButtons()
        {
            _colorGrid.Children.Clear();

            Color[] colors = new Color[]
            {
                Colors.Black, Colors.White, Colors.Gray, Colors.Red,
                Colors.Green, Colors.Blue, Colors.Yellow, Colors.Orange,
                Colors.Purple, Colors.Pink, Colors.Brown, Colors.Transparent
            };

            foreach (var color in colors)
            {
                Button btn = new Button
                {
                    Background = new SolidColorBrush(color),
                    Margin = new Thickness(2),
                    Width = 20,
                    Height = 20,
                    Tag = color
                };

                btn.Click += (sender, e) =>
                {
                    SelectedColor = (Color)((Button)sender).Tag;
                    if (_colorPopup != null)
                    {
                        _colorPopup.IsOpen = false;
                    }
                    if (_colorButton != null)
                    {
                        _colorButton.Background = new SolidColorBrush(SelectedColor);
                    }
                };

                _colorGrid.Children.Add(btn);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == SelectedColorProperty && _colorButton != null)
            {
                _colorButton.Background = new SolidColorBrush(SelectedColor);
            }
        }
    }
}