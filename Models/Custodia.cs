namespace WebApiSample.Models;
public class Custodia
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
 
}

public class CustodiaVista
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
    public string pais{get;set;}
    public string region{get;set;}
 
}