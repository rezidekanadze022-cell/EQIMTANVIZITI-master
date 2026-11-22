namespace EQIMTANVIZITI.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterRequest, User>()
            .ForMember(d => d.Password, o => o.Ignore())   // hash-ს სერვისში ვწერთ
            .ForMember(d => d.Email, o => o.Ignore());      // lowercase-ს სერვისში ვწერთ

        CreateMap<Doctor, DoctorResponse>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.UserId))
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.User.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.User.LastName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.User.PhoneNumber));

        CreateMap<Reservation, ReservationResponse>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.User.FirstName + " " + s.Doctor.User.LastName))
            .ForMember(d => d.Specialization, o => o.MapFrom(s => s.Doctor.Specialization))
            .ForMember(d => d.PatientId, o => o.MapFrom(s => s.UserId))
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName));

        CreateMap<LabTestCreateRequest, LabTest>();
        CreateMap<LabTest, LabTestResponse>();

        CreateMap<Prescriptions, PrescriptionResponse>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.User.FirstName + " " + s.Doctor.User.LastName))
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName));
    }
}
