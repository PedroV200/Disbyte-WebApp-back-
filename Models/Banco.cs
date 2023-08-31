namespace WebApiSample.Models;
public class Banco
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
}

public class BancoVista
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
    public string pais{get;set;}
    public string region{get;set;}
}