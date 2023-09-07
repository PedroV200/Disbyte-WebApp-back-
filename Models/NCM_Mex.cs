namespace WebApiSample.Models;
public class NCM_Mex
{
    public int id{get;set;}
    public string description{get;set;}
    public string code{get;set;}
    public double igi{get;set;}
    public double iva{get;set;}
    public double dta{get;set;}
    public double sp1{get;set;}
    public double sp2{get;set;}
    public bool gravamen_acuerdo{get;set;}
    public bool bk{get;set;}
    public bool bsp1{get;set;}
    public string docum_aduanera{get;set;}
    public string lealtad_com{get;set;}
    public string docum_depo{get;set;}
    public string otras_notas{get;set;}
    public DateTime htimestamp{get;set;}
}