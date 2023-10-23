namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;
using IronXL;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;    

// LISTED 17_10_2023
// Funcion para Importacion de Productos de XLS.
// 12_10_2023 Funcion para importacion del NCM Mex desde XLS.
// NOTA: NO USAR CSV (en la facciones aranc ... a veces vuela los puntos y lo toma como un numero) 
// INVOCACION: 
// Se llama desde /Presupuesto/Importa/ (get, sin params)) (Se usa al swagger UI como boton para comandar la importacion)
// 23_10_2023 Corrige importacion de los datos NCM, ya que el XLS usa coma como separador decimal. Ademas en el XLS son valores/100
// pero el NCM original eran los valores porcentuales, ej 16% = 16 (y no 0.16). Auditado OK contra FLC MEX PRJ017 

public class ImportService: IImportService
{
    IUnitOfWork _unitOfWork;

    //string fileName=;
 
    public ImportService(IUnitOfWork miUWork)
    {
        _unitOfWork=miUWork;
    }

    public async void ImportNCM_Mex(string fileName)
    {
        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ",";
        WorkBook workbook = WorkBook.Load(fileName);
        WorkSheet ws = workbook.DefaultWorkSheet;
        DataTable dt = ws.ToDataTable(true);//parse sheet1 of sample.xlsx file into datatable
        List<csvNcmMex> tablaNcmMex = new List<csvNcmMex>();

        foreach (DataRow row in dt.Rows) //access rows
        {
            double tmpDouble;
            csvNcmMex tmp=new csvNcmMex();

            tmp.fraccionArancelaria=row[0].ToString();

            if(double.TryParse(row[1].ToString().Split('%')[0],nfi,out tmpDouble))
                tmp.igi=tmpDouble*100.0;
            if(double.TryParse(row[2].ToString().Split('%')[0],nfi,out tmpDouble))
                tmp.iva=tmpDouble*100.0; 
            if(double.TryParse(row[3].ToString().Split('%')[0],nfi,out tmpDouble))
                tmp.dta=tmpDouble*100.0;    

            tmp.gravAcuerdo=row[4].ToString();
            tmp.bk=row[5].ToString();
            tmp.descripcion=row[6].ToString();
            tmp.docAduanera=row[7].ToString();
            tmp.lealtadCom=row[8].ToString();
            tmp.docDepo=row[9].ToString();

            NCM_Mex miNCMMex=new NCM_Mex();
            miNCMMex.dta=tmp.dta;
            miNCMMex.code=tmp.fraccionArancelaria;
            miNCMMex.igi=tmp.igi;
            miNCMMex.iva=tmp.iva;
            miNCMMex.description=tmp.descripcion;
            miNCMMex.docum_aduanera=tmp.docAduanera;
            miNCMMex.docum_depo=tmp.docDepo;
            miNCMMex.lealtad_com=tmp.lealtadCom;
            if(tmp.bk.ToUpper()=="SI")
            {
                miNCMMex.bk=true;
            }
            else
            {
                miNCMMex.bk=false;
            }
            if(tmp.gravAcuerdo.ToUpper()=="SI")
            {
                miNCMMex.gravamen_acuerdo=true;
            }
            else
            {
                miNCMMex.gravamen_acuerdo=false;
            }
            miNCMMex.htimestamp=DateTime.Now;
            miNCMMex.sp2=0;
            miNCMMex.sp1=0;
            miNCMMex.bsp1=false;  
            miNCMMex.otras_notas="";    
            await _unitOfWork.NCM_MEXs.AddAsync(miNCMMex);
            tablaNcmMex.Add(tmp); 
                        



            //for (int i = 0; i < dt.Columns.Count; i++) //access columns of corresponding row
            //{
                Console.Write(/*row[i] + "  "*/ "IGI: " + miNCMMex.igi + " DTA: " + miNCMMex.dta + " IVA: " + miNCMMex.iva);                                        
            //}

            Console.WriteLine();
        }
    }



    public async void ImportProductos(string fileName)
    {
        int limite=0;
        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        WorkBook workbook = WorkBook.Load(fileName);
        WorkSheet ws = workbook.DefaultWorkSheet;
        DataTable dt = ws.ToDataTable(true);//parse sheet1 of sample.xlsx file into datatable
        List<csvNcmMex> tablaNcmMex = new List<csvNcmMex>();

        limite=0;
        foreach (DataRow row in dt.Rows) //access rows
        {

            limite++;

            double tmpDouble;
            int tmpInt;
            csvProductos tmp=new csvProductos();

            tmp.codigo=row[0].ToString();
            tmp.name=row[1].ToString();

            if(double.TryParse(row[2].ToString(),nfi,out tmpDouble))
                tmp.alto=tmpDouble;
            if(double.TryParse(row[3].ToString(),nfi,out tmpDouble))
                tmp.largo=tmpDouble; 
            if(double.TryParse(row[4].ToString(),nfi,out tmpDouble))
                tmp.peso=tmpDouble; 
            if(double.TryParse(row[5].ToString(),nfi,out tmpDouble))
                tmp.profundidad=tmpDouble;         

            tmp.tipodeproducto=row[6].ToString();

            if(double.TryParse(row[7].ToString(),nfi,out tmpDouble))
                tmp.volumen=tmpDouble;    

            if(int.TryParse(row[8].ToString(),out tmpInt))
                tmp.unidadesporbulto=tmpInt;    

            tmp.categoriacompleta=row[9].ToString();    

            Producto miProducto=new Producto();


            miProducto.codigo=tmp.codigo;
            miProducto.name=tmp.name;
            miProducto.alto=tmp.alto;
            miProducto.largo=tmp.largo;
            miProducto.peso=tmp.peso;
            miProducto.profundidad=tmp.profundidad;
            miProducto.tipodeproducto=tmp.tipodeproducto;
            miProducto.volumen=tmp.volumen;
            miProducto.unidadesporbulto=tmp.unidadesporbulto;
            miProducto.categoriacompleta=tmp.categoriacompleta;

            await _unitOfWork.Productos.AddAsync(miProducto);
            
            
            if(miProducto.codigo=="" || miProducto.name=="")
            {
                break;
            }

            for (int i = 0; i < dt.Columns.Count; i++) //access columns of corresponding row
            {
                Console.Write(row[i] + "  ");                                        
            }

            Console.WriteLine($"REGISTRO: {limite}");
        }
    }
}


public class csvNcmMex{
    public string fraccionArancelaria {get;set;}
    public double igi{get;set;}
    public double iva{get;set;}
    public double dta{get;set;}
    public string gravAcuerdo{get;set;}
    public string bk{get;set;}
    public string descripcion{get;set;}
    public string docAduanera{get;set;}
    public string lealtadCom{get;set;}
    public string docDepo{get;set;}
}

public class csvProductos{
    public string codigo {get;set;}
    public string name {get;set;}
    public double alto{get;set;}
    public double largo{get;set;}
    public double peso{get;set;}
    public double profundidad{get;set;}
    public string tipodeproducto{get;set;}
    public double volumen{get;set;}
    public int unidadesporbulto{get;set;}
    public string categoriacompleta{get;set;}
}