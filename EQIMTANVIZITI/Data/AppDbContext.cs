using EQIMTANVIZITI.Models.Entity;

namespace EQIMTANVIZITI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Prescriptions> Prescriptions { get; set; }
    public DbSet<LabTest> LabTests { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.Property(x => x.Email).HasMaxLength(150);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
        });

        // one-to-one: User - Doctor (Doctor.UserId არის PK და FK)
        b.Entity<Doctor>(e =>
        {
            e.HasKey(d => d.UserId);
            e.Property(d => d.ConsultationPrice).HasPrecision(10, 2);
            e.HasOne(d => d.User).WithOne(u => u.DoctorProfile)
             .HasForeignKey<Doctor>(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // one-to-many: User/Doctor -> Reservations
        b.Entity<Reservation>(e =>
        {
            e.Property(x => x.TotalPrice).HasPrecision(10, 2);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.HasOne(r => r.User).WithMany(u => u.Reservations)
             .HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Doctor).WithMany(d => d.Reservations)
             .HasForeignKey(r => r.DoctorId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Prescriptions>(e =>
        {
            e.HasOne(p => p.User).WithMany(u => u.Prescriptions)
             .HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Doctor).WithMany(d => d.Prescriptions)
             .HasForeignKey(p => p.DoctorId).OnDelete(DeleteBehavior.Restrict);
            // one-to-one: Reservation - Prescriptions
            e.HasOne(p => p.Reservation).WithOne(r => r.Prescription)
             .HasForeignKey<Prescriptions>(p => p.ReservationId).OnDelete(DeleteBehavior.Restrict);
            // many-to-many: Prescriptions <-> LabTest (join table ავტომატურად შეიქმნება)
            e.HasMany(p => p.LabTests).WithMany(l => l.Prescriptions);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>().Where(x => x.State == EntityState.Modified))
            entry.Entity.UpdatedAt = DateTime.Now;
        return base.SaveChangesAsync(cancellationToken);
    }
}
