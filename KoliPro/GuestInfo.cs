using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KoliPro
{
    public class GuestInfo : INotifyPropertyChanged
    {
        private string _leaveTime;

        public string Name { get; set; }
        public string CardId { get; set; }
        public string Room { get; set; }
        public string ArriveTime { get; set; }

        public string LeaveTime
        {
            get => _leaveTime;
            set
            {
                if (_leaveTime != value)
                {
                    _leaveTime = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsOut));
                    OnPropertyChanged(nameof(CanCheckout));
                }
            }
        }

        public bool IsOut => LeaveTime != "---";
        public bool CanCheckout => LeaveTime == "---";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}