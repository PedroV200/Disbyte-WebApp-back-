namespace WebApiSample.Models;

// VERSION II Ago 2023
// MODELO DE DATOS QUE SE ENVIA AL FRONT.
// Este modelo de datos representa la planilla de precosteo.
public class EstimateDetail
{
    // VALORES DADOS
    public int id {get; set;}
    public string modelo {get; set;}
    public int oemprovee {get; set;}
    public string sku {get; set;}
    public string imageurl {get; set;}
    public int ncm {get; set;}
    public double exw {get; set; }
    public double fobunit{get;set;}
    public int qty{get;set;}
    public int pcsctn{get;set;}
    public double cbmctn{get;set;}
    public double gwctn{get;set;}
    // Spares para valores ingresados.
    public string str_sp1{get;set;}
    public string str_sp2{get;set;}
    public double val_sp1{get;set;}
    public double val_sp2{get;set;}
    public double val_sp3{get;set;}
    // Fin spares
    public int estheaderid { get; set; }
    // Spares para futuros impuestos
    public double imp_sp1{get;set;}
    public double imp_sp2{get;set;}
    public double imp_sp3{get;set;}   
    // VALORES CALCULADOS
    public int ctns{get;set;}
    public double totalcbm{get;set;}
    public double totalgw{get;set;}
    public double totalfob{get;set;}
    public double factorproducto{get;set;}
    public double freightCharge{get;set;}
    public double insuranceCharge{get;set;}
    public double totalcif{get;set;}
    public double cifunit{get;set;}
    public double ratio_fob_cif{get;set;}
    public double ncm_arancelgrav { get; set; }         //DIE o Aranceles
    public double arancelgrav_cif{get;set;}             // Derechos
    public double ncm_te_dta_otro { get; set; }         //TE o DTA
    public double te_dta_otro_cif{get;set;}             // Te061
    public double baseiva{get;set;}                     
    public double baseiva_unit{get;set;}
    public double ncm_iva { get; set; }
    public double iva_cif{get;set;}
    public double ncm_ivaad { get; set; }
    public double ivaad_cif{get;set;}
    public double gcias{get;set;}
    public double gcias424{get;set;}
    public double iibb900{get;set;}
    // Spares para resultados de calculos de los futuros impuestos
    public double calc_sp1{get;set;}
    public double calc_sp2{get;set;}
    public double calc_sp3{get;set;}
    public double totalaranceles{get;set;}
    public double gloc_fwd{get;set;}
    public double gloc_terminal{get;set;}
    public double gloc_flete{get;set;}
    public double gloc_cust{get;set;}
    public double gloc_despachante{get;set;}
    public double gloc_banco{get;set;}
    //SPARES
    public double gloc_sp1{get;set;}
    public double gloc_sp2{get;set;}
    public double gloc_sp3{get;set;}
    //FIN SPARES
    public double preciounit_uss{get;set;}
    public double totalgastosloc_uss{get;set;}
    public double totaltraderfee{get;set;}
    public double extragasto{get;set;}
    public double overhead{get;set;}
    public double costounituss{get;set;}
    public double costounit{get;set;}
    public double ratiopricing{get;set;}
    public double fobtocosto{get;set;}
}