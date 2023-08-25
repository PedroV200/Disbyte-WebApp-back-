namespace WebApiSample.Models;

// VERSION II Ago 2023
// MODELO DE DATOS QUE SE ENVIA AL FRONT.
// Este modelo de datos representa la planilla de precosteo.
public class EstimateDetail
{
    // VALORES DADOS o ajustables
    public int id{get;set;}
    public int estimateheader_id{get;set;}
    public int proveedores_id{get;set;}
    public string sku{get;set;}
    public string description{get;set;}
    public string imageurl{get;set;}
    public double exw{get;set;}
    public double fobunit{get;set;}
    public double fobunit_adj{get;set;}
    public string fobunit_adj_description{get;set;}
    public double qty{get;set;}
    public double pcsctn{get;set;}
    public double cbmctn{get;set;}
    public double gwctn{get;set;}
    public double freight_charge_adj{get;set;}
    public double freight_insurance_adj{get;set;}
    public double cif{get;set;}
    public double cif_adj{get;set;}
    public string cif_adj_description{get;set;}
    public int ncm_id{get;set;}
    public double ncm_arancel{get;set;}
    public double ncm_te_dta_otro{get;set;}
    public double ncm_iva{get;set;}
    public double ncm_ivaad{get;set;}
    public double gcias{get;set;}
    public string ncm_sp1{get;set;}
    public string ncm_sp2{get;set;}
    public double gloc_fwd_adj{get;set;}
    public double gloc_flete_adj{get;set;}
    public double gloc_terminal_adj{get;set;}
    public double gloc_poliza_adj{get;set;}
    public double gloc_deposito_adj{get;set;}
    public double gloc_despachante_adj{get;set;}
    public double gloc_banco_adj{get;set;}
    public double gloc_gestdigdoc_adj{get;set;}
    public double gasto_otro1_adj{get;set;}
    public double gasto_otro2_adj{get;set;}
    public double gasto_otro3_adj{get;set;}
    public string ajuste_expre1{get;set;}
    public string ajuste_expre2{get;set;}
    public string ajuste_expre3{get;set;}
    public string gloc_adj_description{get;set;}
    public double precio_u{get;set;}
    public double costo_u{get;set;}
    public double costo_u_provisorio{get;set;}
    public double costo_u_provisorio_adj{get;set;}
    public double costo_u_financiero{get;set;}
    public double costo_u_financiero_adj{get;set;}
    public double extra_gasto1{get;set;}
    public double extra_gasto2{get;set;}
    public double extra_gasto3{get;set;}
    public double extra_gasto4{get;set;}
    public double extra_gasto5{get;set;}
    public double extra_gasto6{get;set;}
    public double extra_gasto7{get;set;}
    public double extra_gasto8{get;set;}
    public double extra_gasto9{get;set;}
    public double extra_gasto10{get;set;}
    public string extra_gasto_expre{get;set;}
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
    //public double ncm_arancelgrav { get; set; }         //DIE o Aranceles
    public double arancelgrav_cif{get;set;}             // Derechos
    //public double ncm_te_dta_otro { get; set; }         //TE o DTA
    public double te_dta_otro_cif{get;set;}             // Te061
    public double baseiva{get;set;}                     
    public double baseiva_unit{get;set;}
    //public double ncm_iva { get; set; }
    public double iva_cif{get;set;}
    //public double ncm_ivaad { get; set; }
    public double ivaad_cif{get;set;}
    //public double gcias{get;set;}
    public double gcias424{get;set;}
    public double iibb900{get;set;}

    public double totalaranceles{get;set;}
    //public double gloc_fwd{get;set;}
    //public double gloc_terminal{get;set;}
    //public double gloc_flete{get;set;}
    //public double gloc_cust{get;set;}
    //public double gloc_despachante{get;set;}
    //public double gloc_banco{get;set;}
    //SPARES
    //public double gloc_sp1{get;set;}
    //public double gloc_sp2{get;set;}
    //public double gloc_sp3{get;set;}
    //FIN SPARES
    //public double preciounit_uss{get;set;}
    public double totalgastosloc_uss{get;set;}
    public double totaltraderfee{get;set;}
    //public double extragasto{get;set;}
    public double overhead{get;set;}
    //public double costounituss{get;set;}
    public double costounit{get;set;}
    public double ratiopricing{get;set;}
    public double fobtocosto{get;set;}
}