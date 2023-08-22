using WebApiSample.Models;
using WebApiSample.Infrastructure;

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
        EstimateV2 myEstV2=new EstimateV2();
        myEstV2.estHeader=estimateDB.estHeaderDB;


        // En el caso de los Estimate detail, la DB tiene solo lo datos que se cuardaran
        // Este tipo se llama EstimateDetailDB. Mientras que el EstimateV2 definido usa el tipo EstimateDetail
        // que contiene no solo los datos que contiene "EstimateDetailDB" sino ademas provicion para los datos
        // calculados. Estos no se guardan en la base. De ahi la existencia de 2 clases "EstimateDetail"
        foreach(EstimateDetailDB edb in estimateDB.estDetailsDB)
        {
            EstimateDetail tmp=new EstimateDetail();
            tmp.id=edb.id;
            tmp.modelo=edb.modelo;
            tmp.oemprovee=edb.oemprovee;
            tmp.sku=edb.sku;
            tmp.imageurl=edb.imageurl;
            tmp.ncm=edb.ncm;
            tmp.exw=edb.exw;
            tmp.fobunit=edb.fobunit;
            tmp.qty=edb.qty;
            tmp.pcsctn=edb.pcsctn;
            tmp.cbmctn=edb.cbmctn;
            tmp.gwctn=edb.gwctn;
            tmp.str_sp1=edb.str_sp1;
            tmp.str_sp2=edb.str_sp2;
            tmp.val_sp1=edb.val_sp1;
            tmp.val_sp2=edb.val_sp2;
            tmp.val_sp3=edb.calc_sp3;
            tmp.estheaderid=edb.estheaderid;
            tmp.imp_sp1=edb.imp_sp1;
            tmp.imp_sp2=edb.imp_sp2;
            tmp.imp_sp3=edb.imp_sp3;
            tmp.ncm_arancelgrav=edb.ncm_arancelgrav;
            tmp.ncm_te_dta_otro=edb.ncm_te_dta_otro;
            tmp.ncm_iva=edb.ncm_iva;
            tmp.ncm_ivaad=edb.ncm_ivaad;
            tmp.gcias=edb.gcias;
            tmp.calc_sp1=edb.calc_sp1;
            tmp.calc_sp2=edb.calc_sp2;
            tmp.calc_sp3=edb.calc_sp3;

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
            
            tmp.id=ed.id;
            tmp.modelo=ed.modelo;
            tmp.oemprovee=ed.oemprovee;
            tmp.sku=ed.sku;
            tmp.imageurl=ed.imageurl;
            tmp.ncm=ed.ncm;
            tmp.exw=ed.exw;
            tmp.fobunit=ed.fobunit;
            tmp.qty=ed.qty;
            tmp.pcsctn=ed.pcsctn;
            tmp.cbmctn=ed.cbmctn;
            tmp.gwctn=ed.gwctn;
            tmp.str_sp1=ed.str_sp1;
            tmp.str_sp2=ed.str_sp2;
            tmp.val_sp1=ed.val_sp1;
            tmp.val_sp2=ed.val_sp2;
            tmp.val_sp3=ed.calc_sp3;
            tmp.estheaderid=ed.estheaderid;
            tmp.imp_sp1=ed.imp_sp1;
            tmp.imp_sp2=ed.imp_sp2;
            tmp.imp_sp3=ed.imp_sp3;
            tmp.ncm_arancelgrav=ed.ncm_arancelgrav;
            tmp.ncm_te_dta_otro=ed.ncm_te_dta_otro;
            tmp.ncm_iva=ed.ncm_iva;
            tmp.ncm_ivaad=ed.ncm_ivaad;
            tmp.gcias=ed.gcias;
            tmp.calc_sp1=ed.calc_sp1;
            tmp.calc_sp2=ed.calc_sp2;
            tmp.calc_sp3=ed.calc_sp3;

            estimateDB.estDetailsDB.Add(tmp);
        }
        return estimateDB;       
    }


}


 