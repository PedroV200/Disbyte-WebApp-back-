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

public class ImportService: IImportService
{
    IUnitOfWork _unitOfWork;

    //string fileName=;
 
    public ImportService(IUnitOfWork miUWork)
    {
        _unitOfWork=miUWork;
    }

    public async void fopen(string fileName)
    {
        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
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
                tmp.igi=tmpDouble;
            if(double.TryParse(row[2].ToString().Split('%')[0],nfi,out tmpDouble))
                tmp.iva=tmpDouble; 
            if(double.TryParse(row[3].ToString().Split('%')[0],nfi,out tmpDouble))
                tmp.dta=tmpDouble;    

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
                        



            for (int i = 0; i < dt.Columns.Count; i++) //access columns of corresponding row
            {
                Console.Write(row[i] + "  ");                                        
            }

            Console.WriteLine();
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