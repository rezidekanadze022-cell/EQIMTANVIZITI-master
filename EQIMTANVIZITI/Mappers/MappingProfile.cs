namespace EQIMTANVIZITI.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterRequest, User>()
            .ForMember(d => d.Password, o => o.Ignore())
            .ForMember(d => d.Email, o => o.Ignore());

        CreateMap<Doctor, DoctorResponse>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.UserId))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.User.PhoneNumber));

        CreateMap<Reservation, ReservationResponse>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.User.FullName))
            .ForMember(d => d.Specialization, o => o.MapFrom(s => s.Doctor.Specialization))
            .ForMember(d => d.PatientId, o => o.MapFrom(s => s.UserId))
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.User.FullName));

        CreateMap<LabTestCreateRequest, LabTest>();
        CreateMap<LabTest, LabTestResponse>();

        CreateMap<Prescriptions, PrescriptionResponse>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.User.FullName))
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.User.FullName));
    }
}