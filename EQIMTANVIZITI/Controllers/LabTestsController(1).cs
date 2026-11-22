using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EQIMTANVIZITI.Controllers;

public class LabTestsController : ApiControllerBase
{
    private readonly ILabTestService _service;
    public LabTestsController(ILabTestService service) => _service = service;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? search) => Ok(await _service.GetAllAsync(search));

    [Authorize(Roles = "Admin")]
    [HttpPost] public async Task<IActionResult> Create(LabTestCreateRequest r) => Ok(await _service.CreateAsync(r));
}
