using Engine;
using MasteryMarks.WPF;

namespace MasteryMarks
{
    public class SettingsViewModel : BindableBase
    {
        private MasteryMarksLoader Loader { get; } = new MasteryMarksLoader();

        public Region Region
        {
            get => Loader.Region;
            set
            {
                Loader.Region = value;
                RaisePropertyChanged(nameof(Region));
            }
        }

        public string Nickname
        {
            get => Loader.Nickname;
            set
            {
                Loader.Nickname = value;
                RaisePropertyChanged(nameof(Nickname));
            }
        }

        public SettingsViewModel(MasteryMarksLoader loader)
        {
            Loader = loader;
        }
    }
}