﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class Factura
    {
    public static String TABLA="FACTURA";
    public int Id { get; set; }

    public DateTime FechaFactura { get; set; }

    public Cliente Cliente { get; set; }

    public bool EximirIVA { get; set; }

    public List<ArticuloFactura>? Articulos  { get; set; }

    public Presupuesto? Presupuesto{ get; set; }

    public int? ImporteBruto  { get; set; }

    public int PuntoDeVenta { get; set; }

    public int? NumeroFactura { get; set; }

    public int? CaeNumero { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public int? ImporteNeto  { get; set; }

    public int? Iva { get; set; }

    public string TipoFactura   { get; set; }

    public int? DescuentoGeneral    { get; set; }

    }

