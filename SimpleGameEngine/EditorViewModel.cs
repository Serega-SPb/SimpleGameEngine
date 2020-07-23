using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SimpleGameEngine.Classes;
using SimpleGameEngine.Core;
using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.SceneObjects;
using SimpleGameEngine.Examples.Behaviours;
using SimpleGameEngine.ProxyClasses;

namespace SimpleGameEngine
{

    public class EditorViewModel: INotifyPropertyChanged
    {
        private readonly GameLoop _gameLoop = GameLoop.Instance;
        private readonly SceneManager _sceneManager = SceneManager.Instance;
        private readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;
        private Canvas _gameCanvas;

        public EditorViewModel()
        {
            AppDomain.CurrentDomain.Load("SimpleGameEngine.Examples");
            
            SceneObjectTypes = InheritedClassMethods.GetInheritedClasses<SceneObject>();
            BehaviourTypes = InheritedClassMethods.GetInheritedClasses<BaseBehaviour>();
            CreateNewScene();
            InitCommands();
        }

        private void InitCommands()
        {
            CreateNewSceneCommand = new DelegateCommand(CreateNewScene);
            OpenSceneCommand = new DelegateCommand(OpenScene);
            SaveSceneCommand = new DelegateCommand(SaveScene);
            StartGameCommand = new DelegateCommand(StartGame);
            PauseGameCommand = new DelegateCommand(_gameLoop.Stop);
            StopGameCommand = new DelegateCommand(StopGame);
        }

        public ProxyScene EditorScene { get; private set; }

        public Action UpdateSceneAction => EditorScene.Render;

        public IEnumerable<Type> SceneObjectTypes { get; }
        
        public IEnumerable<Type> BehaviourTypes { get; }

        public DelegateCommand CreateNewSceneCommand { get; private set; } 
        public DelegateCommand OpenSceneCommand { get; private set; }
        public DelegateCommand SaveSceneCommand { get; private set; }
        
        public DelegateCommand StartGameCommand { get; private set; }
        public DelegateCommand PauseGameCommand { get; private set; }
        public DelegateCommand StopGameCommand { get; private set; }

        public void SetGameCanvas(Canvas gameCanvas) => _gameCanvas = gameCanvas;

        public void SetEditorCanvas(Canvas editorCanvas)
        {
            EditorScene.SetCanvas(editorCanvas);
            OnPropertyChanged(nameof(UpdateSceneAction));
            UpdateObjects();
        }

        public void UpdateObjects()
        {
            OnPropertyChanged(nameof(EditorScene));
            EditorScene.Render();
        }

        public void SetBackground(BitmapImage image) => 
            EditorScene.Background = image;


        private static bool ConfirmAction(string message)
        {
            var dialogResult = MessageBox.Show(message, "Question", MessageBoxButton.YesNo);
            return dialogResult == MessageBoxResult.Yes;
        }
        
        private void CreateNewScene()
        {
            if (EditorScene?.Objects.Count > 0 && !ConfirmAction("Create new scene?"))
                return;

            var editorCanvas = EditorScene?.MainCanvas;
            EditorScene = new ProxyScene();
            if (editorCanvas != null)
                EditorScene.SetCanvas(editorCanvas);
            OnPropertyChanged(nameof(EditorScene));
            OnPropertyChanged(nameof(UpdateSceneAction));
        }

        private void OpenScene()
        {
            var openDiag = new OpenFileDialog()
            {
                Multiselect = false,
                InitialDirectory = Path.Combine(_baseDir, SceneManager.SceneDir),
                Filter = "Scene files (*.scene)|*.scene"
            };
            var result = openDiag.ShowDialog();
            if (result != true)
                return;

            var editorCanvas = EditorScene.MainCanvas;
            EditorScene = new ProxyScene(_sceneManager.LoadFromFile(openDiag.FileName));
            if (EditorScene.Subject is null)
                return;
            EditorScene.SetCanvas(editorCanvas);
            OnPropertyChanged(nameof(EditorScene));
            OnPropertyChanged(nameof(UpdateSceneAction));
            UpdateSceneAction();
        }

        private void SaveScene()
        {
            var saveDiag = new SaveFileDialog()
            {
                InitialDirectory = Path.Combine(_baseDir, SceneManager.SceneDir),
                Filter = "Scene files (*.scene)|*.scene"
            };
            var result = saveDiag.ShowDialog();
            if (result != true)
                return;
            
            _sceneManager.SaveToFile(EditorScene.Subject, saveDiag.FileName);
        }

        private void StartGame()
        {
            if (_sceneManager.GameScene is null)
            {
                _sceneManager.GameScene = _sceneManager.Copy(EditorScene.Subject);
                _sceneManager.GameScene.SetCanvas(_gameCanvas);
                _sceneManager.GameScene.EnableScene();
            }
            _gameLoop.Start();
        }

        private void StopGame()
        {
            if (_sceneManager.GameScene is null)
                return;
            _gameLoop.Stop();
            _sceneManager.GameScene.DisableScene();
            _sceneManager.GameScene = null;
            _gameCanvas.Background = null;
            _gameCanvas.Children.Clear();
        } 
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string prop = "") => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}