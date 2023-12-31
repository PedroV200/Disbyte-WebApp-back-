namespace WebApiSample.Models;

// CONFORME FINDINGS FROM MEET 29_8_2023
// 12_10_2023 De los 5 extra gastos comex, 2 se pasan a sourcing. Se agrega campo de notas para los extrag sourcing
// Se agrega campo embarque. Antes se agregan prj y un potencial puntero a entrada de tarifonmex (actualmente en desuso)
// LISTED 23_10_2023: Se agregan campos del meeting 20_10_2023 (bl, fecha embarque, pedimiento, fecha pedimiento, y avatar_url)

public class EstimateHeaderDB
{ 
    public int id{get;set;}
    public string description{get;set;}
    public int estnumber{get;set;}
    public int estvers{get;set;}
    public int status{get;set;}
    public int paisregion_id{get;set;}
    public int carga_id{get;set;}
    public int fwdpaisregion_id{get;set;}
    public string own{get;set;}
    public double dolar{get;set;}
    public int tarifupdate{get;set;}
    public int tarifrecent{get;set;}
    public int tarifasfwd_id{get;set;}
    public int tarifasflete_id{get;set;}
    public int tarifasterminales_id{get;set;}
    public int tarifaspolizas_id{get;set;}
    public int tarifasdepositos_id{get;set;}
    public int tarifasdespachantes_id{get;set;}
    public int tarifasbancos_id{get;set;}
    public int tarifasgestdigdoc_id{get;set;}
    public double gloc_fwd{get;set;}
    public double gloc_flete{get;set;}
    public double gloc_terminales{get;set;}
    public double gloc_polizas{get;set;}
    public double gloc_depositos{get;set;}
    public double gloc_despachantes{get;set;}
    public double gloc_bancos{get;set;}
    public double gloc_gestdigdoc{get;set;}
    public double gloc_descarga{get;set;}
    public double extrag_src1{get;set;}
    public double extrag_src2{get;set;}
    public string extrag_src_notas{get;set;}
    public double extrag_comex1{get;set;}
    public double extrag_comex2{get;set;}
    public double extrag_comex3{get;set;}
    public string extrag_comex_notas{get;set;}
    public int extrag_finanformula1_id{get;set;}
    public int extrag_finanformula2_id{get;set;}
    public int extrag_finanformula3_id{get;set;}
    public int extrag_finanformula4_id{get;set;}
    public int extrag_finanformula5_id{get;set;}
    public double extrag_finan1{get;set;}
    public double extrag_finan2{get;set;}
    public double extrag_finan3{get;set;}
    public double extrag_finan4{get;set;}
    public double extrag_finan5{get;set;}
    public string extrag_finan_notas{get;set;}
    public int constantes_id{get;set;}
    public bool ivaexcento{get;set;}
    public bool usarmoneda_local{get;set;}
    public double fob_grand_total{get;set;}
    public double cbm_grand_total{get;set;}
    public double gw_grand_total{get;set;}
    public double cif_grand_total{get;set;}
    public double gastos_loc_total{get;set;}
    public double extragastos_total{get;set;}
    public double impuestos_total{get;set;}
    public double cantidad_contenedores{get;set;}
    public double freight_cost{get;set;}
    public double freight_insurance_cost{get;set;}
    public double iibb_total{get;set;}
    public string project{get;set;}
    public string embarque{get;set;}
    public string bl{get;set;}
    public DateTime fecha_embarque{get;set;}
    public string pedimiento{get;set;}
    public DateTime fecha_pedimiento{get;set;}
    public string avatar_url{get;set;}
    public int limite_carga {get;set;}
    public DateTime htimestamp{get;set;}
}


public class EstimateHeaderDBVista
{ 
    public int id{get;set;}
    public string description{get;set;}
    public int estnumber{get;set;}
    public int estvers{get;set;}
    public int status{get;set;}
    public int paisregion_id{get;set;}
    public int carga_id{get;set;}
    public int fwdpaisregion_id{get;set;}
    public string own{get;set;}
    public double dolar{get;set;}
    public int tarifupdate{get;set;}
    public int tarifrecent{get;set;}
    public int tarifasfwd_id{get;set;}
    public int tarifasflete_id{get;set;}
    public int tarifasterminales_id{get;set;}
    public int tarifaspolizas_id{get;set;}
    public int tarifasdepositos_id{get;set;}
    public int tarifasdespachantes_id{get;set;}
    public int tarifasbancos_id{get;set;}
    public int tarifasgestdigdoc_id{get;set;}
    public double gloc_fwd{get;set;}
    public double gloc_flete{get;set;}
    public double gloc_terminales{get;set;}
    public double gloc_polizas{get;set;}
    public double gloc_depositos{get;set;}
    public double gloc_despachantes{get;set;}
    public double gloc_bancos{get;set;}
    public double gloc_gestdigdoc{get;set;}
    public double gloc_descarga{get;set;}
    public double extrag_src1{get;set;}
    public double extrag_src2{get;set;}
    public string extrag_src_notas{get;set;}
    public double extrag_comex1{get;set;}
    public double extrag_comex2{get;set;}
    public double extrag_comex3{get;set;}
    public string extrag_comex_notas{get;set;}
    public int extrag_finanformula1_id{get;set;}
    public int extrag_finanformula2_id{get;set;}
    public int extrag_finanformula3_id{get;set;}
    public int extrag_finanformula4_id{get;set;}
    public int extrag_finanformula5_id{get;set;}
    public double extrag_finan1{get;set;}
    public double extrag_finan2{get;set;}
    public double extrag_finan3{get;set;}
    public double extrag_finan4{get;set;}
    public double extrag_finan5{get;set;}
    public string extrag_finan_notas{get;set;}
    public int constantes_id{get;set;}
    public bool ivaexcento{get;set;}
    public bool usarmoneda_local{get;set;}
    public double fob_grand_total{get;set;}
    public double cbm_grand_total{get;set;}
    public double gw_grand_total{get;set;}
    public double cif_grand_total{get;set;}
    public double gastos_loc_total{get;set;}
    public double extragastos_total{get;set;}
    public double impuestos_total{get;set;}
    public double cantidad_contenedores{get;set;}
    public double freight_cost{get;set;}
    public double freight_insurance_cost{get;set;}
    public double iibb_total{get;set;}
    public string project{get;set;}
    public string embarque{get;set;}
    public string bl{get;set;}
    public DateTime fecha_embarque{get;set;}
    public string pedimiento{get;set;}
    public DateTime fecha_pedimiento{get;set;}
    public string avatar_url{get;set;}
    public int limite_carga{get;set;}
    public DateTime htimestamp{get;set;}

    public string carga_str{get;set;}
    public string paisorig{get;set;}
    public string paisdest{get;set;}
}


[Flags]
public enum tarifaControl
{
    tarifBanco = 0,
    tarifDepo = 1,
    tarifDespa = 2,
    tarifFlete = 3,
    tarifasFwd = 4,
    tarifGestDigDoc = 5,
    tarifPoliza =6,
    tarifasTerm = 7,
    freight_cost = 8,
    freight_insurance_cost=9
}
