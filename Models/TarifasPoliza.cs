namespace WebApiSample.Models;
public class TarifasPoliza
{
    public int id{get;set;}
    public string description{get;set;}
    public int poliza_id{get;set;}              //FK a la nomina dwe proveedores de poliza
    public int carga_id{get;set;}               //FK a la tabla de tipo de cargas (contenedores)
    public int paisregion_id{get;set;}          //FK al la nomina de paises-regiones
    public double prima{get;set;}
    public double demora{get;set;}
    public double impuestos_internos{get;set;}
    public double sellos{get;set;}
    public double factor1{get;set;}
    public double factor2{get;set;}
    public string notas{get;set;}
    public DateTime htimestamp{get;set;}
 
}