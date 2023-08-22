namespace WebApiSample.Models;
public class EstimateHeaderDB
{ // LISTED 26_7_2023 17:05
    // Id unico autoincremental de la BD (PK)
    public int id { get; set; }
    // Spare
    public string description { get; set; }
    // Numero de Estimate
    public int estnumber { get; set; }
    // Version del Estimate
    public int estvers { get; set; }
    // Emitido por:
    public string own { get; set;}
    // FK a la tabla de paises
    public int pais_region{get;set;}
    public double dolar{get;set;}
    // Condicion ante el iva    CELDA I5
    public bool ivaexcento {get; set;}
    // CELDA F3
             
    // Tipo de contenedor: 40HQ / 40ST / 20ST / LCL   CELDA C9
    // FK a la tabla contenedores
    public int freight_type {get; set; }
    // Foward from: CHINA / PANAMA                    VAR LIBRO BASE_TARIFAS
    // Entre el FreightType y el Freightfwd se puede determinar el costo del flete.
    public int freight_fwd {get;set;}
    public double freight_cost{get;set;}
    public double freight_insurance_cost{get;set;}
    // CELDA C10
    public double cantidadcontenedores{get;set;}
    // FK a la tabla constantes
    public int constantesid{get;set;}
    // FKs a las distintas tablas de proveedores
    /*public int p_gloc_poliza {get;set;}
    public int p_gloc_banco{get;set;}
    public int p_gloc_fwd{get;set;}
    public int p_gloc_terminal{get;set;}
    public int p_gloc_despachante{get;set;}
    public int p_gloc_flete{get;set;}
    public int p_gloc_custodia{get;set;}
    public int p_gloc_gestdigdoc{get;set;}
    // Spares para proveedores futuros
    public int p_gloc_sp1{get;set;}
    public int p_gloc_sp2{get;set;}
    public int p_gloc_sp3{get;set;}
    public int p_gloc_sp4{get;set;}
    public int gloc_poliza {get;set;}
    public int gloc_banco{get;set;}
    public int gloc_fwd{get;set;}
    public int gloc_terminal{get;set;}
    public int gloc_despachante{get;set;}
    public int gloc_flete{get;set;}
    public int gloc_custodia{get;set;}
    public int gloc_gestdigdoc{get;set;}
    // Spares para proveedores futuros
    public int gloc_sp1{get;set;}
    public int gloc_sp2{get;set;}
    public int gloc_sp3{get;set;}
    public int gloc_sp4{get;set;}*/

    public int tarifasbydateid{get;set;}
    public double totalgastosloc_uss{get;set;}
    //CELDA C3
    public double fob_grandtotal{get;set;}
    public double cbm_grandtotal{get;set;}
    public double gw_grandtotal{get;set;}
    public double cif_grandtotal{get;set;}
    public double iibbtotal{get;set;}
    public double total_impuestos{get;set;}
    public double sp1_grandtotal{get;set;}
    public double sp2_grandtotal{get;set;}
    public double sp3_grandtotal{get;set;}
    public double sp4_grandtotal{get;set;}

    // Momento de la emision
    public DateTime hTimeStamp {get; set;}


}