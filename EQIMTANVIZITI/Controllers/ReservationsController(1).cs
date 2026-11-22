using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EQIMTANVIZITI.Controllers;

public class ReservationsController : ApiControllerBase
{
    private readonly IReservationService _service;
    public ReservationsController(IReservationService service) => _service = service;

    /// ვიზიტის დაჯავშნა (პაციენტი)
    [Authorize(Roles = "Patient")]
    [HttpPost] public async Task<IActionResult> Create(ReservationCreateRequest r) => Ok(await _service.CreateAsync(CurrentUserId, r));

    /// ჩემი ვიზიტები (პაციენტი)
    [Authorize(Roles = "Patient")]
    [HttpGet("my")] public async Task<IActionResult> My() => Ok(await _service.GetMyAsync(CurrentUserId));

    /// ჩემი ვიზიტის გაუქმება (პაციენტი)
    [Authorize(Roles = "Patient")]
    [HttpPut("{id:int}/cancel")] public async Task<IActionResult> Cancel(int id) => Ok(await _service.CancelAsync(CurrentUserId, id));

    /// ჩემთან დაჯავშნილი ვიზიტები (ექიმი)
    [Authorize(Roles = "Doctor")]
    [HttpGet("doctor")] public async Task<IActionResult> ForDoctor() => Ok(await _service.GetForDoctorAsync(CurrentUserId));

    /// ყველა ვიზიტი (ადმინი)
    [Authorize(Roles = "Admin")]
    [HttpGet] public async Task<IActionResult> All() => Ok(await _service.GetAllAsync());

    /// სტატუსის შეცვლა (ექიმი - თავისი ვიზიტის, ადმინი - ნებისმიერის)
    [Authorize(Roles = "Doctor,Admin")]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, ReservationStatusUpdateRequest r) =>
        Ok(await _service.UpdateStatusAsync(CurrentUserId, IsAdmin, id, r.Status));
}
