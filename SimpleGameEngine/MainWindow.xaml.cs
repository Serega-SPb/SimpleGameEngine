using System.Reflection;
using System.Windows;

namespace SimpleGameEngine
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            FixMenuOpenSide();
        }

        private static void FixMenuOpenSide()
        {
            var ifLeft = SystemParameters.MenuDropAlignment;
            if (!ifLeft) 
                return;
            var t = typeof(SystemParameters);
            var field = t.GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, false);
            ifLeft = SystemParameters.MenuDropAlignment;
        }
    }
}