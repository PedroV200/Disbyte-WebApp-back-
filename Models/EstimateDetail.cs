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
    public double gloc_fwd{get;set;}
    public double gloc_flete{get;set;}
    public double gloc_terminales{get;set;}
    public double gloc_polizas{get;set;}
    public double gloc_depositos{get;set;}
    public double gloc_despachantes{get;set;}
    public double gloc_bancos{get;set;}
    public double gloc_gestdigdoc{get;set;}

    public double extrag_glob_comex1{get;set;}
    public double extrag_glob_comex2{get;set;}
    public double extrag_glob_comex3{get;set;}
    public double extrag_glob_comex4{get;set;}
    public double extrag_glob_comex5{get;set;}

    public double extrag_glob_finan1{get;set;}
    public double extrag_glob_finan2{get;set;}
    public double extrag_glob_finan3{get;set;}
    public double extrag_glob_finan4{get;set;}
    public double extrag_glob_finan5{get;set;}
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
    public double totalgastos_loc_y_extra{get;set;}
    public double totalgastos_loc_y_extra_u{get;set;}
    public double totaltraderfee{get;set;}
    //public double extragasto{get;set;}
    public double overhead{get;set;}
    //public double costounituss{get;set;}
    public double costounit{get;set;}
    public double ratiopricing{get;set;}
    public double fobtocosto{get;set;}
    public DateTime htimestamp{get;set;}
}