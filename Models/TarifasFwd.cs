namespace WebApiSample.Models;
// Segun el modelo de tablas "3B" Ago15
public class TarifasFwd
{
    public int id{get;set;}
    public string description{get;set;}
    public int fwdtte_id{get;set;}              //FK a la agencia de fowarding / transporte
    public int carga_id{get;set;}               //FK al tipo de carga (contenedor)
    public int paisregion_id{get;set;}          //FK al pais-region DESTINO
    public int paisfwd_id{get;set;}             //FK al pais-region ORIGEN
    public double costo{get;set;}               
    public double costo_local{get;set;}
    public double gasto_otro1{get;set;}
    public double seguro_porct{get;set;}
    public string notas{get;set;}
    public DateTime htimestamp{get;set;}
 
}

public class TarifasFwdVista
{
    public int id{get;set;}
    public string description{get;set;}
    public int fwdtte_id{get;set;}              //FK a la agencia de fowarding / transporte
    public int carga_id{get;set;}               //FK al tipo de carga (contenedor)
    public int paisregion_id{get;set;}          //FK al pais-region DESTINO
    public int paisfwd_id{get;set;}             //FK al pais-region ORIGEN
    public double costo{get;set;}               
    public double costo_local{get;set;}
    public double gasto_otro1{get;set;}
    public double seguro_porct{get;set;}
    public string notas{get;set;}
    public DateTime htimestamp{get;set;}
    public string fwdtte{get;set;}              //FK a la agencia de fowarding / transporte
    public string carga{get;set;}               //FK al tipo de carga (contenedor)
    public string pais_dest{get;set;}          //FK al pais-region DESTINO
    public string region_dest{get;set;}
    public string pais_orig{get;set;}             //FK al pais-region ORIGEN
    public string region_orig{get;set;}
 
}