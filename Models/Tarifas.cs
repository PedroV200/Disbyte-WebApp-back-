namespace WebApiSample.Models;

// Segun Version 3B 15Ago
public class Tarifas
{
    public TarifasBanco tBanco;
    public TarifasDespachante tDespa;
    public TarifasDeposito tDepo;
    public TarifasFlete    tFlete;
    public TarifasFwd      tFwd;
    public TarifasGestDigDoc tGestDigDoc;
    public TarifasPoliza tPoliza;
    public TarifasTerminal tTerminal;

    public Tarifas()
    {
        tBanco=new TarifasBanco();
        tDespa=new TarifasDespachante();
        tDepo=new TarifasDeposito();
        tFlete=new TarifasFlete();
        tFwd=new TarifasFwd();
        tGestDigDoc=new TarifasGestDigDoc();
        tPoliza=new TarifasPoliza();
        tTerminal=new TarifasTerminal();
    }
}

