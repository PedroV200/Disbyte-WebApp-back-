namespace WebApiSample.Models;
public class TarifasByDate
{
// NUEVO MODELO DE TABLA DE TARIFAS, QUE AGRUPA TODAS LA OTRAS TABLAS DE TARFIAS EN UNA SOLA ENTRADA HORIZONTAL
// Y POR FECHA.
    public int id { get; set; }
    public string description { get; set; }
    public int paisregionid {get;set;}
    public int freight_type {get;set;}
    // Terminal
    public int    terminal_provee{get;set;}
    public double terminal_gastoFijo {get;set;}
    public double terminal_gastoVariable{get;set;}
    // FWD / Agencia de TTE
    public int    fwdtte_provee{get;set;}
    public int    freight_fwdfrom {get; set;}
    public double freight_cost {get; set;}
    public double freight_gastos1   {get;set;}
    public double freight_gastos2 {get;set;}
    // Deposito
    public int    depo_provee { get; set; }
    public double depo_descarga {get; set;}
    public double depo_ingreso {get; set;}
    public double depo_totingreso{get; set;}
    public double depo_carga{get; set;}
    public double depo_armado{get;set;}
    public double depo_egreso{get;set;}
    public double depo_total_egreso{get;set;}
    // Flete local
    public int    flete_provee{get;set;}
    public double fleteint {get;set;}
    public double flete_devacio {get;set;}
    public double flete_demora {get;set;}
    public double flete_guarderia {get;set;}
    public double flete_totgastos {get;set;}
    public int    flete_trucksemiid{get;set;}
    // Poliza
    public int    poliza_provee { get; set; }
    public double poliza_prima {get;set;}
    public double poliza_demora{get;set;}
    public double poliza_impint {get;set;}
    public double poliza_sellos {get; set;}
    // Despachante
    public int   despachante_provee{get;set;}
    public double despa_fijo{get;set;}
    public double despa_variable{get;set;}
    public double despa_clasificacion{get;set;}
    public double despa_consultoria{get;set;}
    public double despa_total_gastos{get;set;}
    // Custodia
    public int    custodia_provee {get;set;}
    public double custodia_fact1 {get;set;}
    public double custodia_fact2 {get;set;}
    public double custodia_gastos{get;set;}
    // Bancario
    public int    banco_provee{get;set;}
    public double banco_fact1{get;set;}
    public double banco_fact2{get;set;}
    public double banco_gastos{get;set;}
    // Digitalizacion
    public int    gestdigdoc_provee{get;set;}
    public double    gestdigdoc_fact1{get;set;}
    public double    gestdigdoc_fact2{get;set;}
    public double gestdigdoc_gastos{get;set;}
    // Fecha de la entrada en la tabla
    public DateTime hTimeStamp {get; set;}
    }
 