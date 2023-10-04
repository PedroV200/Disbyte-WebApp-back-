   namespace WebApiSample.Models;
   

// Clase que agrupa todas las constantes desperdigadas a lo largo de los calculos.

   public class CONSTANTES
   {
    public int id;
    public double CNST_GASTOS_DESPA_Cif_Min{get;set;}
    public double CNST_GASTOS_DESPA_Cif_Mult{get;set;}
    public double CNST_GASTOS_DESPA_Cif_Thrhld{get;set;}
    public double CNST_GASTOS_CUSTODIA_Thrshld{get;set;}
    public double CNST_GASTOS_GESTDIGDOC_Mult{get;set;}
    public double CNST_GASTOS_BANCARIOS_Mult{get;set;}
    public double CONST_NCM_DIE_Min{get;set;}  
    public double CNST_ESTAD061_ThrhldMAX{get;set;}
    public double CNST_ESTAD061_ThrhldMIN{get;set;}
    public double CNST_GCIAS_424_Mult{get;set;}
    public double CNST_SEGURO_PORCT{get;set;}
    public double CNST_ARANCEL_SIM{get;set;}
    public double CNST_FREIGHT_PORCT_ARG{get;set;}
    public int paisreg_china_shezhen{get;set;}
    public int paisreg_mex_guad{get;set;}
    public int carga20{get;set;}
    public int carga220{get;set;}
    public int carga40{get;set;}
    public int carga240{get;set;}
    public int fwdtte_id{get;set;}
    public int flete_id{get;set;}
    public int terminal_id{get;set;}
    public int despachantes_id{get;set;}
    public int trucksemi_id{get;set;}
    public double CNST_SP12{get;set;}

    public DateTime hTimeStamp {get; set;}

    }