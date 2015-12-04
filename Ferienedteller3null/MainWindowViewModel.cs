using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Ferienedteller3null.Properties;

using Color = System.Drawing.Color;

namespace Ferienedteller3null
{

    public class MainWindowViewModel : ViewModelBase
    {
        readonly DateTime _chirstmasRings = new DateTime(2015,12,24,16,0,0);

        public MainWindowViewModel()
        {
            SelectedDate = Settings.Default.SelectedVacationData;
           
            UserName = Settings.Default.UserName;
            var newUsername = string.Empty;
            if (UserName.Equals("Hei, Julenisse!"))
                newUsername = UserPrincipal.Current.DisplayName;
            if (!string.IsNullOrEmpty(newUsername))
                UserName = newUsername;
            
            StartTimer();
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (!propertyChangedEventArgs.PropertyName.Equals(nameof(SelectedDate))) return;
            if (SelectedDate.Hour != 16)
            {
                SelectedDate = new DateTime(SelectedDate.Year,SelectedDate.Month,SelectedDate.Day,16,0,0);
            }
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        private string _restTimeToVacation;
        public string RestTimeToVacationString
        {
            get { return _restTimeToVacation; }
            set
            {
                _restTimeToVacation = value;
                OnPropertyChanged(nameof(RestTimeToVacationString));
            }
        }

        private string _restTimeToChristmasString;
        public string RestTimeToChristmasString
        {
            get { return _restTimeToChristmasString; }
            set
            {
                _restTimeToChristmasString = value;
                OnPropertyChanged(nameof(RestTimeToChristmasString));
            }
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
               Settings.Default.SelectedVacationData = value;
                Settings.Default.Save();
                OnPropertyChanged(nameof(SelectedDate));
            }
        }


        private ImageSource _selectedImageSource;

        public ImageSource SelectedImageSources
        {
            get { return _selectedImageSource; }
            set
            {
                _selectedImageSource = value;
                OnPropertyChanged(nameof(SelectedImageSources));
            }
        }

        private ObservableCollection<ImageSourceStore> _themeSources;

        public ObservableCollection<ImageSourceStore> ThemeSources
        {
            get { return _themeSources; }
            set
            {
                _themeSources = value;
                OnPropertyChanged(nameof(ThemeSources));
            }
        }


        private void StartTimer()
        {

            
            var timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1)};
            timer.Tick += TimerOnTick;
            timer.Start();



        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            
            var deltaVacation = SelectedDate - DateTime.Now;
            var days = deltaVacation.Days;
            var hours = deltaVacation.Hours;
            var min = deltaVacation.Minutes;
            var sec = deltaVacation.Seconds;
            var dagStreng = (days == 1) ? "dag" : "dager";
            var timestring = (hours == 1) ? "time" : "timer";
            var minstring = (min == 1) ? "minutt" : "minutter";
            var sekstring = (sec == 1) ? "sekund" : "sekunder";

            RestTimeToVacationString = $"{days} {dagStreng} {hours} {timestring} {min} {minstring} {sec} {sekstring}";

            var deltaChristmas = _chirstmasRings - DateTime.Now;
            var ddays = deltaChristmas.Days;
            var dhours = deltaChristmas.Hours;
            var dmin = deltaChristmas.Minutes;
            var dsec = deltaChristmas.Seconds;
            var ddagStreng = (ddays == 1) ? "dag" : "dager";
            var dtimestring = (dhours == 1) ? "time" : "timer";
            var dminstring = (dmin == 1) ? "minutt" : "minutter";
            var dsekstring = (dsec == 1) ? "sekund" : "sekunder";
            RestTimeToChristmasString = $"{ddays} {ddagStreng} {dhours} {dtimestring} {dmin} {dminstring} {dsec} {dsekstring}";


        }
    }
}
