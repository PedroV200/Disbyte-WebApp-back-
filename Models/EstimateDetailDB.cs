namespace WebApiSample.Models;

public class EstimateDetailDB
{
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
    public double ncm_arancelgrav { get; set; }         //DIE o Aranceles
    public double ncm_te_dta_otro { get; set; }         //TE o DTA
    public double ncm_iva { get; set; }
    public double ncm_ivaad { get; set; }
    public double gcias{get;set;}
    // Spares para resultados de calculos de los futuros impuestos
    public double calc_sp1{get;set;}
    public double calc_sp2{get;set;}
    public double calc_sp3{get;set;}
} 