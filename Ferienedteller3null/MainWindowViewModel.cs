using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.Windows.Threading;
using Ferienedteller3null.Properties;

namespace Ferienedteller3null
{

    public class MainWindowViewModel : ViewModelBase
    {
        private DateTime _dateAndHourToCountDownTo = new DateTime(2015, 12, 24, 16, 0, 0);
        private DispatcherTimer _timer;
        public MainWindowViewModel()
        {
            Settings.Default.ChristmasRingsDate = _dateAndHourToCountDownTo;
            Settings.Default.Save();

            ThemeSources = new ObservableCollection<string> { "Standard", "Lofoten", "StarWars - DarkSide", "StarWars - Christmas" };
            SelectedImageSource = Settings.Default.SelectedTheme;
            UpdateStringsAndDates();
            UserName = Settings.Default.UserName;
            var newUsername = string.Empty;
            try
            {
                if (UserName.Equals("Hei, Julenisse!"))
                    newUsername = UserPrincipal.Current.DisplayName;
                if (!string.IsNullOrEmpty(newUsername))
                    UserName = newUsername;
            }
            catch (Exception e)
            {

            }


            UseShortTextTimeString = Settings.Default.UseShortStrings;

            StartTimer();
            PropertyChanged += OnPropertyChanged;

        }

       private void UpdateStringsAndDates()
        {
            FirstString = SelectedImageSource.Contains("StarWars") ? Resources.StarWars_MyDate : Resources.Christmas_Vacation;
            SecondString = SelectedImageSource.Contains("StarWars") ? Resources.StarWars_Premiere : Resources.Chrismas_ChristmasRingin;
            LastString = SelectedImageSource.Contains("StarWars") ? Resources.StarWars_mayTheForceBeWithYou : Resources.Christmas_MerryChristmas;
           _dateAndHourToCountDownTo = SelectedImageSource.Contains("StarWars")
               ? Settings.Default.StarWarsPremierDate
               : Settings.Default.ChristmasRingsDate;
            SelectedDate = SelectedImageSource.Contains("StarWars") ? Settings.Default.StarWarsMyDate : Settings.Default.SelectedVacationData;
            SelectedVacationHour = SelectedImageSource.Contains("StarWars") ? Settings.Default.StarWarsMyHour : Settings.Default.SelectedVacationData;

        }

        

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals(nameof(SelectedVacationHour)))
            {
                SelectedDate = new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day,
                    SelectedVacationHour.Hour, SelectedVacationHour.Minute, SelectedVacationHour.Second);
            }


            if (propertyChangedEventArgs.PropertyName.Equals(nameof(SelectedImageSource)))
            {

                UpdateStringsAndDates();
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

        private string _firstString;

        public string FirstString
        {
            get { return _firstString; }
            set
            {
                _firstString = value;
                OnPropertyChanged(nameof(FirstString));
            }
        }

        private string _secondString;

        public string SecondString
        {
            get { return _secondString; }
            set
            {
                _secondString = value;
                OnPropertyChanged(nameof(SecondString));
            }
        }

        private string _lastSTring;

        public string LastString
        {
            get { return _lastSTring; }
            set
            {
                _lastSTring = value;
                OnPropertyChanged(nameof(LastString));
            }
        }



        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                if (SelectedImageSource.Contains("StarWars"))
                    Settings.Default.StarWarsMyDate = value;
                else
                    Settings.Default.SelectedVacationData = value;


                OnPropertyChanged(nameof(SelectedDate));
            }
        }


        private string _selectedImageSource;

        public string SelectedImageSource
        {
            get { return _selectedImageSource; }
            set
            {
                _selectedImageSource = value;
                Settings.Default.SelectedTheme = value;
                OnPropertyChanged(nameof(SelectedImageSource));
            }
        }

        private ObservableCollection<string> _themeSources;

        public ObservableCollection<string> ThemeSources
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
                if (SelectedImageSource.Contains("StarWars"))
                    Settings.Default.StarWarsMyHour = value;
                else
                    Settings.Default.selectedVacationHour = value;
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


        private bool _useStarWarsCountDown;

        public bool StarWarsCountDown
        {
            get { return _useStarWarsCountDown; }
            set
            {
                _useStarWarsCountDown = value;
                Settings.Default.StarWarsCountDown = value;
                OnPropertyChanged(nameof(StarWarsCountDown));
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

            var deltaChristmas = _dateAndHourToCountDownTo - DateTime.Now;
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
