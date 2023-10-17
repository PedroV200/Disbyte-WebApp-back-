namespace WebApiSample.Models;

public class Producto
{
    public int id {get;set;}
	public string codigo {get;set;}
	public string name {get;set;}
	public double alto {get;set;}
	public double largo {get;set;}
	public double peso {get;set;}
	public double profundidad{get;set;} 
	public string tipodeproducto{get;set;}
	public double volumen {get;set;}
	public int unidadesporbulto{get;set;}
	public string categoriacompleta{get;set;} 
}