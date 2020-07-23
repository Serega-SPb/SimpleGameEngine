using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SimpleGameEngine.Core;

namespace SimpleGameEngine.Examples
{
    public partial class MainWindow: INotifyPropertyChanged
    {
        private readonly GameLoop _gameLoop = GameLoop.Instance;
        private readonly SceneManager _sceneManager = SceneManager.Instance;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Prepare();
        }

        private void Prepare()
        {
            foreach (var scene in _sceneManager.SceneCache.Keys)
            {
                Scenes.Add(scene);
            }
            
            _gameLoop.Start(); // ???
        }
        
        public ObservableCollection<string> Scenes { get; } = new ObservableCollection<string>();

        public string CurrentScene { get; set; }


        private void StartGameClick(object sender, RoutedEventArgs e)
        {
            if (_sceneManager.GameScene is null)
            {
                _sceneManager.GameScene = _sceneManager.GetSceneByName(CurrentScene);
                _sceneManager.GameScene.SetCanvas(GameCanvas);
                _sceneManager.GameScene.EnableScene();
            }
            _gameLoop.Start();
        }

        private void PauseGameClick(object sender, RoutedEventArgs e) => _gameLoop.Stop();
        
        private void StopGameClick(object sender, RoutedEventArgs e)
        {
            if (_sceneManager.GameScene is null)
                return;
            _gameLoop.Stop();
            _sceneManager.GameScene.DisableScene();
            _sceneManager.GameScene = null;
            GameCanvas.Background = null;
            GameCanvas.Children.Clear();
        } 
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}