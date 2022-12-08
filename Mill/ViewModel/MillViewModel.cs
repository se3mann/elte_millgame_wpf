using Mill.Model;
using Mill.Persistence;
using System;
using System.Collections.ObjectModel;

namespace Mill.ViewModel
{
    public class MillViewModel : ViewModelBase
    {
        #region Fields

        private MillGameModel _model;

        #endregion

        #region

        public DelegateCommand NewGameCommand { get; private set; }

        public DelegateCommand LoadGameCommand { get; private set; }
        
        public DelegateCommand SaveGameCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }

        public ObservableCollection<MillField> Fields { get; set; }

        public String CurrentPlayer { get { return _model.CurrentPlayer == Player.Player1? "Első játékos" : "Második játékos"; } }

        public String CurrentAction { 
            get 
            {
                string nextStep = "";
                if (_model.CurrentAction == Mill.Model.Action.Adding) nextStep = "Tegyél fel egy korongot!";
                if (_model.CurrentAction == Mill.Model.Action.Removing) nextStep = "Vegyél le az ellenféltől egy korongot!";
                if (_model.CurrentAction == Mill.Model.Action.MoveDest) nextStep = "Jelöld ki mivel akarsz mozogni!";
                if (_model.CurrentAction == Mill.Model.Action.MoveTarget) nextStep = "Jelöld ki hova akarsz mozogni!";
                return nextStep;
            } 
        }

        public int Player1Talon { get { return _model.Table.Player1UnusedToken; } }

        public int Player2Talon { get { return _model.Table.Player2UnusedToken; } }

        #endregion

        #region Events 

        public event EventHandler? NewGame;
        public event EventHandler? LoadGame;
        public event EventHandler? ExitGame;
        public event EventHandler? SaveGame;

        #endregion

        #region Constructors

        public MillViewModel(MillGameModel model) 
        {
            _model= model;
            _model.GameAdvanced += Model_GameAdvanced;
            _model.GameOver += Model_GameOver;
            _model.PlayerHasToPass += Model_PlayerHasToPass;
            _model.GameCreated += Model_GameCreated;

            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());

            Fields = new ObservableCollection<MillField>();
            for (int i = 0; i < _model.Table.Fields.Length; i++)
            {
                Fields.Add(new MillField
                {
                    Player = 0,
                    Number = i,
                    StepCommand = new DelegateCommand(param => StepGame(Convert.ToInt32(param)))
                });
            }

            RefreshTable();
        }

        

        #endregion

        #region Private methods

        private void RefreshTable()
        {
            foreach (MillField field in Fields)
            {
                int player = _model.Table.GetField(field.Number).Player;
                switch (player)
                {
                    case 0:
                        field.Player = 0;
                        break;
                    case 1:
                        field.Player = 1;
                        break;
                    case 2:
                        field.Player = 2;
                        break;
                    default:
                        field.Player = 0;
                        break;
                }
            }

            OnPropertyChanged("CurrentPlayer");
            OnPropertyChanged("CurrentAction");
            OnPropertyChanged("Player1Talon");
            OnPropertyChanged("Player2Talon");
        }

        private void StepGame(int index)
        {
            _model.Step(index);
            Fields[index].Player = _model.Table.GetField(index).Player;

            //OnPropertyChanged("CurrentPlayer");
            //OnPropertyChanged("CurrentAction");
            //OnPropertyChanged("Player1Talon");
            //OnPropertyChanged("Player2Talon");
        }

        #endregion

        #region Game event handlers

        private void Model_GameAdvanced(object? sender, MillEventArgs e)
        {
            OnPropertyChanged("CurrentPlayer");
            OnPropertyChanged("CurrentAction");
            OnPropertyChanged("Player1Talon");
            OnPropertyChanged("Player2Talon");
        }

        private void Model_PlayerHasToPass(object? sender, PassingEventArgs e)
        {
            //
        }

        private void Model_GameOver(object? sender, MillEventArgs e)
        {
            //
        }

        private void Model_GameCreated(object? sender, MillEventArgs e)
        {
            Fields.Clear();
            for (int i = 0; i < _model.Table.Fields.Length; i++)
            {
                Fields.Add(new MillField
                {
                    Player = 0,
                    Number = i,
                    StepCommand = new DelegateCommand(param => StepGame(Convert.ToInt32(param)))
                });
            }
            RefreshTable();
        }

        #endregion

        #region Event methods

        private void OnNewGame()
        {
            if (NewGame != null)
                NewGame(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            if (LoadGame != null)
                LoadGame(this, EventArgs.Empty);
        }

        private void OnSaveGame()
        {
            if (SaveGame != null)
                SaveGame(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            if (ExitGame != null)
                ExitGame(this, EventArgs.Empty);
        }

        #endregion
    }
}
