using WebApiSample.Models;
using WebApiSample.Infrastructure;
using Microsoft.AspNetCore.Mvc;

// LISTED 25_8_2023 17:39 - cambios para Version 3C

public class dbutils
{
    public IUnitOfWork _unitOfWork;
    public dbutils(IUnitOfWork unitOfowrk)
    {
        _unitOfWork=unitOfowrk;
    }

    public async Task<EstimateDB> getEstimateLastVers(int estNumber)
    {
        EstimateDB myEst=new EstimateDB(); 
        // La consulta de headers por un numero de presupuesto puede dar como resultado mas
        // de un header (multiples versiones del mismo presupuesto). 
        List<EstimateHeaderDB>misDetalles=new List<EstimateHeaderDB>();
        // La query se hace ORDENADA POR VERSION de MAYOR A MENOR. Es una LISTA de estHeaders
        var result=await _unitOfWork.EstimateHeadersDB.GetByEstNumberLastVersAsync(estNumber);
        if(result==null)
        {
            
            return null;
        }
        misDetalles=result.ToList();
        // El elemento 0 corresponde al header buscado en con la version MAYOR.
        myEst.estHeaderDB=misDetalles[0];
        // Con el PK del header anterior me voy a buscar los Details que lo referencia en su FK
        var result1=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(myEst.estHeaderDB.id);
        // De nuevo, la consulta x detalles, da una LISTA como resultado.
        myEst.estDetailsDB=result1.ToList();
        // Devuelvo el presupuesto.
        return myEst;
    }

    public async Task<EstimateDB> getEstimateFirstVers(int estNumber)
    {
        EstimateDB myEst=new EstimateDB(); 
        // La consulta de headers por un numero de presupuesto puede dar como resultado mas
        // de un header (multiples versiones del mismo presupuesto). 
        List<EstimateHeaderDB>misDetalles=new List<EstimateHeaderDB>();
        // La query se hace ORDENADA POR VERSION de MAYOR A MENOR. Es una LISTA de estHeaders
        var result=await _unitOfWork.EstimateHeadersDB.GetByEstNumberLastVersAsync(estNumber);
        misDetalles=result.ToList();
        // El elemento n-1 corresponde al header buscado en con la version MENOR.
        myEst.estHeaderDB=misDetalles[misDetalles.Count-1];
        // Con el PK del header anterior me voy a buscar los Details que lo referencia en su FK
        var result1=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(myEst.estHeaderDB.id);
        // De nuevo, la consulta x detalles, da una LISTA como resultado.
        myEst.estDetailsDB=result1.ToList();
        // Devuelvo el presupuesto.
        return myEst;
    }

    public async Task<int> deleteEstimateByNumByVers(int estNumber, int estVers)
    {
        EstimateHeaderDB myEst=new EstimateHeaderDB();
        myEst=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(estNumber,estVers);
        // Primero liquido los estDetails del ESTIMATE. No puedo borrar primero el header por que las FK quedan sueltas.
        // Primero borro los estDetail con FK coinciente.
        var result= await _unitOfWork.EstimateDetailsDB.DeleteByIdEstHeaderAsync(myEst.id);
        // Ahora si borro el header
        var result1=await _unitOfWork.EstimateHeadersDB.DeleteAsync(myEst.id);
        // devuelvo solo la cantidad de estimateDetail borrados, por que header ya se que borre uno solo.
        return result;
    }

    public async Task<EstimateDB> GetEstimateByNumByVers(int estNumber, int estVers)
    {
        EstimateDB myEst=new EstimateDB();
        myEst.estHeaderDB=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(estNumber,estVers);
        if(myEst.estHeaderDB==null)
        {
            return null;
        }
        var result=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(myEst.estHeaderDB.id);
        myEst.estDetailsDB=result.ToList();
        return myEst;
    }

    public EstimateV2 transferDataFromDBType(EstimateDB estimateDB)
    {
        int updated;
        EstimateV2 myEstV2=new EstimateV2();
        myEstV2.estHeader=estimateDB.estHeaderDB;

        // 30/8/2023 12:02PM
        // En el caso de los Estimate detail, la DB tiene solo lo datos que se cuardaran
        // Este tipo se llama EstimateDetailDB. Mientras que el EstimateV2 definido usa el tipo EstimateDetail
        // que contiene no solo los datos que contiene "EstimateDetailDB" sino ademas provicion para los datos
        // calculados. Estos no se guardan en la base. De ahi la existencia de 2 clases "EstimateDetail"
        updated=0;
        foreach(EstimateDetailDB edb in estimateDB.estDetailsDB)
        {
            EstimateDetail tmp=new EstimateDetail();

            tmp.id = edb.id;
            tmp.estimateheader_id = edb.estimateheader_id;
            tmp.proveedores_id	= edb.proveedores_id;
            tmp.ncm_id = edb.ncm_id;
            tmp.ncm_ack	= edb.ncm_ack;
            tmp.sku = edb.sku;
            tmp.description = edb.description;
            tmp.imageurl	= edb.imageurl;
            tmp.exw_u = edb.exw_u;
            tmp.fob_u = edb.fob_u;
            tmp.qty = edb.qty;
            tmp.pcsctn = edb.pcsctn;
            tmp.cbmctn = edb.cbmctn;
            tmp.gwctn = edb.gwctn;
            tmp.cambios_notas =	edb.cambios_notas;
            tmp.ncm_arancel = edb.ncm_arancel;
            tmp.ncm_te_dta_otro = edb.ncm_te_dta_otro;
            tmp.ncm_iva = edb.ncm_iva;
            tmp.ncm_ivaad = edb.ncm_ivaad;
            tmp.gcias = edb.gcias;
            tmp.ncm_sp1	= edb.ncm_sp1;
            tmp.ncm_sp2	= edb.ncm_sp2;
            tmp.precio_u = edb.precio_u;
            tmp.extrag_comex1 =	edb.extrag_comex1;
            tmp.extrag_comex2 =	 edb.extrag_comex2;
            tmp.extrag_comex3 =	 edb.extrag_comex3;
            tmp.extrag_comex_notas = edb.extrag_comex_notas;
            tmp.extrag_local1 = edb.extrag_local1;
            tmp.extrag_local2 = edb.extrag_local2;
            tmp.extrag_finan1 = edb.extrag_finan1;
            tmp.extrag_finan2 = edb.extrag_finan2;
            tmp.extrag_finan3 = edb.extrag_finan3;
            tmp.extrag_finan_notas = edb.extrag_finan_notas;
            tmp.costo_u_est = edb.costo_u_est;
            tmp.costo_u_prov = edb.costo_u_prov;
            tmp.costo_u = edb.costo_u;
            tmp.htimestamp = edb.htimestamp;
            if(edb.updated)
            {
                updated++;
            }    
            myEstV2.estDetails.Add(tmp);
        }
        return myEstV2;
    }
// La inversa de la anterior. Pasa un EstimateV2 a un EstimateDB.
    public EstimateDB transferDataToDBType(EstimateV2 myEstV2)
    {
         EstimateDB estimateDB=new EstimateDB();


        estimateDB.estHeaderDB=myEstV2.estHeader;

// Aqui se descartan los calculos. Solo se transfieren los valores necesarios para los mismos
        foreach(EstimateDetail ed in myEstV2.estDetails)
        {
            EstimateDetailDB tmp=new EstimateDetailDB();

            tmp.id = ed.id;
            tmp.estimateheader_id = ed.estimateheader_id;
            tmp.proveedores_id	= ed.proveedores_id;
            tmp.ncm_id = ed.ncm_id;
            tmp.ncm_ack	= ed.ncm_ack;
            tmp.sku = ed.sku;
            tmp.description = ed.description;
            tmp.imageurl	= ed.imageurl;
            tmp.exw_u = ed.exw_u;
            tmp.fob_u = ed.fob_u;
            tmp.qty = ed.qty;
            tmp.pcsctn = ed.pcsctn;
            tmp.cbmctn = ed.cbmctn;
            tmp.gwctn = ed.gwctn;
            tmp.cambios_notas =	ed.cambios_notas;
            tmp.ncm_arancel = ed.ncm_arancel;
            tmp.ncm_te_dta_otro = ed.ncm_te_dta_otro;
            tmp.ncm_iva = ed.ncm_iva;
            tmp.ncm_ivaad = ed.ncm_ivaad;
            tmp.gcias = ed.gcias;
            tmp.ncm_sp1	= ed.ncm_sp1;
            tmp.ncm_sp2	= ed.ncm_sp2;
            tmp.precio_u = ed.precio_u;
            tmp.extrag_comex1 =	ed.extrag_comex1;
            tmp.extrag_comex2 =	 ed.extrag_comex2;
            tmp.extrag_comex3 =	 ed.extrag_comex3;
            tmp.extrag_comex_notas = ed.extrag_comex_notas;
            tmp.extrag_local1 = ed.extrag_local1;
            tmp.extrag_local2 = ed.extrag_local2;
            tmp.extrag_finan1 = ed.extrag_finan1;
            tmp.extrag_finan2 = ed.extrag_finan2;
            tmp.extrag_finan3 = ed.extrag_finan3;
            tmp.extrag_finan_notas = ed.extrag_finan_notas;
            tmp.costo_u_est = ed.costo_u_est;
            tmp.costo_u_prov = ed.costo_u_prov;
            tmp.costo_u = ed.costo_u;
            tmp.htimestamp = ed.htimestamp;
            tmp.htimestamp=DateTime.Now;
            //tmp.htimestamp=ed.htimestamp;
            estimateDB.estDetailsDB.Add(tmp);
        }
        return estimateDB;       
    }

    public EstimateV2 ClearExtraGastosComex(EstimateV2 miEst)
    {
        miEst.estHeader.extrag_comex1=0;
        miEst.estHeader.extrag_comex2=0;
        miEst.estHeader.extrag_comex3=0;
        miEst.estHeader.extrag_comex4=0;
        miEst.estHeader.extrag_comex5=0;
        miEst.estHeader.extrag_comex_notas="";

        return miEst;
    }

    public EstimateV2 ClearExtraGastosFinanzas(EstimateV2 miEst)
    {
        miEst.estHeader.extrag_finan1=0;
        miEst.estHeader.extrag_finan2=0;
        miEst.estHeader.extrag_finan3=0;
        miEst.estHeader.extrag_finan4=0;
        miEst.estHeader.extrag_finan5=0;
        miEst.estHeader.extrag_finan_notas="";
        miEst.estHeader.extrag_finanformula1_id=0;
        miEst.estHeader.extrag_finanformula2_id=0;
        miEst.estHeader.extrag_finanformula3_id=0;
        miEst.estHeader.extrag_finanformula4_id=0;
        miEst.estHeader.extrag_finanformula5_id=0;

        return miEst;
    }

    public EstimateV2 setDefaultEstimateDB(EstimateV2 miEst)
    {
       
        miEst=ClearExtraGastosComex(miEst);
        miEst=ClearExtraGastosFinanzas(miEst);
        // IMPORTANTE !!!!! 
        // Valores por defecto: Estodo 0.
        // Todos las tarifas son actualizables (bits 0 a 7)
        // fregith_cost y freight_insurance se calculan desde las tarfias (bits 8 y 9)
        // Las tarfias se actualizaran x fecha y no por ID.
        miEst.estHeader.status=0x00;
        miEst.estHeader.tarifrecent=1023;
        miEst.estHeader.tarifupdate=1023;
        // FIN IMPORTANTE
        foreach(EstimateDetail edb in miEst.estDetails)
        {
            edb.extrag_comex1=0;
            edb.extrag_comex2=0;
            edb.extrag_comex3=0;
            edb.extrag_comex_notas="";
            edb.extrag_finan1=0;
            edb.extrag_finan2=0;
            edb.extrag_finan3=0;
            edb.extrag_finan_notas="";
            edb.extrag_local1=0;
            edb.extrag_local2=0;
            edb.updated=false;
        }
        return miEst;
    } 
}


 