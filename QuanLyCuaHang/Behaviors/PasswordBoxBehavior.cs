using System.Windows;
using System.Windows.Controls;

namespace QuanLyCuaHang.Behaviors
{
    public static class PasswordBoxBehavior
    {
        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(PasswordBoxBehavior),
                new PropertyMetadata(string.Empty));

        public static bool GetIsPasswordVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPasswordVisibleProperty);
        }

        public static void SetIsPasswordVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPasswordVisibleProperty, value);
        }

        public static readonly DependencyProperty IsPasswordVisibleProperty =
            DependencyProperty.RegisterAttached(
                "IsPasswordVisible",
                typeof(bool),
                typeof(PasswordBoxBehavior),
                new PropertyMetadata(false));
    }
}