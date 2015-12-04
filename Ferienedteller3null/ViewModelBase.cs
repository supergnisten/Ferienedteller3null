using System;
using System.ComponentModel;

namespace Ferienedteller3null
{

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public event EventHandler ClosingRequest;

        protected void OnClosingRequest()
        {
            if (ClosingRequest != null)
            {
                ClosingRequest(this, EventArgs.Empty);
            }
        }
    }

}
