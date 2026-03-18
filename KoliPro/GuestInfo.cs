using Postgrest.Attributes;
using Postgrest.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KoliPro
{
    [Table("guests")]
    public class GuestInfo : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("card_id")]
        public string CardId { get; set; }

        [Column("room")]
        public string Room { get; set; }

        [Column("arrive_time")]
        public string ArriveTime { get; set; }

        private string _leaveTime;
        [Column("leave_time")]
        public string LeaveTime
        {
            get => _leaveTime;
            set
            {
                if (_leaveTime != value)
                {
                    _leaveTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}