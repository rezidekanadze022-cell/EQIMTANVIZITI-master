using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EQIMTANVIZITI.Controllers;

public class DoctorsController : ApiControllerBase
{
    private readonly IDoctorService _service;
    public DoctorsController(IDoctorService service) => _service = service;

    [HttpGet("get-all-doctors")] public async Task<IActionResult> GetAll([FromQuery] string? specialization) => Ok(await _service.GetAllAsync(specialization));
    [HttpGet("get-doctors-by-id/{id:int}")] public async Task<IActionResult> Get(int id) => Ok(await _service.GetAsync(id));

    [Authorize(Roles = "Admin")]
    [HttpPost("add-doctor")] public async Task<IActionResult> Create(DoctorCreateRequest r) => Ok(await _service.CreateAsync(r));

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete-doctor-by-id/{id:int}")] public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return NoContent(); }
}
