namespace WebApiSample.Models;

public class EstimateDetailDB
{
    public int id{get;set;}
    public int estimateheader_id{get;set;}
    public int proveedores_id{get;set;}
    public int ncm_id{get;set;}
    public bool ncm_ack{get;set;}
    public string sku{get;set;}
    public string description{get;set;}
    public string imageurl{get;set;}
    public double exw_u{get;set;}
    public double fob_u{get;set;}
    public int qty{get;set;}
    public int pcsctn{get;set;}
    public double cbmctn{get;set;}
    public double gwctn{get;set;}
    public string cambios_notas{get;set;}
    public double ncm_arancel{get;set;}
    public double ncm_te_dta_otro{get;set;}
    public double ncm_iva{get;set;}
    public double ncm_ivaad{get;set;}
    public double gcias{get;set;}
    public string ncm_sp1{get;set;}
    public string ncm_sp2{get;set;}
    public double precio_u{get;set;}
    public double extrag_comex1{get;set;}
    public double extrag_comex2{get;set;}
    public double extrag_comex3{get;set;}
    public string extrag_comex_notas{get;set;}
    public double extrag_local1{get;set;}
    public double extrag_local2{get;set;}
    public double extrag_finan1{get;set;}
    public double extrag_finan2{get;set;}
    public double extrag_finan3{get;set;}
    public string extrag_finan_notas{get;set;}
    public double costo_u_est{get;set;}
    public  double costo_u_prov{get;set;}
    public double costo_u{get;set;}
    public bool updated{get;set;}
    public DateTime htimestamp{get;set;}
} 