using BlumeAPI.Entities;
using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class SubFamiliaController : ControllerBase{

    private readonly ILogger<SubFamiliaController> _logger;
    private readonly ISubfamiliaService _subfamiliaService;

    public SubFamiliaController(ILogger<SubFamiliaController> logger, ISubfamiliaService subfamiliaService)
    {
        _logger = logger;
        _subfamiliaService = subfamiliaService;
    }

[HttpGet]
    public async Task<IActionResult> Get()
    {
        var subfamilias = await _subfamiliaService.getAll();
        return Ok(subfamilias);
    }



}