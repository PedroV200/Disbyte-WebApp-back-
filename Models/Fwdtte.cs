namespace WebApiSample.Models;
public class Fwdtte
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
 
}

public class FwdtteVista
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
    public string pais {get;set;}
 
}