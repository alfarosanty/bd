using System;

public class FECAERequest
{
    public FECAECabRequest FeCabReq { get; set; } = null!;
    public FECAEDetRequest[] FeDetReq { get; set; } = Array.Empty<FECAEDetRequest>();
}

public class FECAECabRequest
{
    public int CantReg { get; set; }
    public int PtoVta { get; set; }
    public int CbteTipo { get; set; }
}

public class FECAEDetRequest
{
    public int Concepto { get; set; }

    public int DocTipo { get; set; }
    public long DocNro { get; set; }

    public long CbteDesde { get; set; }
    public long CbteHasta { get; set; }

    /// <summary>
    /// Formato yyyyMMdd
    /// </summary>
    public string CbteFch { get; set; } = string.Empty;

    public decimal ImpTotal { get; set; }
    public decimal ImpTotConc { get; set; }
    public decimal ImpNeto { get; set; }
    public decimal ImpOpEx { get; set; }
    public decimal ImpTrib { get; set; }
    public decimal ImpIVA { get; set; }

    public string MonId { get; set; } = "PES";
    public decimal MonCotiz { get; set; }

    public AlicIva[]? Iva { get; set; }
}

public class AlicIva
{
    /// <summary>
    /// 5 = 21%
    /// 4 = 10.5%
    /// 6 = 27%
    /// 3 = 0%
    /// 8 = 5%
    /// 9 = 2.5%
    /// </summary>
    public int Id { get; set; }

    public decimal BaseImp { get; set; }
    public decimal Importe { get; set; }
}