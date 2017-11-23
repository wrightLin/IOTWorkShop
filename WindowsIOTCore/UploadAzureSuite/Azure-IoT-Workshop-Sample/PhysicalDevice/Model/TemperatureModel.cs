using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PhysicalDevice.Model
{
    public class TemperatureModel : INotifyPropertyChanged
    {
        double _temperature = 0;
        bool _started = false;
        string _systemSymbol = "C";
        string _statusText = "";

        public double Temperature
        {
            get { return _temperature; }
            set
            {
                if (value != _temperature)
                {
                    _temperature = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool Started
        {
            get { return _started; }
            set
            {
                if (value != _started)
                {
                    _started = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("Stopped");
                }
            }
        }
        public bool Stopped
        {
            get { return !Started; }
        }
        public string SystemSymbol
        {
            get { return _systemSymbol; }
            set
            {
                if (value != _systemSymbol)
                {
                    _systemSymbol = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string StatusText
        {
            get { return _statusText; }
            set
            {
                if (value != _statusText)
                {
                    _statusText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
