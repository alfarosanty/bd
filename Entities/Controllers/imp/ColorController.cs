using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ColorController : ControllerBase, IColorController
{
    private readonly ILogger<ColorController> _logger;
    private readonly IColorService _colorService;

    public ColorController(ILogger<ColorController> logger, IColorService colorService)
    {
        _logger = logger;
        _colorService = colorService;
    }

    [HttpGet("GetColores")]
    public async Task<IActionResult> ListarColores()
    {
        var colores = await _colorService.ListarColoresAsync();
        return Ok(colores);
    }
}
