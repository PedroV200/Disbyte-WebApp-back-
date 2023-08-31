namespace WebApiSample.Models;
public class Flete
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
 
}

public class FleteVista
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
    public string pais {get;set;} 
    public string region{get;set;}
}