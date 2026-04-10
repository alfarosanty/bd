namespace BlumeAPI.Services;
using BlumeAPI.Entities;
public interface IColorService
{
    Task<List<Color>> getAll();
}