namespace WebApiSample.Models;
public class EstimateHeaderDB
{ 
    public int  id{get;set;}
    public string description{get;set;}
    public int estnumber{get;set;}
    public int estvers{get;set;}
    public int status{get;set;}
    public int paisregion_id{get;set;}
    public int carga_id{get;set;}
    public int fwdpaisregion_id{get;set;}
    public string own{get;set;}
    public double dolar{get;set;}
    public int tarifasfwd_id{get;set;}
    public int tarifasflete_id{get;set;}
    public int tarifasterminales_id{get;set;}
    public int tarifaspolizas_id{get;set;}
    public int tarifasdepositos_id{get;set;}
    public int tarifasdespachantes_id{get;set;}
    public int tarifasbancos_id{get;set;}
    public int tarifasgestdigdoc_id{get;set;}
    public double gasto_otro1{get;set;}
    public double gasto_otro2{get;set;}
    public double gasto_otro3{get;set;}
    public string gasto_otro_description{get;set;}
    public int constantes_id{get;set;}
    public bool ivaexcento{get;set;}
    public double fob_grand_total{get;set;}
    public double cbm_grand_total{get;set;}
    public double gw_grand_total{get;set;}
    public double cif_grand_total{get;set;}
    public double gastos_loc_total{get;set;}
    public double extragastos_total{get;set;}
    public double impuestos_total{get;set;}
    public double cantidad_contenedores{get;set;}
    public double iibb_total{get;set;}
    public DateTime htimestamp{get;set;}
    
}