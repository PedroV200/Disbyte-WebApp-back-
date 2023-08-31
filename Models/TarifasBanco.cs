namespace WebApiSample.Models;

// Segun Version 3B 15Ago 
public class TarifasBanco
{
    public int id{get;set;}
    public string description{get;set;}
    public int banco_id{get;set;}
    public int paisregion_id{get;set;}
    public double costo{get;set;}
    public double factor1{get;set;}
    public double gasto_otro1{get;set;}
    public string notas{get;set;}
    public DateTime htimestamp{get;set;}
}

public class TarifasBancoVista
{
    public int id{get;set;}
    public string description{get;set;}
    public int banco_id{get;set;}
    public int paisregion_id{get;set;}
    public double costo{get;set;}
    public double factor1{get;set;}
    public double gasto_otro1{get;set;}
    public string notas{get;set;}
    public DateTime htimestamp{get;set;}

    public string banco{get;set;}
    public string pais{get;set;}
    public string region{get;set;}
}