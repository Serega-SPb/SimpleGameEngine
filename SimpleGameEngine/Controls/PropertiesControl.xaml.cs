using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SimpleGameEngine.Classes;

namespace SimpleGameEngine.Controls
{
    public partial class PropertiesControl
    {

        public PropertiesControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty UpdateActionProperty = DependencyProperty.Register(
            "UpdateAction", typeof(Action), typeof(PropertiesControl), new PropertyMetadata(null));

        public Action UpdateAction
        {
            get => (Action) GetValue(UpdateActionProperty);
            set => SetValue(UpdateActionProperty, value);
        }
        
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            "Target", typeof(object), typeof(PropertiesControl), new PropertyMetadata(null, OnTargetChanged));

        public object Target
        {
            get => GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (!(d is PropertiesControl control))
                return;
            
            if (control.UpdateAction is null)
                throw new ArgumentNullException("Update action is null");
            
            control.ProprtiesGrid.Children.Clear();
            control.ProprtiesGrid.RowDefinitions.Clear();
            if (args.NewValue is null)
                return;

            var target = args.NewValue;
            var types = new List<Type>(){target.GetType()};
            var i = 0;
            while (true)
            {
                var type = types[i].BaseType;
                if (type != null && type != typeof(object))
                    types.Add(type);
                else
                    break;
                i++;
            }

            var rowIndex = 0;
            foreach (var type in types)
            {
                if (type.IsSealed)
                    continue;
                
                control.AddRow();
                var blockHeader = new TextBlock()
                {
                    Text = type.Name, HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = new SolidColorBrush(Colors.LightGray), Padding = new Thickness(5),
                    TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center
                };
                control.AddRowContent(blockHeader, rowIndex, columnSpan: 2);
                rowIndex++;
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly| BindingFlags.Instance))
                {
                    if (!property.CanWrite || property.SetMethod.IsPrivate)
                        continue;
                    
                    control.AddRow();
                    var propNameWid = new TextBlock()
                    {
                        Text = property.Name, Margin = new Thickness(5),
                        TextAlignment = TextAlignment.Left, VerticalAlignment = VerticalAlignment.Top
                    };
                    control.AddRowContent(propNameWid, rowIndex);

                    var propValueWid = PropertyWidgetFactory.GetWidgetByType(property, control.UpdateAction);
                    propValueWid.DataContext = target;
                    propValueWid.VerticalAlignment = VerticalAlignment.Top;
                    propValueWid.Margin = new Thickness(5);
                    control.AddRowContent(propValueWid, rowIndex, 1);
                    rowIndex++;
                }
                 
            }
            
        }

        private void AddRow() => 
            ProprtiesGrid.RowDefinitions.Add(new RowDefinition(){Height = GridLength.Auto});

        private void AddRowContent(UIElement element, int row, int column = 0, int columnSpan = 1)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            Grid.SetColumnSpan(element, columnSpan);
            ProprtiesGrid.Children.Add(element);
        }
    }
}