namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;


// LISTED 9_8_2023 11:36 AM 
public class PresupuestoService:IPresupuestoService
{
    
    private calc myCalc;

    IUnitOfWork _unitOfWork;

    IEstimateService _estService;

    public ICnstService _cnstService {get;}

    public string presupError;

    public PresupuestoService(IUnitOfWork unitOfWork, IEstimateService estService,ICnstService constService)
    {
        _unitOfWork=unitOfWork;
        _estService=estService;
        _cnstService=constService;
         myCalc = new calc(_unitOfWork,_estService);
    }

    public string getLastErr()
    {
        return myCalc.haltError+presupError;
    }


    public async Task<EstimateV2>acalcPresupuesto(EstimateDB miEst)
    {
            //return await myCalc.aCalc(miEst,miEst.estHeaderDB.estnumber,miEst.estHeaderDB.freight_insurance_cost,miEst.estHeaderDB.p_gloc_banco,miEst.estHeaderDB.total_impuestos,miEst.estHeaderDB.own);
            return null;
    }

    public async Task<EstimateV2>submitPresupuestoNew(EstimateDB miEst)
    {
        var result=0;
        EstimateV2 myEstV2=new EstimateV2();
        dbutils myDBhelper=new dbutils(_unitOfWork);
        EstimateDB resultEDB=new EstimateDB();
        EstimateHeaderDB readBackHeader=new EstimateHeaderDB();

        // ###### INIT
        // Es un presupuesto nuevo ... me aseguro que todo lo que es extragasto este en cero y los flags de update en false
        resultEDB=myDBhelper.setDefaultEstimateDB(resultEDB);
        // ###### FIN INIT

        // Numeracion / Versionado:
        // Este es un nuevo presupuesto, con lo cual me limito a preguntar el estNumber mas alto usado
        // y luego le sumo 1. La version es 1.
        miEst.estHeaderDB.estnumber=await _unitOfWork.EstimateHeadersDB.GetNextEstNumber();
        miEst.estHeaderDB.estvers=1;

        // Grabo la estampa de tiempo en el header
        miEst.estHeaderDB.htimestamp=DateTime.Now;



        //#-----ARREGLAR------##### => miEst.estHeaderDB.tarifasbydateid=myTar.id;

        // Cuando me pasan el presupuesto con dolar billete "-1" es por que debo extraerlo
        // desde la base TC-CDA. 
        if(miEst.estHeaderDB.dolar<0)
        {
            // Leo la fecha y la paso al formato que le gusta a postgre
            string hoy=DateTime.Now.ToString("yyyy-MM-dd");
            // Consulto el TC del dia (si existe, ojo)
            double tipoDeCambio= await _unitOfWork.TiposDeCambio.GetByDateAsync(hoy); 
            if(tipoDeCambio>0)
            {   // La consulta tuvo exito ?
                miEst.estHeaderDB.dolar=tipoDeCambio;
            }
            else
            {   // FALLO, no existe el TC de la fecha mencionada*/ !!!!!!!
                _estService.setLastError("Se solicito usar TC de la base TC_CDA, pero el dia de la fecha no tiene valor asignado");
                return null;
            }
        }

        // ################################### INIT ######################################
        
        
        // Expando el EstimateDB a un EstimateV2
        myEstV2=myDBhelper.transferDataFromDBType(miEst);
        // Cargo las constnates de calculo.
        myEstV2.constantes=await _cnstService.getConstantesLastVers();
        if(myEstV2.constantes==null)
        {
            _estService.setLastError("No existe una instancia de constantes en tabla");
            return null;
        }

        // Ingresa las constantes a la varieble local de _estService.
         _estService.setConstants(myEstV2.constantes);
         // est service carga los datos del contenedor referenciado en una variable de la clase para su posterior uso
         _estService.loadContenedor(myEstV2);
        // Busca las tarifas, calcula cada uno de los gastos locales y los pasa al header segun sea
        // necesario. Luego popula todas las columnas de gastos locales ponderando por el FP.
        // El FP solo estara disponible luego de calculado el FOB TOTAL.
         myEstV2=await _estService.loadTarifas(myEstV2);
         if(myEstV2==null)
         {
            presupError=_estService.getLastError();
            return null;
         }
        // ################################## FIN INIT ############################################

        // CALCULOS PROPIAMENTE DICHOS
        myEstV2=await myCalc.calcBatch(myEstV2);
        // Si los calculos fallan, no hacer nada.
        if(myEstV2==null)
        {
            return null;
        }

        // Le pongo la fecha / hora !!!!!!
        //ret.estHeader.hTimeStamp=DateTime.Now;

        // lo que me deuvelve la rutina de calculo es un EstimateV2, cuyo Detail es mucho mas extenso
        // En la base no se guardan calculos,  por lo que debi convertir el estimate V2 a estimate DB y guardarlo.

        resultEDB=myDBhelper.transferDataToDBType(myEstV2);

        // Guardo el header.
        result=await _unitOfWork.EstimateHeadersDB.AddAsync(resultEDB.estHeaderDB);
        // Veo que ID le asigno la base:
        readBackHeader=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(resultEDB.estHeaderDB.estnumber,miEst.estHeaderDB.estvers);
        // Ahora si, inserto los detail uno a uno ne la base
        foreach(EstimateDetailDB ed in resultEDB.estDetailsDB)
        {
            ed.estimateheader_id=readBackHeader.id; // El ID que la base le asigno al header que acabo de insertar.
            ed.updated=false;                       // El calculo se hizo por completo. No hay actualizaciones epndientes. Borro el flag de cada producto.
            result+=await _unitOfWork.EstimateDetailsDB.AddAsync(ed);
        }
        
        return myEstV2;      
    }

    public async Task<EstimateV2>simulaPresupuesto(EstimateDB miEst)
    {
        dbutils dbhelper=new dbutils(_unitOfWork);
        EstimateV2 ret=new EstimateV2();
        // Grabo la estampa de tiempo en el header
        miEst.estHeaderDB.htimestamp=DateTime.Now;
        // Preparo un string solo con la fecha para consultar a la DB
        string fecha=miEst.estHeaderDB.htimestamp.ToString("yyyy-MM-dd");


        // Cuando me pasan el presupuesto con dolar billete "-1" es por que debo extraerlo
        // desde la base TC-CDA. 
        if(miEst.estHeaderDB.dolar<0)
        {
            // Leo la fecha y la paso al formato que le gusta a postgre
            string hoy=DateTime.Now.ToString("yyyy-MM-dd");
            // Consulto el TC del dia (si existe, ojo)
            double tipoDeCambio= await _unitOfWork.TiposDeCambio.GetByDateAsync(hoy); 
            if(tipoDeCambio>0)
            {   // La consulta tuvo exito ?
                miEst.estHeaderDB.dolar=tipoDeCambio;
            }
            else
            {   // FALLO, no existe el TC de la fecha mencionada*/ !!!!!!!
                _estService.setLastError("Se solicito usar TC de la base TC_CDA, pero el dia de la fecha no tiene valor asignado");
                return null;
            }
        }


        // El objeto Estimate que se definio. 
        EstimateV2 myEstV2=new EstimateV2();
        // Expando el EstimateDB a un EstimateV2
        myEstV2=dbhelper.transferDataFromDBType(miEst);
        // Cargo las constnates de calculo.
        myEstV2.constantes=await _cnstService.getConstantesLastVers();
        if(myEstV2.constantes==null)
        {
            _estService.setLastError("No existe una instancia de constantes en tabla");
            return null;
        }
        ret=await myCalc.calcBatch(myEstV2);
        return ret;
    }

    public async Task<EstimateV2>submitPresupuestoUpdated(int estNumber,EstimateDB miEst)
    {
        var result=0;
        dbutils dbhelper=new dbutils(_unitOfWork);
        EstimateV2 ret=new EstimateV2();

       /* // Cuando me pasan un presupuesto con VERSION 0, significa que es una simulacion
        // y no se ingresara a la base.
        if(estNumber==0)
        {
            ret=await myCalc.calcBatch(miEst);
            return ret;
        } */



        // La version no es 0. No es una simulacion. Va en serio. 
        EstimateHeaderDB readBackHeader=new EstimateHeaderDB();
        
        /*readBackHeader=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(miEst.estHeaderDB.EstNumber,miEst.estHeaderDB.EstVers);
        if(readBackHeader !=null)
        {   // OJO
             return null;
        }*/

        miEst.estHeaderDB.estnumber=estNumber;
        miEst.estHeaderDB.estvers=await _unitOfWork.EstimateHeadersDB.GetNextEstVersByEstNumber(estNumber);

        if(miEst.estHeaderDB.estvers==0)
        {
            return null;
        }

// Grabo la estampa de tiempo en el header
        miEst.estHeaderDB.htimestamp=DateTime.Now;
        // Preparo un string solo con la fecha para consultar a la DB
        string fecha=miEst.estHeaderDB.htimestamp.ToString("yyyy-MM-dd");

        // Cuando me pasan el presupuesto con dolar billete "-1" es por que debo extraerlo
        // desde la base TC-CDA. 
        if(miEst.estHeaderDB.dolar<0)
        {
            // Leo la fecha y la paso al formato que le gusta a postgre
            string hoy=DateTime.Now.ToString("yyyy-MM-dd");
            // Consulto el TC del dia (si existe, ojo)
            double tipoDeCambio= await _unitOfWork.TiposDeCambio.GetByDateAsync(hoy); 
            if(tipoDeCambio>0)
            {   // La consulta tuvo exito ?
                miEst.estHeaderDB.dolar=tipoDeCambio;
            }
            else
            {   // FALLO, no existe el TC de la fecha mencionada*/ !!!!!!!
                _estService.setLastError("Se solicito usar TC de la base TC_CDA, pero el dia de la fecha no tiene valor asignado");
                return null;
            }
        }

        // ########################### INIT ##################################################
        // El objeto Estimate que se definio. 
        EstimateV2 myEstV2=new EstimateV2();
        // Expando el EstimateDB a un EstimateV2
        myEstV2=dbhelper.transferDataFromDBType(miEst);
        // Cargo las constnates de calculo.
        myEstV2.constantes=await _cnstService.getConstantesLastVers();
        if(myEstV2.constantes==null)
        {
            _estService.setLastError("No existe una instancia de constantes en tabla");
            return null;
        }
        
        // Ingresa las constantes a la varieble local de _estService.
         _estService.setConstants(myEstV2.constantes);
         // est service carga los datos del contenedor referenciado en una variable de la clase para su posterior uso
         _estService.loadContenedor(myEstV2);
        // Busca las tarifas, calcula cada uno de los gastos locales y los pasa al header segun sea
        // necesario. Luego popula todas las columnas de gastos locales ponderando por el FP.
        // El FP solo estara disponible luego de calculado el FOB TOTAL.
         myEstV2=await _estService.loadTarifas(myEstV2);
         if(myEstV2==null)
         {
            presupError=_estService.getLastError();
            return null;
         }
        // ############################# FIN INIT ########################################


        // Los calculos propiemanete dichos.
        ret=await myCalc.calcBatch(myEstV2);
        // Si los calculos fallan, no hacer nada.
        if(ret==null)
        {
            return null;
        }

        // Le pongo la fecha / hora !!!!!!
        ret.estHeader.htimestamp=DateTime.Now;

        // lo que me deuvelve la rutina de calculo es un EstimateV2, cuyo Detail es mucho mas extenso
        // En la base no se guardan calculos,  por lo que debi convertir el estimate V2 a estimate DB y guardarlo.
        dbutils myDBhelper=new dbutils(_unitOfWork);
        EstimateDB resultEDB=new EstimateDB();
        resultEDB=myDBhelper.transferDataToDBType(ret);

        // Guardo el header.
        result=await _unitOfWork.EstimateHeadersDB.AddAsync(resultEDB.estHeaderDB);
        // Veo que ID le asigno la base:
        readBackHeader=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(resultEDB.estHeaderDB.estnumber,miEst.estHeaderDB.estvers);
        // Ahora si, inserto los detail uno a uno ne la base
        foreach(EstimateDetailDB ed in resultEDB.estDetailsDB)
        {
            ed.estimateheader_id=readBackHeader.id; // El ID que la base le asigno al header que acabo de insertar.
            result+=await _unitOfWork.EstimateDetailsDB.AddAsync(ed);
        }
        
        return ret;      
    }


    public async Task<EstimateV2>reclaimPresupuesto(int estNumber,int estVers)
    {
        dbutils dbhelper=new dbutils(_unitOfWork);
        EstimateV2 myEstV2=new EstimateV2();
        EstimateDB miEst=new EstimateDB();

        // Levanto el header segun numero y version
        miEst.estHeaderDB=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(estNumber,estVers);
        if(miEst.estHeaderDB ==null)
        {   // OJO
             return null;
        }
        // Con el ID del header levanto el estDetail.
        var result=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(miEst.estHeaderDB.id);
        if(result==null)
        {
            return null;
        }
        miEst.estDetailsDB=result.ToList();

        // Expando el EstimateDB a un EstimateV2
        myEstV2=dbhelper.transferDataFromDBType(miEst);
        // Cargo las constnates de calculo.
        myEstV2.constantes=await _cnstService.getConstantesLastVers();
        if(myEstV2.constantes==null)
        {
            _estService.setLastError("No existe una instancia de constantes en tabla");
            return null;
        }
        // Paso las costantes a estDetailServ via estimateService
        _estService.setConstants(myEstV2.constantes);

        // Por el momento reclaim tarifas no hace mucho. Los GLOC ya fueron calculados.
        myEstV2=_estService.reclaimTarifas(myEstV2);
        /*if(myEstV2==null)
        {
            presupError=_estService.getLastError();
            return null;
        }*/
        // Carga los datos de la carga en EstimateV2.miCarga
        _estService.loadContenedor(myEstV2);

        myEstV2=await myCalc.calcReclaim(myEstV2);
        
        return myEstV2;      
    }

}