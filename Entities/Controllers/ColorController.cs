using BlumeAPI.Entities;
using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class ColorController : ControllerBase{

    private readonly ILogger<ColorController> _logger;
    private readonly IColorService _colorService;

    public ColorController(ILogger<ColorController> logger, IColorService colorService)
    {
        _logger = logger;
        _colorService = colorService;
    }


[HttpGet]
    public async Task<IActionResult> Get()
    {
        var colores = await _colorService.getAll();
        return Ok(colores);  
        
    }



}