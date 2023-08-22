namespace WebApiSample.Models;

// Segun Version 3B 15Ago
public class TarifasGestDigDoc
{
    public int id{get;set;}
    public string description{get;set;}
    public int gestdigdoc_id{get;set;}              //FK a la nomina de Empresas digitalizacion documental
    public int paisregion_id{get;set;}              //FK a la tabla de pais-region
    public double costo{get;set;}
    public double factor1{get;set;}
    public double gasto_otro1{get;set;}
    public string notas{get;set;}
    public DateTime htimestamp{get;set;}
}