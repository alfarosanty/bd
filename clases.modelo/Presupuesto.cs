using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class Presupuesto
    {
    public static String TABLA="PRESUPUESTO";
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public Cliente Cliente { get; set; }

    public bool EximirIVA { get; set; }

    public EstadoPresupuesto EstadoPresupuesto{ get; set; }

    public Factura factura{ get; set; }

     
    public List<ArticuloPresupuesto> Articulos  { get; set; }
    
    public string RazonSocial{ get; set; }
    public string Contacto{ get; set; }
    public string Telefono{ get; set; }
    public string Domicilio{ get; set; }
    public string Localidad{ get; set; }
    public string Provincia { get; set; }
    public string Cuit{ get; set; }

    public CondicionFiscal CondicionFiscal{ get; set; }

    }

