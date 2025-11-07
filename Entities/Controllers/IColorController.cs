using Microsoft.AspNetCore.Mvc;

public interface IColorController
{
    Task<IActionResult> ListarColores();
}
