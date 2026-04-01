using BlumeAPI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class SubFamiliaController : ControllerBase{

    private readonly ILogger<SubFamiliaController> _logger;

    public SubFamiliaController(ILogger<SubFamiliaController> logger)
    {
        _logger = logger;
    }



}