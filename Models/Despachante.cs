namespace WebApiSample.Models;

public class Despachante
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
}

public class DespachanteVista
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
    public string pais {get;set;}
}
