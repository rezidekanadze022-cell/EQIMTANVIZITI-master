using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EQIMTANVIZITI.Controllers;

public class PrescriptionsController : ApiControllerBase
{
    private readonly IPrescriptionService _service;
    public PrescriptionsController(IPrescriptionService service) => _service = service;

    /// რეცეპტის დაწერა (ექიმი) - ვიზიტი ხდება Completed
    [Authorize(Roles = "Doctor")]
    [HttpPost] public async Task<IActionResult> Create(PrescriptionCreateRequest r) => Ok(await _service.CreateAsync(CurrentUserId, r));

    /// ჩემი რეცეპტები (პაციენტი)
    [Authorize(Roles = "Patient")]
    [HttpGet("my")] public async Task<IActionResult> My() => Ok(await _service.GetMyAsync(CurrentUserId));

    /// ჩემს მიერ დაწერილი რეცეპტები (ექიმი)
    [Authorize(Roles = "Doctor")]
    [HttpGet("issued")] public async Task<IActionResult> Issued() => Ok(await _service.GetIssuedAsync(CurrentUserId));

    /// რეცეპტი ვიზიტის მიხედვით (პაციენტი/ექიმი/ადმინი - მხოლოდ თავისი)
    [HttpGet("by-reservation/{reservationId:int}")]
    public async Task<IActionResult> ByReservation(int reservationId) =>
        Ok(await _service.GetByReservationAsync(CurrentUserId, IsAdmin, reservationId));
}
