using System.Windows;
using System.Windows.Input;

namespace KPInt.Models.Tools
{
    class ClickTool : IDrawingTool
    {
        public event MouseButtonEventHandler MouseDown;
        public event MouseButtonEventHandler MouseUp;
        public event MouseEventHandler MouseEnter;

        public bool Pressed { get; private set; }
        public bool LeftButton { get; private set; }

        private readonly UIElement _parent;

        public ClickTool(UIElement parent)
        {
            _parent = parent;
            LeftButton = true;
        }

        public void Attach()
        {
            Pressed = false;
            _parent.PreviewMouseDown += Parent_PreviewMouseDown;
            _parent.PreviewMouseUp += Parent_PreviewMouseUp;
        }

        public void Detach()
        {
            _parent.MouseEnter -= Parent_MouseEnter;
            _parent.PreviewMouseDown -= Parent_PreviewMouseDown;
            _parent.PreviewMouseUp -= Parent_PreviewMouseUp;
        }

        private void Parent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Pressed || (e.ChangedButton != MouseButton.Left && e.ChangedButton != MouseButton.Right)) return;

            LeftButton = e.ChangedButton == MouseButton.Left;
            _parent.MouseEnter += Parent_MouseEnter;
            Pressed = true;

            MouseDown?.Invoke(sender, e);
        }

        private void Parent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!Pressed || e.ChangedButton != (LeftButton ? MouseButton.Left : MouseButton.Right)) return;

            _parent.MouseEnter -= Parent_MouseEnter;
            Pressed = false;

            MouseUp?.Invoke(sender, e);
        }

        private void Parent_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Pressed) return;
            Pressed = (LeftButton ? e.LeftButton : e.RightButton) == MouseButtonState.Pressed;

            MouseEnter?.Invoke(sender, e);
        }
    }
}
