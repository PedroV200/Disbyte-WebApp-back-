namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;


// LISTED 9_8_2023 11:36 AM  
// LISTED 11_09_2023 17:15 Primera logica de estados aplicada a presupuestoUpdate.
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
        return myCalc.haltError+presupError+_estService.getLastError();
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

        // Traduzco el id del pais / region en un string de 3 caracteres normalizado.
        // Sera clave para bifurcar la logica del calculo segun los diferentes paises
         myEstV2=await getCountry(myEstV2);



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
        // Fijo el estado en 0. Es un presupuesto nuevo, no me importa lo que me manden en el JSON.
        // el presupuesto nuevo arranca con estado 0. 
        resultEDB.estHeaderDB.status=0;

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
        EstimateV2 myEstV2=new EstimateV2();
        dbutils myDBhelper=new dbutils(_unitOfWork);
        EstimateDB resultEDB=new EstimateDB();
        EstimateHeaderDB estHDBPrevia=new EstimateHeaderDB();
        EstimateHeaderDB readBackHeader=new EstimateHeaderDB();

        estHDBPrevia=await _unitOfWork.EstimateHeadersDB.GetByEstNumberLastVers_1ROW_Async(miEst.estHeaderDB.estnumber);

        // Nueva version usando el numero anterior.
        miEst.estHeaderDB.estnumber=estNumber;
        miEst.estHeaderDB.estvers=await _unitOfWork.EstimateHeadersDB.GetNextEstVersByEstNumber(estNumber);

        if(miEst.estHeaderDB.estvers==0)
        {
            return null;
        }

        // Grabo la estampa de tiempo en el header
        miEst.estHeaderDB.htimestamp=DateTime.Now;


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

        // Traduzco el id del pais / region en un string de 3 caracteres normalizado.
        // Sera clave para bifurcar la logica del calculo segun los diferentes paises
         myEstV2=await getCountry(myEstV2);



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

        // Atenti con la primera pregunta. Me fijo en como vino el estimate original
        // Sio su estatus era 0, pasa a 1 automaticamente por que este fue el primer upgrade
        // IGNORO el JSON.
        // Todas las demas preguntas seran en base al valor que venga del json.
        if(miEst.estHeaderDB.status==0)
        {   
            resultEDB.estHeaderDB.status=1;
        }
        else if(resultEDB.estHeaderDB.status==1)
        {   // Estado 0 y 1 perteneces a sourcing. No leo los extragastos del json
            // los piso con 0
            resultEDB=myDBhelper.ClearExtraGastosComex(resultEDB);
            resultEDB=myDBhelper.ClearExtraGastosFinanzas(resultEDB);
        }
        else if(resultEDB.estHeaderDB.status==2)
        {   // Estado 2 pertenes a comex. Ellos solo pueden editar sus extragastos.
            if(resultEDB.estHeaderDB.tarifupdate>estHDBPrevia.tarifupdate)
            {
                 _estService.setLastError("No se pueden actualizar tarifas de gloc previamente congelados. Accion Rechazada");
                return null;
            }
            // Los extragastos finanzas (numericos y formulas los piso con 0)
            resultEDB=myDBhelper.ClearExtraGastosFinanzas(resultEDB);
        }
        else if(resultEDB.estHeaderDB.status==3)
        {   // Estdo 3 pertenece a Finanzas. Ellos solo pueden tocar sus extragastos numericos y formulas.
            // Protejo los extragastos de COMEX. miEst es el estimate que levante de la base antes de comenzar con
            // el update.
            
            // Preservo la seleccion de las tarifas por ID. Finanzas no puede cambiar las tarifas, solo editart los gloc
            resultEDB.estHeaderDB.tarifasbancos_id=estHDBPrevia.tarifasbancos_id;
            resultEDB.estHeaderDB.tarifasdepositos_id=estHDBPrevia.tarifasdepositos_id;
            resultEDB.estHeaderDB.tarifasdespachantes_id=estHDBPrevia.tarifasdespachantes_id;
            resultEDB.estHeaderDB.tarifasflete_id=estHDBPrevia.tarifasflete_id;
            resultEDB.estHeaderDB.tarifasfwd_id=estHDBPrevia.tarifasfwd_id;
            resultEDB.estHeaderDB.tarifasgestdigdoc_id=estHDBPrevia.tarifasgestdigdoc_id;
            resultEDB.estHeaderDB.tarifaspolizas_id=estHDBPrevia.tarifaspolizas_id;
            resultEDB.estHeaderDB.tarifasterminales_id=estHDBPrevia.tarifasterminales_id;
        
            // Finanzas no puede ordenar una actualizacion de tarfias desde la DB.
            // Solo habilito los bits 8 y 9 para que puedan ajustar los costos de flete y seguro.
            // El resto de los flags de update, se los anulo. Aqui no tengo en cuenta el JSON.
            resultEDB.estHeaderDB.tarifupdate=(1<<(int)tarifaControl.freight_cost)+(1<<(int)tarifaControl.freight_insurance_cost);
            resultEDB.estHeaderDB.tarifrecent=0;

            // Preservo los gastos de comex. Ignoro los que vienen del JSON.
            resultEDB.estHeaderDB.extrag_comex1=estHDBPrevia.extrag_comex1;
            resultEDB.estHeaderDB.extrag_comex2=estHDBPrevia.extrag_comex2;
            resultEDB.estHeaderDB.extrag_comex3=estHDBPrevia.extrag_comex3;
            resultEDB.estHeaderDB.extrag_comex4=estHDBPrevia.extrag_comex4;
            resultEDB.estHeaderDB.extrag_comex5=estHDBPrevia.extrag_comex5;
            resultEDB.estHeaderDB.extrag_comex_notas=estHDBPrevia.extrag_comex_notas;
        }
        else
        {
            _estService.setLastError("ESTADO INVALIDO !!!. Proceso DETENIDO");
            return null;
        }

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
        await _estService.loadContenedor(myEstV2);

        myEstV2=myCalc.calcReclaim(myEstV2);
        
        return myEstV2;      
    }

    public async Task<EstimateV2> getCountry(EstimateV2 miEst)
    {
        PaisRegion pais=await _unitOfWork.PaisesRegiones.GetByIdAsync(miEst.estHeader.paisregion_id);
        string pais_str=pais.description;
        if(pais_str.ToUpper().Contains("BRA"))
        {
            miEst.pais="BRA";
        }
        else if(pais_str.ToUpper().Contains("MEX"))
        {
            miEst.pais="MEX";       
        }
        else if(pais_str.ToUpper().Contains("ARG"))
        {
            miEst.pais="ARG";
        }
        else if(pais_str.ToUpper().Contains("USA")||(pais_str.ToUpper().Contains("ESTADOS")))
        {
            miEst.pais="USA";
        }
        else if(pais_str.ToUpper().Contains("COL"))
        {
            miEst.pais="COL";
        }
        else
        {
        miEst.pais="";
        }
        return miEst;
    }

}