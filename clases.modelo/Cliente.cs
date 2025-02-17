using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class Cliente
    {
    public static String TABLA="CLIENTE";
    public int Id { get; set; }
    public string RazonSocial{ get; set; }
    public string Contacto{ get; set; }
    public string Telefono{ get; set; }
    public string Domicilio{ get; set; }
    public string Localidad{ get; set; }
    public string Provincia { get; set; }
    public string Cuit{ get; set; }

    public CondicionFiscal CondicionFiscal{ get; set; }

    }

