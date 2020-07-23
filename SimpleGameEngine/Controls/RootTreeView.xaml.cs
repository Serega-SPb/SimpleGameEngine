using System.ComponentModel;
using System.Windows;

namespace SimpleGameEngine.Controls
{
    public partial class RootTreeView
    {
        public RootTreeView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty RootProperty = DependencyProperty.Register(
            "Root", typeof(object), typeof(RootTreeView), new PropertyMetadata(default(object), RootChanged));

        private static void RootChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RootTreeView tree))
                return;
            
            tree.Items.Clear();
            
            if (e.OldValue != null)
                ((INotifyPropertyChanged) e.OldValue).PropertyChanged -= tree.Reload;
            
            if (e.NewValue != null)
            {
                tree.Items.Add(e.NewValue);
                ((INotifyPropertyChanged) e.NewValue).PropertyChanged += tree.Reload;
            }
        }

        public INotifyPropertyChanged Root
        {
            get => (INotifyPropertyChanged) GetValue(RootProperty);
            set => SetValue(RootProperty, value);
        }

        public static readonly DependencyProperty RefreshTriggerNameProperty = DependencyProperty.Register(
            "RefreshTriggerName", typeof(string), typeof(RootTreeView), new PropertyMetadata(default(string)));

        public string RefreshTriggerName
        {
            get => (string) GetValue(RefreshTriggerNameProperty);
            set => SetValue(RefreshTriggerNameProperty, value);
        }
        
        private void Reload(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals(RefreshTriggerName))
                return;
            Items.Refresh();
            UpdateLayout();
        }
    }
}