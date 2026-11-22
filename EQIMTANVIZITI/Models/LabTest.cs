using EQIMTANVIZITI.Models.Entity;

namespace EQIMTANVIZITI.Models
{
    public class LabTest : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // many-to-many
        public List<Prescriptions> Prescriptions { get; set; } = new();
    }
}
