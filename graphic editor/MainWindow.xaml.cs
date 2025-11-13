using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;

namespace graphic_editor
{
    public partial class MainWindow : Window
    {
        private ITool _currentTool;
        private readonly EditorContext _context;
        private readonly Stack<ICommand> _undoStack = new Stack<ICommand>();
        private readonly Stack<ICommand> _redoStack = new Stack<ICommand>();
        private const int MAX_UNDO_STEPS = 20;
        private bool _isSpacePressed = false;

        public MainWindow()
        {
            InitializeComponent();
            _context = new EditorContext(this.ExecuteCommand);
            FillColorPicker.SelectedColor = _context.CurrentFillColor;
            StrokeColorPicker.SelectedColor = _context.CurrentStrokeColor;
            _currentTool = new SelectTool(_context);
            UpdateZoomText();
            UpdateUndoRedoButtons();
        }

        private void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear(); // Очищаем redo при новом действии

            // Ограничение размера стека Undo
            if (_undoStack.Count > MAX_UNDO_STEPS)
            {
                var newStack = new Stack<ICommand>();
                while (_undoStack.Count > 0 && newStack.Count < MAX_UNDO_STEPS)
                {
                    newStack.Push(_undoStack.Pop());
                }
                _undoStack.Clear();
                while (newStack.Count > 0)
                {
                    _undoStack.Push(newStack.Pop());
                }
            }

            Cava.Invalidate();
            UpdateUndoRedoButtons();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
                Cava.Invalidate();
                UpdateUndoRedoButtons();
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
                Cava.Invalidate();
                UpdateUndoRedoButtons();
            }
        }

        private void UpdateUndoRedoButtons()
        {
            // Обновляем состояния кнопок Undo/Redo если нужно
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Cava.SelectedShape != null)
            {
                var command = new DeleteShapeCommand(Cava.Shapes, Cava.SelectedShape);
                ExecuteCommand(command);
                Cava.SelectedShape = null;
            }
        }

        // --- Обработчики инструментов ---

        private void SetCurrentTool(ITool newTool)
        {
            _currentTool?.FinishPolygon(Cava);
            Cava.SelectedShape = null;
            _currentTool = newTool;
        }

        private void SelectTool_Click(object sender, RoutedEventArgs e) => SetCurrentTool(new SelectTool(_context));
        private void RectangleTool_Click(object sender, RoutedEventArgs e) => SetCurrentTool(new RectangleTool(_context));
        private void EllipseTool_Click(object sender, RoutedEventArgs e) => SetCurrentTool(new EllipseTool(_context));
        private void LineTool_Click(object sender, RoutedEventArgs e) => SetCurrentTool(new LineTool(_context));
        private void PolygonTool_Click(object sender, RoutedEventArgs e) => SetCurrentTool(new PolygonTool(_context));

        // --- Обработчики мыши для Canvas ---

        private void Cava_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isSpacePressed)
            {
                Cava.StartPan(e.GetPosition(Cava));
            }
            else
            {
                _currentTool?.OnMouseDown(Cava, e);
            }
        }

        private void Cava_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isSpacePressed)
            {
                Cava.DoPan(e.GetPosition(Cava));
            }
            else
            {
                _currentTool?.OnMouseMove(Cava, e);
            }
        }

        private void Cava_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isSpacePressed)
            {
                Cava.StopPan();
            }
            else
            {
                _currentTool?.OnMouseUp(Cava, e);
            }
        }

        // --- Обработчики клавиатуры ---

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                _isSpacePressed = true;
                Cava.Cursor = Cursors.SizeAll;
            }
            else if (e.Key == Key.Delete && Cava.SelectedShape != null)
            {
                Delete_Click(sender, e);
            }
            else if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Undo_Click(sender, e);
            }
            else if (e.Key == Key.Y && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Redo_Click(sender, e);
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                _isSpacePressed = false;
                Cava.Cursor = Cursors.Arrow;
                Cava.StopPan();
            }
        }

        // --- Обработчики цвета ---

        private void FillColor_Changed(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (_context != null)
            {
                _context.CurrentFillColor = e.NewValue;

                // Обновляем выбранную фигуру если есть
                if (Cava.SelectedShape != null)
                {
                    var oldColor = Cava.SelectedShape.FillColor;
                    Cava.SelectedShape.FillColor = SerializableColor.FromColor(e.NewValue);
                    Cava.Invalidate();
                }
            }
        }

        private void StrokeColor_Changed(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (_context != null)
            {
                _context.CurrentStrokeColor = e.NewValue;

                // Обновляем выбранную фигуру если есть
                if (Cava.SelectedShape != null)
                {
                    var oldColor = Cava.SelectedShape.StrokeColor;
                    Cava.SelectedShape.StrokeColor = SerializableColor.FromColor(e.NewValue);
                    Cava.Invalidate();
                }
            }
        }

        // --- Обработчики масштаба ---

        private void UpdateZoomText()
        {
            ZoomText.Text = $"{(Cava.Scale * 100):F0}%";
        }

        private void Cava_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            Cava.ZoomToPoint(zoomFactor, e.GetPosition(Cava));
            UpdateZoomText();
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            Point center = new Point(Cava.ActualWidth / 2, Cava.ActualHeight / 2);
            Cava.ZoomToPoint(1.2, center);
            UpdateZoomText();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            Point center = new Point(Cava.ActualWidth / 2, Cava.ActualHeight / 2);
            Cava.ZoomToPoint(0.8, center);
            UpdateZoomText();
        }

        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            Cava.ResetView();
            UpdateZoomText();
        }

        // --- Сохранение/Загрузка ---

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Графический проект (*.graph)|*.graph",
                DefaultExt = ".graph"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(fs, Cava.Shapes.ToList());
                    }
                    MessageBox.Show("Проект успешно сохранен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                }
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "Графический проект (*.graph)|*.graph",
                DefaultExt = ".graph"
            };

            if (openDialog.ShowDialog() == true)
            {
                try
                {
                    using (FileStream fs = new FileStream(openDialog.FileName, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        var shapes = (List<Shape>)formatter.Deserialize(fs);

                        _undoStack.Clear();
                        _redoStack.Clear();
                        Cava.Shapes.Clear();
                        Cava.Shapes.AddRange(shapes);
                        Cava.SelectedShape = null;
                        Cava.ResetView();
                        Cava.Invalidate();
                    }
                    MessageBox.Show("Проект успешно загружен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                }
            }
        }
    }
}