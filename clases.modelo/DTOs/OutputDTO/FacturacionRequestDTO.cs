public class FEAuthRequest
{
    public string Token { get; set; }
    public string Sign { get; set; }
    public long Cuit { get; set; }
}

public class FeCabReq
{
    public int CantReg { get; set; }
    public int PtoVta { get; set; }
    public int CbteTipo { get; set; } // 1 = Factura A, 6 = Factura B, etc.
}

public class AlicIva
{
    public short Id { get; set; }       // 5 = 21%, 4 = 10.5%, etc.
    public double BaseImp { get; set; }
    public double Importe { get; set; }
}

public class FECAEDetRequest
{
    public int Concepto { get; set; }   // 1=Productos, 2=Servicios, 3=Ambos
    public int DocTipo { get; set; }    // 80=CUIT, 96=DNI, etc.
    public long DocNro { get; set; }
    public long CbteDesde { get; set; }
    public long CbteHasta { get; set; }
    public string CbteFch { get; set; }
    public double ImpTotal { get; set; }
    public double ImpTotConc { get; set; }
    public double ImpNeto { get; set; }
    public double ImpOpEx { get; set; }
    public double ImpIVA { get; set; }
    public double ImpTrib { get; set; }
    public string MonId { get; set; } = "PES";
    public double MonCotiz { get; set; } = 1.0;
    public int CondicionIVAReceptorId { get; set; } = 0; // 0 = no informado


    public List<AlicIva> Iva { get; set; } = new();
}

public class FeCAEReq
{
    public FeCabReq FeCabReq { get; set; }
    public List<FECAEDetRequest> FeDetReq { get; set; } = new();
}

public class FECAESolicitarRequest
{
    public FEAuthRequest Auth { get; set; }
    public FeCAEReq FeCAEReq { get; set; }
}