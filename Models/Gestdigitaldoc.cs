namespace WebApiSample.Models;
public class GestDigitalDoc
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}

 
}

public class GestDigitalDocVista
{
    public int id { get; set; }
    public string description { get; set; }
    public int paisregion_id {get;set;}
    public string pais {get;set;}
    public string region {get;set;}
    
 
}