using System.Windows;

namespace SimpleGameEngine.Controls
{
    public partial class PropertyWindow : Window
    {
        public PropertyWindow()
        {
            InitializeComponent();
        }
        
        public object Target { get; private set; }

        public bool Result { get; private set; }

        public void ShowDialog(object target)
        {
            Result = false;
            Target = target;
            Control.UpdateAction = () => { };
            Control.Target = target;
            base.ShowDialog();
        }

        private void OkBtnClick(object sender, RoutedEventArgs e)
        {
            Result = true;
            Hide();
        }
        
        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}