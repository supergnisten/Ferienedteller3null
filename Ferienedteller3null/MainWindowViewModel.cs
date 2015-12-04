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
        private DispatcherTimer _timer; 
        public MainWindowViewModel()
        {
            SelectedDate = Settings.Default.SelectedVacationData;
            UserName = Settings.Default.UserName;
            var newUsername = string.Empty;
            if (UserName.Equals("Hei, Julenisse!"))
                newUsername = UserPrincipal.Current.DisplayName;
            if (!string.IsNullOrEmpty(newUsername))
                UserName = newUsername;

            UseShortTextTimeString = Settings.Default.UseShortStrings;
            SelectedVacationHour = Settings.Default.SelectedVacationData;

            StartTimer();
            PropertyChanged += OnPropertyChanged;
            ClosingRequest += OnClosingRequest;
        }

        private void OnClosingRequest(object sender, EventArgs eventArgs)
        {
        _timer?.Stop();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals(nameof(SelectedVacationHour)))
            {
                SelectedDate = new DateTime(SelectedDate.Year,SelectedDate.Month,SelectedDate.Day,
                    SelectedVacationHour.Hour,SelectedVacationHour.Minute,SelectedVacationHour.Second);
            }

            Settings.Default.Save();
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


        private bool _useShortTextTimeString;

        public bool UseShortTextTimeString
        {
            get { return _useShortTextTimeString; }
            set
            {
                _useShortTextTimeString = value;
                Settings.Default.UseShortStrings = value;
                OnPropertyChanged(nameof(UseShortTextTimeString));
            }
        }
 
        private DateTime _selectedHours;

        public DateTime SelectedVacationHour
        {
            get { return _selectedHours; }
            set
            {
                _selectedHours = value;
                OnPropertyChanged(nameof(SelectedVacationHour));
            }
        }

        private bool _alwaysOnTop;

        public bool AlwaysOnTop
        {
            get { return _alwaysOnTop; }
            set
            {
                _alwaysOnTop = value;
                Settings.Default.AllwaysOnTop = value;
                OnPropertyChanged(nameof(AlwaysOnTop));
            }
        }
 

        private void StartTimer()
        {
            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1) };
            _timer.Tick += TimerOnTick;
            _timer.Start();
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
            var longString = $"{days} {dagStreng} {hours} {timestring} {min} {minstring} {sec} {sekstring}";
            var shortstring = $"{days} D {hours} H {min} m {sec} s";

            RestTimeToVacationString = UseShortTextTimeString ? shortstring : longString;

            var deltaChristmas = _chirstmasRings - DateTime.Now;
            var ddays = deltaChristmas.Days;
            var dhours = deltaChristmas.Hours;
            var dmin = deltaChristmas.Minutes;
            var dsec = deltaChristmas.Seconds;
            var ddagStreng = (ddays == 1) ? "dag" : "dager";
            var dtimestring = (dhours == 1) ? "time" : "timer";
            var dminstring = (dmin == 1) ? "minutt" : "minutter";
            var dsekstring = (dsec == 1) ? "sekund" : "sekunder";
            var dLongString = $"{ddays} {ddagStreng} {dhours} {dtimestring} {dmin} {dminstring} {dsec} {dsekstring}";
            var dshortstring = $"{ddays} D {dhours} H {dmin} m {dsec} s";

            RestTimeToChristmasString = UseShortTextTimeString ? dshortstring : dLongString;


        }
    }
}
