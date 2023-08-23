namespace WebApiSample.Models;
// Conforme la version 3B de las tablas 15Ago
public class TarifasFlete
{
    public int id{get;set;}
    public string description{get;set;}
    public int flete_id{get;set;}       //FK al proveedro de fletes
    public int carga_id{get;set;}       //FK al tipo de carga (contenedor)
    public int paisregion_id{get;set;}  //FK al listado de paises-regiones
    public int trucksemi_id{get;set;}   //FK al listado de tipo de camiones-remolques
    public double flete_interno{get;set;}
    public double devolucion_vacio{get;set;}
    public double demora{get;set;}
    public double guarderia{get;set;}
    public double costo{get;set;}
    public double descarga_depo{get;set;}
    public double gasto_otro1{get;set;}
    public double gasto_otro2{get;set;}
    public string description_depo{get;set;}
    public double peso_minimo{get;set;}
    public double peso_maximo{get;set;}
    public string notas{get;set;}
    public DateTime htimestamp{get;set;}
}


public class TarifasFleteVista
{
    public int id{get;set;}
    public string description{get;set;}
    public int flete_id{get;set;}       //FK al proveedro de fletes
    public int carga_id{get;set;}       //FK al tipo de carga (contenedor)
    public int paisregion_id{get;set;}  //FK al listado de paises-regiones
    public int trucksemi_id{get;set;}   //FK al listado de tipo de camiones-remolques
    public double flete_interno{get;set;}
    public double devolucion_vacio{get;set;}
    public double demora{get;set;}
    public double guarderia{get;set;}
    public double costo{get;set;}
    public double descarga_depo{get;set;}
    public double gasto_otro1{get;set;}
    public double gasto_otro2{get;set;}
    public string description_depo{get;set;}
    public double peso_minimo{get;set;}
    public double peso_maximo{get;set;}
    public string notas{get;set;}
    public DateTime htimestamp{get;set;}
    public string flete{get;set;}
    public string carga{get;set;}
    public string pais{get;set;}
    public string semi{get;set;}
}