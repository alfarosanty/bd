using BlumeApi.Models;
using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class MedidaController : ControllerBase{

    private readonly ILogger<MedidaController> _logger;
    private readonly IMedidaService iMedidaService;

    public MedidaController(ILogger<MedidaController> logger, IMedidaService medidaService)
    {
        _logger = logger;
        iMedidaService = medidaService;
    }


    [HttpGet("GetMedidas")]
    public async Task<IEnumerable<Medida>> Get()
    {
        List<Medida> medidas = await iMedidaService.GetMedidasAsync();
        return medidas;
    }



}