using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class StockController : ControllerBase
{
    List<Stock> stList = new List<Stock>();
    private readonly ILogger<StockController> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly string stListKey = "stListKey";
    
    public StockController(ILogger<StockController> logger, IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;
    }


    [HttpGet("GetStock")]
    public IEnumerable<Stock> GetStock()
    {
        try{
            

            // Si ya esta en la cache la retorno
            if (_memoryCache.TryGetValue(stListKey, out stList))
            {
                return stList;
            }

            stList = new List<Stock>();

            // Cargo una lista de ejemplo (no tenog la db)
            Stock st = new Stock();
            st.id = 1;
            st.cantidad = 4533;
            st.producto = "Tela Moja";
            st.baja = false;
            stList.Add(st);
            Stock st1 = new Stock();
            st1.id = 2;
            st1.cantidad = 4231;
            st1.producto = "Tela Hace";
            st1.baja = false;
            stList.Add(st1);
            Stock st2 = new Stock();
            st2.id = 3;
            st2.cantidad = 123;
            st2.producto = "Tela Palpita";
            st2.baja = false;
            stList.Add(st2);
            Stock st3 = new Stock();
            st3.id = 4;
            st3.cantidad = 44324;
            st3.producto = "Tela Rompe";
            st3.baja = false;
            stList.Add(st3);

            // 900 segundos que viva en cache
            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(900));

            // Guardo el objeto en la cache
            _memoryCache.Set(stListKey, stList, cacheOptions);
            
            return stList;
        }

        catch(Exception ex)
        {
            return null;
        }
    }

    [HttpGet("GetById/{id}")]
    public Stock GetById(int id)
    {
        try{
            
            IEnumerable<Stock> resultado = stList;

            if (_memoryCache.TryGetValue(stListKey, out stList))
            {
                resultado = stList.Where(a => a.id == id);
            }

            return resultado.First<Stock>();
        }
        catch(Exception ex)
        {
            return null;
        }
    }

    [HttpGet("GetByProd/{producto}")]
    public IEnumerable<Stock> GetByProd(string producto)
    {
        try{
            
            IEnumerable<Stock> resultado = stList;

            if (_memoryCache.TryGetValue(stListKey, out stList))
            {
                resultado = stList.Where(a => a.producto.Contains(producto));
            }

            return resultado;
        }
        catch(Exception ex)
        {
            return null;
        }
    }

    [HttpPost("PostStock")]
    public async Task<ActionResult<Stock>> PostStock(Stock stock)
    {
        try{
            if (_memoryCache.TryGetValue(stListKey, out stList))
            {
                stList.Add(stock);
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(900));
                _memoryCache.Set(stListKey, stList, cacheOptions);
            }

            return stock;
        }
        catch(Exception ex)
        {
            return null;
        }
    }

    [HttpDelete("DeleteStock/{id}")]
    public async Task<ActionResult<Stock>> DeleteStock(int id)
    {
            IEnumerable<Stock> resultado = stList;

            if (_memoryCache.TryGetValue(stListKey, out stList))
            {
                resultado = stList.Where(a => a.id == id);
                stList.Remove(resultado.First<Stock>());
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(900));
                _memoryCache.Set(stListKey, stList, cacheOptions);
            }

            return stList[0];
    }

    [HttpPost("UpdateStock")]
    public async Task<ActionResult<Stock>> UpdateStock(Stock stock)
    {
        try{
            
            IEnumerable<Stock> resultado = stList;

            if (_memoryCache.TryGetValue(stListKey, out stList))
            {
                resultado = stList.Where(a => a.id == stock.id);
                stList.Remove(resultado.First<Stock>());
                stList.Add(stock);
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(900));
                _memoryCache.Set(stListKey, stList, cacheOptions);
            }

            return resultado.First<Stock>();
        }
        catch(Exception ex)
        {
            return null;
        }
    }

}
