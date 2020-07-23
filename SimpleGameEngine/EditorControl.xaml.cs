using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SimpleGameEngine.Classes;
using SimpleGameEngine.Controls;
using SimpleGameEngine.Core;
using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.Enums;
using SimpleGameEngine.Core.SceneObjects;
using SimpleGameEngine.ProxyClasses;
using Path = System.IO.Path;

namespace SimpleGameEngine
{
    public partial class EditorControl
    {
        
        private readonly PropertyWindow _setPropsWin = new PropertyWindow();
        
        public EditorControl()
        {
            InitializeComponent();
            Console.SetOut(new LogWriter(LogList));
            ViewModel.SetEditorCanvas(EditorSceneCanvas);
            ViewModel.SetGameCanvas(GameSceneCanvas);
            EditorSceneCanvas.SizeChanged += (s,e) => ViewModel.UpdateObjects();
        }

        private EditorViewModel ViewModel => (EditorViewModel) DataContext;
        
        private static readonly List<string> ImageExtensions = 
            new List<string> { ".JPG", ".JPEG", ".BMP", ".GIF", ".PNG" };
        
        private void MainSceneDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                DropFile(e);
            
            var f = e.Data.GetFormats();
            var data = e.Data.GetData(f[0], true);
            if (data is Type)
                DropSceneObject((Type)data, e);
            
        }

        private void DropFile(DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null || files.Length == 0)
                return;
            var file = files[0];
            var ext = Path.GetExtension(file);
            if (ext is null || !ImageExtensions.Contains(ext.ToUpper()))
                return;
            
            ViewModel.SetBackground(new BitmapImage(new Uri(file)));
        }

        private void DropSceneObject(Type objType, DragEventArgs e)
        {
            if (!ViewModel.SceneObjectTypes.Contains(objType))
                return;
            var dropPoint = e.GetPosition(EditorSceneCanvas);
            var percX = Math.Round(dropPoint.X/EditorSceneCanvas.ActualWidth*100, 1);
            var percY = Math.Round(dropPoint.Y/EditorSceneCanvas.ActualHeight*100, 1);
            if (!(Activator.CreateInstance(objType) is SceneObject obj))
            {
                Console.WriteLine("Incorrect type (not SceneObject)");
                return;
            }
            obj.PositionPoint = new MutablePoint(percX, percY);
            obj.Direction = PointDirection.TopLeft;
            _setPropsWin.ShowDialog(obj);
            if (!_setPropsWin.Result)
                return;
            obj.SetParent(ViewModel.EditorScene);
            ViewModel.UpdateObjects();
        }

        
        private void PreviewListItemLeftBtnDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
                DragDrop.DoDragDrop(item, item.DataContext, DragDropEffects.Copy);
        }

        private void SceneObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => 
            PropertyControl.Target = e.NewValue;

        private void SceneObjects_Drop(object sender, DragEventArgs e)
        {
            var sceneObj = ((FrameworkElement) sender).DataContext as SceneObject;
            
            var f = e.Data.GetFormats();
            var behType = e.Data.GetData(f[0], true) as Type;
            if (behType is null)
                return;
            Activator.CreateInstance(behType, sceneObj);
            RefreshScene();
        }

        private void SceneObject_Drag(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement elem && elem.DataContext is SceneObject)
                DragDrop.DoDragDrop(elem, elem.DataContext, DragDropEffects.Copy);
        }

        private void RemoveSceneObject(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement elem) || !(elem.DataContext is SceneObject scObj)) 
                return;
            
            scObj.Parent.RemoveObject(scObj);
            RefreshScene();
        }
        
        private void RemoveBehaviour(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement elem) || !(elem.DataContext is BaseBehaviour beh)) 
                return;
            beh.SceneObject.RemoveBehaviour(beh);
            RefreshScene();
        }

        private void RefreshScene()
        {
            if (ViewModel.EditorScene is ProxyScene proxy)
                proxy.ObjectsChanged();
        }
    }
}