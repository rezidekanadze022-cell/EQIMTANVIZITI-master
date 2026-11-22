using EQIMTANVIZITI.Models.Entity;

namespace EQIMTANVIZITI.Models
{
    public class Prescriptions : BaseEntity
    {
        public List<string> Medications { get; set; } = new();
        public string? Notes { get; set; }

        // many-to-many: Prescriptions <-> LabTest
        public List<LabTest> LabTests { get; set; } = new();

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;
    }
}
