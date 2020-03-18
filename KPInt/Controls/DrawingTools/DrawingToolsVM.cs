using KPInt.Controls.Canvas;
using KPInt.Models.Tools;
using KPInt_Shared;
using System;
using System.ComponentModel;
using System.Windows;

namespace KPInt.Controls.DrawingTools
{
    class DrawingToolsVM
    {
        public event Action LineDrawn;

        public FrameworkElement View => _control;

        private readonly DrawingToolsView _control = new DrawingToolsView();
        private ToolsWindow _window;

        public ColorLine DrawnLine { get; private set; }

        public DrawingToolsVM(Window parent, CanvasControlVM canvasControl, double fps)
        {
            var pencilTool = new PencilTool(_control.DrawingPanel, (x) => _window.GetDrawingColor(x), ChangeLine, fps);
            var lineTool = new LineTool(_control.DrawingPanel, (x) => _window.GetDrawingColor(x), () => pencilTool.Thickness, ChangeLine);
            var pickerTool = new PickerTool(_control.DrawingPanel, canvasControl.View, (x, y) => _window.SetDrawingColor(x, y));

            _control.DrawingPanel.Children.Add(canvasControl.View);
            _control.Loaded += (s, e) => Control_Loaded(pencilTool, lineTool, pickerTool, parent);
        }

        private void ChangeLine(ColorLine line)
        {
            DrawnLine = line;
            LineDrawn?.Invoke();
        }

        private void Control_Loaded(IDrawingTool pencil, IDrawingTool line, IDrawingTool picker, Window parent)
        {
            _window = new ToolsWindow(pencil, line, picker) { Owner = parent };
            _window.Closing += ToolWindow_Closing;
            _window.Show();
            OpenToolWindow();
        }

        public void OpenToolWindow()
        {
            _window.Top = _window.Owner.Top;
            _window.Left = _window.Owner.Left;
            _window.Visibility = Visibility.Visible;
        }

        private void ToolWindow_Closing(object sender, CancelEventArgs e)
        {
            _window.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }
    }
}
