using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.Controls
{
    public partial class RouteControl
    {
        public RouteControl()
        {
            InitializeComponent();
            IsLoopChkbx.Checked += OnLoopChanged;
            IsLoopChkbx.Unchecked += OnLoopChanged;
        }

        public static readonly DependencyProperty RouteProperty = DependencyProperty.Register(
            "Route", typeof(Route), typeof(RouteControl), new PropertyMetadata(default(Route), OnRouteChanged));

        public Route Route
        {
            get => (Route) GetValue(RouteProperty);
            set => SetValue(RouteProperty, value);
        }

        private static void OnRouteChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (!(d is RouteControl control))
                return;

            if (!(args.NewValue is Route val))
                return;

            control.IsLoopChkbx.IsChecked = val.IsLoop;
            control.PointsList.ItemsSource = val.RoutePoints.ToList();
            control.UpdateControl();
        }
        
        public Action UpdateAction { get; set; }

        private static string FormattedHeader(int count) => $"Points: {count}";

        private void OnLoopChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Route == null)
                return;
            Route.IsLoop = IsLoopChkbx.IsChecked ?? false;
            UpdateAction?.Invoke();
        }

        private void RemoveItemClick(object sender, RoutedEventArgs args)
        {
            var index = (int)((FrameworkElement)sender).Tag;
            Route.RemoveByIndex(index);
            UpdateControl();
        }

        private void ItemDrop(object sender, DragEventArgs e)
        {
            var dataType = e.Data.GetFormats().First();
            var data = e.Data.GetData(dataType, true) as SceneObject;
            if (data is null || Route.Subject.Equals(data))
                return;
            Route.AddPoint(data);
            UpdateControl();
        }
        
        private void UpdateControl()
        {
            PointsExp.Header = FormattedHeader(Route.RoutePoints.Count());
            UpdateAction?.Invoke();
            PointsList.ItemsSource = Route.RoutePoints.ToList();
        }
    }
}