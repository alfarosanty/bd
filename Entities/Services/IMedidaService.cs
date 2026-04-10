namespace BlumeAPI.Services;
using BlumeAPI.Entities;
public interface IMedidaService
{
    Task<List<Medida>> getAll();
}