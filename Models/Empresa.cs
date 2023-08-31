namespace WebApiSample.Models;

public class Empresa
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
}

public class EmpresaVista
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
    public string pais {get;set;}
    public string region {get;set;}
}
