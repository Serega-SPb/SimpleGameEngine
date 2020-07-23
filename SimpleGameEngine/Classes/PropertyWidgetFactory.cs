using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SimpleGameEngine.Controls;
using SimpleGameEngine.Converters;
using SimpleGameEngine.Core;
using SimpleGameEngine.Core.Animation;
using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.SceneObjects;
using SimpleGameEngine.Validators;
using Xceed.Wpf.Toolkit;

namespace SimpleGameEngine.Classes
{
    public static class PropertyWidgetFactory
    {
        private static readonly Dictionary<Type, Func<string, Action, FrameworkElement>> DataTemplates =
            new Dictionary<Type, Func<string, Action, FrameworkElement>>()
            {
                {typeof(string), StringWidget},
                {typeof(bool), BooleanWidget},
                {typeof(double), DoubleWidget},
                {typeof(int), IntegerWidget},
                {typeof(Shape), ShapeWidget},
                {typeof(MutablePoint), PointWidget},
                {typeof(MutableSize), SizeWidget},
                {typeof(BitmapImage), BitmapImageWidget},
                {typeof(object), DefaultWidget},
                {typeof(Animation), AnimationWidget},
                {typeof(Route), RouteWidget}
            };

        public static FrameworkElement GetWidgetByType(PropertyInfo property, Action updateAction)
        {
            if (property.PropertyType.IsEnum)
                return EnumWidget(property.Name, property.PropertyType, updateAction);
            
            var widget = DataTemplates.ContainsKey(property.PropertyType) 
                ? DataTemplates[property.PropertyType](property.Name, updateAction) 
                : DataTemplates[typeof(object)](property.Name, updateAction);
            return widget;
        }

        private static TextBlock GetLabel(string text) =>
            new TextBlock() {Text = text, Padding = new Thickness(5, 0, 5, 0)};

        private static FrameworkElement DefaultWidget(string propertyName, Action updateAction)
        {
            var widget = new TextBlock(){Padding = new Thickness(5, 0, 5, 0)};
            var bind = new Binding(propertyName);
            widget.SetBinding(TextBlock.TextProperty, bind);
            return widget;
        }
        
        private static FrameworkElement StringWidget(string propertyName, Action updateAction)
        {
            var widget = new TextBox(){
                HorizontalAlignment = HorizontalAlignment.Left, 
                MinWidth = 150, MaxWidth = 250};
            widget.TextChanged += (sender, args) => updateAction();
            var bind = new Binding(propertyName)
            {
                Mode = BindingMode.TwoWay, 
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            widget.SetBinding(TextBox.TextProperty, bind);
            return widget;
        }

        private static FrameworkElement BooleanWidget(string propertyName, Action updateAction)
        {
            var widget = new CheckBox();
            widget.Checked += (sender, args) => updateAction();
            widget.Unchecked += (sender, args) => updateAction();
            var bind = new Binding(propertyName) {Mode = BindingMode.TwoWay};
            widget.SetBinding(ToggleButton.IsCheckedProperty, bind);
            return widget;
        }

        private static FrameworkElement DoubleWidget(string propertyName, Action updateAction)
        {
            var widget = new DoubleUpDown() {
                MaxWidth = 50, Increment = .1, FormatString = "F1",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            widget.ValueChanged += (sender, args) => updateAction(); 
            var bind = new Binding(propertyName){Mode = BindingMode.TwoWay};
            BindingOperations.SetBinding(widget, DoubleUpDown.ValueProperty, bind);
            return widget;
        }

        private static FrameworkElement IntegerWidget(string propertyName, Action updateAction)
        {
            var widget = new IntegerUpDown()
            {
                MaxWidth = 50, HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            widget.ValueChanged += (sender, args) => updateAction();
            var bind = new Binding(propertyName){Mode = BindingMode.TwoWay};
            BindingOperations.SetBinding(widget, IntegerUpDown.ValueProperty, bind);
            return widget;
        }
        
        private static FrameworkElement ShapeWidget(string propertyName, Action updateAction)
        {
            var widget = new StackPanel(){Orientation = Orientation.Horizontal};
            var shapeCmbBx = new ComboBox(){DisplayMemberPath = "Name"};
            foreach (var shape in ShapeSceneObject.Shapes.Values) 
                shapeCmbBx.Items.Add(shape);
            var shapeBind = new Binding(propertyName)
            {
                Mode = BindingMode.TwoWay,
                Converter = new ShapeConverter(),
            };
            shapeCmbBx.SetBinding(Selector.SelectedValueProperty, shapeBind);
            shapeCmbBx.SelectionChanged += (sender, args) => updateAction();
            
            var fillClrPr = new ColorPicker();
            var fillBind = new Binding($"{propertyName}.Fill")
            {
                Mode = BindingMode.TwoWay,
                Converter = new SolidBrushToColorConverter()
            };
            fillClrPr.SetBinding(ColorPicker.SelectedColorProperty, fillBind);

            widget.Children.Add(GetLabel("Shape"));
            widget.Children.Add(shapeCmbBx);
            widget.Children.Add(GetLabel("Fill color"));
            widget.Children.Add(fillClrPr);
            
            return widget;
        }

        private static FrameworkElement PointWidget(string propertyName, Action updateAction)
        {
            var widget = new StackPanel(){Orientation = Orientation.Horizontal};
            
            var xDblUpDwn = DoubleWidget($"{propertyName}.X", updateAction);
            var yDblUpDwn = DoubleWidget($"{propertyName}.Y", updateAction);

            widget.Children.Add(GetLabel("X"));
            widget.Children.Add(xDblUpDwn);
            widget.Children.Add(GetLabel("Y"));
            widget.Children.Add(yDblUpDwn);
            return widget;
        }
        
        private static FrameworkElement SizeWidget(string propertyName, Action updateAction)
        {
            var widget = new StackPanel(){Orientation = Orientation.Horizontal};

            var wDblUpDwn = (DoubleUpDown) DoubleWidget($"{propertyName}.Width", updateAction);
            wDblUpDwn.Minimum = 0;
            wDblUpDwn.Increment = 1;
            var hDblUpDwn = (DoubleUpDown) DoubleWidget($"{propertyName}.Height", updateAction);
            hDblUpDwn.Minimum = 0;
            hDblUpDwn.Increment = 1;

            widget.Children.Add(GetLabel("Width"));
            widget.Children.Add(wDblUpDwn);
            widget.Children.Add(GetLabel("Height"));
            widget.Children.Add(hDblUpDwn);
            return widget;
        }

        private static FrameworkElement BitmapImageWidget(string propertyName, Action updateAction)
        {
            var widget = new StackPanel(){Orientation = Orientation.Horizontal};
            var uriTxtBx = new TextBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                MaxWidth = 250, MinWidth = 150
            };
            var uriBind = new Binding(propertyName)
            {
                Converter = new BitmapToStringConverter(),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            uriTxtBx.TextChanged += (sender, args) => updateAction(); 
            uriBind.ValidationRules.Add(new BitmapSourceValidation(){ValidatesOnTargetUpdated = true});
            uriTxtBx.SetBinding(TextBox.TextProperty, uriBind);
            widget.Children.Add(GetLabel("Source"));
            widget.Children.Add(uriTxtBx);
            
            return widget;
        }

        private static FrameworkElement EnumWidget(string propertyName, Type enumType, Action updateAction)
        {
            var widget = new ComboBox()
            {
                VerticalAlignment = VerticalAlignment.Center, 
                ItemsSource = Enum.GetValues(enumType)
            };
            widget.SelectionChanged += (sender, args) => updateAction();
            var bind = new Binding(propertyName){Mode = BindingMode.TwoWay};
            widget.SetBinding(Selector.SelectedItemProperty, bind);
            return widget;
        }

        private static FrameworkElement AnimationWidget(string propertyName, Action updateAction)
        {
            var widget = new ComboBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                ItemsSource = AnimationManager.Instance.AnimationsCache,
                DisplayMemberPath = "Key",
                SelectedValuePath = "Value.Value"
                
            };
            widget.SelectionChanged += (sender, args) => updateAction();
            var bind = new Binding(propertyName){Mode = BindingMode.TwoWay};
            widget.SetBinding(Selector.SelectedValueProperty, bind);
            return widget;
        }

        private static FrameworkElement RouteWidget(string propertyName, Action updateAction)
        {
            var widget = new RouteControl(){UpdateAction = updateAction};
            var bind = new Binding(propertyName);
            widget.SetBinding(RouteControl.RouteProperty, bind);
            return widget;
        }
    }
}