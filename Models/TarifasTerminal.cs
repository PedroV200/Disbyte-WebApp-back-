namespace WebApiSample.Models;

// En conformidad con el modelo de tablas "3B" Ago15
public class TarifasTerminal
{
    public int id{get;set;}
    public string description{get;set;}
    public int terminal_id{get;set;}                //FK a la nomina de Terminales
    public int carga_id{get;set;}                   //FK a la tabla de tipos de Catga (contenedores)
    public int paisregion_id{get;set;}              //FK a la tabla de paises-regiones
    public double gasto_fijo{get;set;}
    public double gasto_variable{get;set;}
    public double gasto_otro1{get;set;}
    public double gasto_otro2{get;set;}
    public string notas{get;set;}
    public DateTime htimestamp{get;set;}
 
}