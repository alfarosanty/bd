using BlumeAPI.Entities;
using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class MedidaController : ControllerBase{

    private readonly ILogger<MedidaController> _logger;
    private readonly IMedidaService _medidaService;

    public MedidaController(ILogger<MedidaController> logger, IMedidaService medidaService)
    {
        _logger = logger;
        _medidaService = medidaService;
    }

[HttpGet]
    public async Task<IActionResult> Get()
    {
        var medidas = await _medidaService.getAll();
        return Ok(medidas);
    }



}