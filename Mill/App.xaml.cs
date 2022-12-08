using Microsoft.Win32;
using Mill.Model;
using Mill.Persistence;
using Mill.ViewModel;
using Mill.View;
using System;
using System.Windows;

namespace Mill
{
    public partial class App : Application
    {
        #region Fields
        private MainWindow? _view;
        private MillGameModel? _model;
        private MillViewModel? _viewModel;
        #endregion

        #region Constructors
        public App()
        {
            Startup += App_Startup;
        }
        #endregion

        #region App event handlers
        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new MillGameModel(new MillFileDataAccess());
            _model.GameOver += Model_GameOver;
            _model.PlayerHasToPass += Model_PlayerHasToPass;

            _viewModel = new MillViewModel(_model);
            _viewModel.NewGame += ViewModel_NewGame;
            _viewModel.ExitGame += ViewModel_ExitGame;
            _viewModel.LoadGame += ViewModel_LoadGame;
            _viewModel.SaveGame += ViewModel_SaveGame;

            _view = new MainWindow
            {
                DataContext = _viewModel
            };
            _view.Closing += _view_Closing;
            _view.Show();
        }

        #endregion

        #region View event handlers
        private void _view_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Biztos, hogy ki akar lépni?", "Malom", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; 
            }
        }
        #endregion

        #region ViewModel event handlers

        private void ViewModel_NewGame(object? sender, EventArgs e)
        {
            if (_model != null) _model.NewGame();
        }

        private async void ViewModel_LoadGame(object? sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog(); 
                openFileDialog.Title = "Malom betöltése";
                openFileDialog.Filter = "Malom tábla|*.mtl";
                if (openFileDialog.ShowDialog() == true)
                {
                    if (_model != null) await _model.LoadGameAsync(openFileDialog.FileName);
                }
            }
            catch (MillDataException)
            {
                MessageBox.Show("A fájl betöltése sikertelen!", "Malom", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog(); // dialógablak
                saveFileDialog.Title = "Malom betöltése";
                saveFileDialog.Filter = "Malom|*.mtl";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        if (_model != null)  await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (MillDataException)
                    {
                        MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("A fájl mentése sikertelen!", "Malom", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewModel_ExitGame(object? sender, System.EventArgs e)
        {
            if (_view != null) _view.Close(); 
        }

        #endregion

        #region Model event handlers

        private void Model_GameOver(object? sender, MillEventArgs e)
        {
            String strPlayer;
            strPlayer = e.CurrentPlayer == Player.Player1? "Első játékos" : "Második játékos";

            MessageBox.Show("Gratulálok, győztél " + strPlayer + "!",
                                "Malom játék",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
        }

        private void Model_PlayerHasToPass(object? sender, PassingEventArgs e)
        {
            String strPlayer;
            String action;
            strPlayer = e.CurrentPlayer == Player.Player1 ? "Első játékos" : "Második játékos";
            action = e.MoveToken == true ? "mozogni" : "ellenséges bábut levenni";
            MessageBox.Show(strPlayer + " passzolnod kell, nem tusz " + action + "!" + Environment.NewLine,
                                "Passzolj!",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
        }

        #endregion
    }
}
