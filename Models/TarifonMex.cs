namespace WebApiSample.Models;

// Acordado con GF 3_10_2023
public class TarifonMex
{
    public int id {get;set;}
    public string description {get;set;}
    public double flete_internacional_40sthq{get;set;}
    public double flete_internacional_20ft{get;set;}
    public double seguro{get;set;}
    public double gastosLocales_40sthq{get;set;}
    public double gastosLocales_20ft{get;set;}   
    public double terminal_40sthq{get;set;}
    public double terminal_20ft{get;set;}
    public double flete_interno_1p40sthq_guad{get;set;}     //1*40hq
    public double flete_interno_1p20ft_guad{get;set;}       //1*20ft
    public double flete_interno_1p40sthq_cdmx{get;set;}     
    public double flete_interno_1p20ft_cdmx{get;set;}
    public double flete_interno_2p40sthq_guad{get;set;}     //2*40hq o st GUAD
    public double flete_interno_2p20ft_guad{get;set;}       //2*20ft GUAD
    public double flete_interno_2p40sthq_cdmx{get;set;}     //2*40hq CDMX
    public double flete_interno_2p20ft_cdmx{get;set;}       //2*20ft CDMX
    public double descarga_meli_40sthq_guad{get;set;}
    public double descarga_meli_20ft_guad{get;set;}
    public double descarga_meli_40sthq_cdmx{get;set;}
    public double descarga_meli_20ft_cdmx{get;set;}
    public double despa_fijo{get;set;}
    public double despa_var{get;set;}
    public double despa_clasific_oper{get;set;}
    public double despa_consult_compl{get;set;}
    public DateTime htimestamp{get;set;}
}