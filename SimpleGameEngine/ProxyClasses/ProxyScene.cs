using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SimpleGameEngine.Core;
using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.ProxyClasses
{
    public sealed class ProxyScene: Scene, INotifyPropertyChanged
    {
        private readonly Scene _subject;

        public ProxyScene() : this(new Scene())
        {
        }

        public ProxyScene(Scene scene)
        {
            _subject = scene;
        }

        public Scene Subject => _subject;
        
        public override string Name
        {
            get => _subject.Name;
            set
            {
                _subject.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public override string Description
        {
            get => _subject.Description;
            set
            {
                _subject.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public override MutableSize Size
        {
            get => _subject.Size;
            set
            {
                _subject.Size = value;
                MainCanvas.Width = value.Width;
                MainCanvas.Height = value.Height;
                OnPropertyChanged(nameof(Size));
            }
        }

        public override BitmapImage Background
        {
            get => _subject.Background;
            set
            {
                _subject.Background = value;
                OnPropertyChanged(nameof(Background));
            }
        }

        public override Canvas MainCanvas => _subject.MainCanvas;

        public override List<SceneObject> Objects => _subject.Objects;


        public override void SetCanvas(Canvas canvas) => _subject.SetCanvas(canvas);

        public override void AddSceneObject(SceneObject sceneObject)
        {
            Objects.Add(sceneObject);
            OnPropertyChanged(nameof(Objects));
            Render();
        }

        public override void RemoveObject(SceneObject sceneObject)
        {
            Objects.Remove(sceneObject);
            OnPropertyChanged(nameof(Objects));
            Render();
        }

        public void ObjectsChanged() => OnPropertyChanged(nameof(Objects));
        
        public override void Render()
        {
            if (Size != MainCanvas.DesiredSize)
            {
                MainCanvas.Width = Size.Width;
                MainCanvas.Height = Size.Height;
            }
            
            _subject.Render();
            RenderSupport();
        }

        private void RenderSupport()
        {
            var routes = (from sceneObject in Objects 
                from behaviour in sceneObject.Behaviours
                    .Where(b => b is MovePathBehaviour) 
                select ((MovePathBehaviour) behaviour).Route);
            foreach (var route in routes)
            {
                if (route.RoutePoints.Count() < 2)
                    continue;
                
                for (int i = 0; i < route.RoutePoints.Count()-1; i++)
                {
                    var p1 = ((IList<SceneObject>) route.RoutePoints)[i].PositionPoint.ToPoint();
                    var p2 = ((IList<SceneObject>) route.RoutePoints)[i+1].PositionPoint.ToPoint();
                    p1 = ConvertPoint(p1);
                    p2 = ConvertPoint(p2);
                    var line = DrawLinkArrow(p1, p2);
                    Subject.MainCanvas.Children.Add(line);
                }

                if (route.IsLoop)
                {
                    var pf = route.RoutePoints.First().PositionPoint.ToPoint();
                    var pl = route.RoutePoints.Last().PositionPoint.ToPoint();
                    pf = ConvertPoint(pf);
                    pl = ConvertPoint(pl);
                    var line = DrawLinkArrow(pl, pf);
                    Subject.MainCanvas.Children.Add(line);
                }
            }
        }
        
        private Shape DrawLinkArrow(Point p1, Point p2)
        {
            GeometryGroup lineGroup = new GeometryGroup();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            Point p = new Point(p1.X + ((p2.X - p1.X) / 1.35), p1.Y + ((p2.Y - p1.Y) / 1.35));
            pathFigure.StartPoint = p;

            Point lpoint = new Point(p.X + 6, p.Y + 15);
            Point rpoint = new Point(p.X - 6, p.Y + 15);
            LineSegment seg1 = new LineSegment();
            seg1.Point = lpoint;
            pathFigure.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment();
            seg2.Point = rpoint;
            pathFigure.Segments.Add(seg2);

            LineSegment seg3 = new LineSegment();
            seg3.Point = p;
            pathFigure.Segments.Add(seg3);

            pathGeometry.Figures.Add(pathFigure);
            RotateTransform transform = new RotateTransform();
            transform.Angle = theta + 90;
            transform.CenterX = p.X;
            transform.CenterY = p.Y;
            pathGeometry.Transform = transform;
            lineGroup.Children.Add(pathGeometry);

            LineGeometry connectorGeometry = new LineGeometry();
            connectorGeometry.StartPoint = p1;
            connectorGeometry.EndPoint = p2;
            lineGroup.Children.Add(connectorGeometry);
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = lineGroup;
            path.StrokeThickness = 2;
            path.Stroke = path.Fill = Brushes.Black;

            return path;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}