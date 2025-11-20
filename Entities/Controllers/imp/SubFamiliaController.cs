using BlumeApi.Models;
using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class SubFamiliaController : ControllerBase{

    private readonly ILogger<SubFamiliaController> _logger;
    private readonly ISubFamiliaService iSubFamiliaService;

    public SubFamiliaController(ILogger<SubFamiliaController> logger, ISubFamiliaService subFamiliaService)
    {
        _logger = logger;
        iSubFamiliaService = subFamiliaService;
    }

    [HttpGet("GetSubFamilias")]
    public async Task<IEnumerable<SubFamilia>> Get()
    {
        List<SubFamilia> subFamilias = await iSubFamiliaService.listarSubFamiliasAsync();
        return subFamilias;
    }



}