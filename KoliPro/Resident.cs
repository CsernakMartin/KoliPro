using Postgrest.Attributes;
using Postgrest.Models;

namespace KoliPro
{
    [Table("residents")]
    public class Resident : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("room_number")]
        public string RoomNumber { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("photo_url")]
        public string PhotoUrl { get; set; }
    }
}