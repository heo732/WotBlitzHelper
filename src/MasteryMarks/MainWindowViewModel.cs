using MasteryMarks.WPF;
using System.Collections.ObjectModel;
using System.Linq;

namespace MasteryMarks
{
    public class MainWindowViewModel : BindableBase
    {
        private MasteryMarksLoader Loader { get; } = new MasteryMarksLoader();

        public ObservableCollection<TankMasteryMarks> Items { get; private set; } = new ObservableCollection<TankMasteryMarks>();

        public DelegateCommand ReloadCommand { get; }

        public DelegateCommand SettingsCommand { get; }

        public MainWindowViewModel()
        {
            ReloadCommand = new DelegateCommand(ReloadItems);
            SettingsCommand = new DelegateCommand(OpenSettings);
        }

        private void ReloadItems()
        {
            Items = new ObservableCollection<TankMasteryMarks>(Loader.GetMasteryMarks()
                .OrderBy(t => t.MarksGettingsProbability)
                .ThenBy(t => t.MarksEventPoints)
                .ThenBy(t => t.NumberOfBattles)
                .ThenBy(t => t.NumberOfMasteryMarks)
                .ThenBy(t => t.NumberOfMastery1Marks)
                .ThenBy(t => t.NumberOfMastery2Marks)
                .ThenBy(t => t.NumberOfMastery3Marks)
                .ThenBy(t => t.TankName));

            RaisePropertyChanged(nameof(Items));
        }

        private void OpenSettings()
        {
            string oldNickname = Loader.Nickname;
            string oldRegion = Loader.Region.Value;

            var viewModel = new SettingsViewModel(Loader);
            var view = new SettingsWindow(viewModel);
            view.ShowDialog();

            if ((oldNickname != Loader.Nickname) || (oldRegion != Loader.Region.Value))
            {
                Items.Clear();
            }
        }
    }
}