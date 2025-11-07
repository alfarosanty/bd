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

    public DateTime? Fecha { get; set; }

    public Cliente? Cliente { get; set; }

    public bool? EximirIVA { get; set; }

    public EstadoPresupuesto? EstadoPresupuesto{ get; set; }

    public Factura? factura{ get; set; }

     
    public List<ArticuloPresupuesto>? Articulos  { get; set; }

    public float? descuentoGeneral {get; set; }
 
    }

