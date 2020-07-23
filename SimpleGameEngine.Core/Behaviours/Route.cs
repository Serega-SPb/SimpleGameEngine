using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Newtonsoft.Json.Linq;
using SimpleGameEngine.Core.SceneObjects;
using Properties = SimpleGameEngine.Core.Serialization.SerializeExtensions.RouteProperties;

namespace SimpleGameEngine.Core.Behaviours
{
    public class Route
    {
        
        private readonly List<SceneObject> _routePoints = new List<SceneObject>();
        private int _index;
        private const double DistError = 0.1d;

        private readonly Vector _vectorZero = new Vector();

        public Route(SceneObject subject)
        {
            Subject = subject;
        }

        public SceneObject Subject { get; }

        public IEnumerable<SceneObject> RoutePoints
        {
            get
            {
                if (!_routePoints.Any())
                    InitPoints();
                return _routePoints;
            }
        }

        public SceneObject CurrentPoint => _routePoints[Index];

        public bool IsLoop { get; set; }

        private int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPointChanged();
            }
        }

        public void AddPoint(SceneObject point) => _routePoints.Add(point);
        public void RemovePoint(SceneObject point) => _routePoints.Remove(point);
        public void RemoveByIndex(int index) => _routePoints.RemoveAt(index);
        
        public void Move(int oldIndex, int newIndex)
        {
            var item = _routePoints[oldIndex];
            Move(oldIndex, newIndex, item);
        }

        public void Move(int newIndex, SceneObject item)
        {
            var oldIndex = _routePoints.IndexOf(item);
            Move(oldIndex, newIndex, item);
        }

        private void Move(int oldIndex, int newIndex, SceneObject item)
        {
            _routePoints.RemoveAt(oldIndex);
            if (newIndex > oldIndex) 
                newIndex--;
            _routePoints.Insert(newIndex, item);
        }

        public Vector GetMoveVector()
        {
            if (Index >= _routePoints.Count)
            {
                if (IsLoop)
                    Index = 0;
                else
                {
                    Index = _routePoints.Count - 1;
                    return _vectorZero;
                }
            }

            if ((CurrentPoint.PositionPoint - Subject.PositionPoint).Length < DistError)
            {
                Index++;
                return GetMoveVector();
            }
            return CurrentPoint.PositionPoint - Subject.PositionPoint;
        }

        public Vector GetMoveVectorNormalized()
        {
            var vector = GetMoveVector();
            if (vector == _vectorZero)
                return vector;
            vector.Normalize();
            return vector;
        }

        public delegate void PointChangedHandler(SceneObject newPoint);
        public event PointChangedHandler PointChanged;
        private void OnPointChanged() => PointChanged?.Invoke(CurrentPoint);
        
        private readonly List<Lazy<SceneObject>> _loadedPoints = new List<Lazy<SceneObject>>();

        public void InitPoints()
        {
            if (!_loadedPoints.Any()) 
                return;
            _routePoints.AddRange(_loadedPoints.Select(l=> l.Value));
            _loadedPoints.Clear();
        }
        
        public static Route FromJson(JObject jsObj, SceneObject subject)
        {
            var route = new Route(subject)
            {
                IsLoop = jsObj[Properties.IsLoop].Value<bool>()
            };
            var scene = subject.Parent;
            foreach (var jsVal in jsObj[Properties.RoutePoints])
            {
                var name = jsVal[Properties.PointName].Value<string>();
                var positionArr = jsVal[Properties.Position].Values<double>().ToArray();
                var position = new MutablePoint(positionArr[0], positionArr[1]);
                route._loadedPoints.Add(new Lazy<SceneObject>(() => FindObjectFromScene(subject, name, position)));
            }
            return route;
        }

        private static SceneObject FindObjectFromScene(SceneObject subject, string name, MutablePoint position)
        {
            var objs = subject.Parent.GetObjectsByName(name).ToList();
            return objs.Count() == 1 
                ? objs.First() 
                : objs.First(o => o.PositionPoint == position);
        }
    }
}