using EQIMTANVIZITI.Enums;
using EQIMTANVIZITI.Models.Entity;

namespace EQIMTANVIZITI.Models
{
    public class Reservation : BaseEntity
    {
        public decimal TotalPrice { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ReservationDate { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        // one-to-one: ვიზიტი -> რეცეპტი
        public Prescriptions? Prescription { get; set; }
    }
}
