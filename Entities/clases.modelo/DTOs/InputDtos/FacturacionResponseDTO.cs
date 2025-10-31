public class FEDetResponse
{
    public string Resultado { get; set; }   // "A" aprobado, "R" rechazado
    public string CAE { get; set; }
    public string CAEFchVto { get; set; }
    public long CbteDesde { get; set; }
    public long CbteHasta { get; set; }
}

public class FeCabResp
{
    public long Cuit { get; set; }
    public int PtoVta { get; set; }
    public int CbteTipo { get; set; }
    public string FchProceso { get; set; }
    public int CantReg { get; set; }
    public string Resultado { get; set; }
}

public class FECAESolicitarResult
{
    public FeCabResp FeCabResp { get; set; }
    public List<FEDetResponse> FeDetResp { get; set; }
}

public class FECAESolicitarResponse
{
    public FECAESolicitarResult FECAESolicitarResult { get; set; }
}
