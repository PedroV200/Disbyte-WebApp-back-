namespace WebApiSample.Models;
public class Deposito
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
}

public class DepositoVista
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
    public string pais {get;set;}
}