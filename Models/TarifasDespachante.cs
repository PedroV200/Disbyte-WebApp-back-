namespace WebApiSample.Models;

// Segun Version 3B 15Ago
public class TarifasDespachante
{
    public int id{get;set;}
    public string description{get;set;}
    public int despachantes_id{get;set;}
    public int paisregion_id{get;set;}
    public double cargo_fijo{get;set;}
    public double cargo_variable{get;set;}
    public double clasificacion{get;set;}
    public double consultoria{get;set;}
    public double gasto_otro1{get;set;}
    public double gasto_otro2{get;set;}
    public string notas{get;set;}
    public DateTime htimestamp{get;set;}
}

public class TarifasDespachanteVista
{
    public int id{get;set;}
    public string description{get;set;}
    public int despachantes_id{get;set;}
    public int paisregion_id{get;set;}
    public double cargo_fijo{get;set;}
    public double cargo_variable{get;set;}
    public double clasificacion{get;set;}
    public double consultoria{get;set;}
    public double gasto_otro1{get;set;}
    public double gasto_otro2{get;set;}
    public string notas{get;set;}
    public DateTime htimestamp{get;set;}
    public string despachante{get;set;}
    public string pais{get;set;}
}