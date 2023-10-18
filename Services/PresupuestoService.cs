namespace WebApiSample.Core;

using WebApiSample.Infrastructure; 
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;
using System.Diagnostics.Metrics;


// LISTED 9_8_2023 11:36 AM    
// LISTED 11_09_2023 17:15 Primera logica de estados aplicada a presupuestoUpdate.
// LISTED 12_09_2023 17:21 Repara bug en calc reclaim. Comenta calc flete total y seguro total ya que
// su valor fue calculado durante el POST y guardado en flete_cost flete_cost_insurance.
// LISTED 13_09_2023 10:36 Repara bug pais en calculo fob reclaim. Se reordena el codigo para evitar
// repeticion. Se habilita el modo simulacion.  
// Listed 14_09_2023 15:57. SE agregan los roles a la logica de estados. Los roles llegan en un string concatenado
// desde el controller mismo que saca los claims. Un metodo permite saber si un permiso dado esta o no en el string
// Se reordena create / update y sim. Se concatena en el owner los permisos usados.
// ADVERTENCIA:
// Cuidado con miEstV2 y miEst cuando se copian los headers de modo directo, parece mas bien la misma instancia
// del objeto en memoria, ya que alterar el own en uno, se altera en el otro.
// LISTED 19_09_2023 Se acomodola la propagacion de errores. Se arregla bug de enmascaramiento de tarifas no usadas en MEX
// y el valor de tarifupdate y recent se inicializa en 255 y no en 1023. De este modo se puede ingresar ya desde la creacion
// valores manuales para flete y seguro. 
// LISTED 11_10_2023 Se agrega Enumerador a los articulos, tanto en create como en update (solo los art nuevos)
// LIESTED 18_20_2023. En submitupload, el if para el estado 0 es solo para pasarlo automaticamente a 1, sin lugar
// a que entre en la logica de estados y gastos (antes parte del else/if de la logica de estados)
// LISTED 18_10_2023 Repara faltantes en extrag_src y src_notas 


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
    // ########### NUEVO PRESUPUESTO ###############
    // Un prespuesto nuevo consisten en los calculos (EstimateDB -> EstimateV2)
    // Y luego la conversion inversa (EstimateV2 a estimateDB) y su posterior guardado a la base
    // #############################################
    public async Task<EstimateV2>submitPresupuestoNew(EstimateDB miEst)
    {
        var result=0;
        EstimateV2 myEstV2=new EstimateV2();
        dbutils myDBhelper=new dbutils(_unitOfWork);
        EstimateDB resultEDB=new EstimateDB();
        EstimateHeaderDB readBackHeader=new EstimateHeaderDB();

       
 
        miEst.estHeaderDB.estnumber=await _unitOfWork.EstimateHeadersDB.GetNextEstNumber();
        miEst.estHeaderDB.estvers=1;

        // Grabo la estampa de tiempo en el header
        miEst.estHeaderDB.htimestamp=DateTime.Now;

        // Expando
        myEstV2=myDBhelper.transferDataFromDBType(miEst);

        // Es un presupuesto nuevo ... me aseguro que todo lo que es extragasto este en cero y los flags de update en false
        myEstV2=myDBhelper.setDefaultEstimateDB(myEstV2);

        myEstV2=await calcularPresupuestoNewOrUpdate(myEstV2);

        if(myEstV2==null)
        {
            return null; 
        }



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
        
        // Cada articulo tiene x defecto un ID que lse sera unico a lo largo de todas las versiones. Se le agina ACA. Es automatico y transparente al usuario
        int enumerador=111200000;
        // Ahora si, inserto los detail uno a uno ne la base
        foreach(EstimateDetailDB ed in resultEDB.estDetailsDB)
        {
            ed.detailorder=enumerador;
            enumerador++;
            ed.estimateheader_id=readBackHeader.id; // El ID que la base le asigno al header que acabo de insertar.
            ed.updated=false;                       // El calculo se hizo por completo. No hay actualizaciones epndientes. Borro el flag de cada producto.
            result+=await _unitOfWork.EstimateDetailsDB.AddAsync(ed);
        }
        
        return myEstV2;      
    }

    // Una simulacion, es solo los calculos.
    public async Task<EstimateV2>simulaPresupuesto(EstimateDB miEst)
    {
        dbutils myDBhelper=new dbutils(_unitOfWork);
        EstimateV2 miEstV2=new EstimateV2();
        // Expando
        miEstV2=myDBhelper.transferDataFromDBType(miEst);
        return await calcularPresupuestoNewOrUpdate(miEstV2);
    }

    //  ########## CALCULOS ###########
    // Toma un EstimaV2 y hace los calculos.
    //  ###############################
    public async Task<EstimateV2>calcularPresupuestoNewOrUpdate(EstimateV2 myEstV2)
    {
        var result=0;
        dbutils myDBhelper=new dbutils(_unitOfWork);
        EstimateDB resultEDB=new EstimateDB();
        EstimateHeaderDB readBackHeader=new EstimateHeaderDB();





        //#-----ARREGLAR------##### => miEst.estHeaderDB.tarifasbydateid=myTar.id;

        // Cuando me pasan el presupuesto con dolar billete "-1" es por que debo extraerlo
        // desde la base TC-CDA. 
        if(myEstV2.estHeader.dolar<0)
        {
            // Leo la fecha y la paso al formato que le gusta a postgre
            string hoy=DateTime.Now.ToString("yyyy-MM-dd");
            // Consulto el TC del dia (si existe, ojo)
            double tipoDeCambio= await _unitOfWork.TiposDeCambio.GetByDateAsync(hoy); 
            if(tipoDeCambio>0)
            {   // La consulta tuvo exito ?
                myEstV2.estHeader.dolar=tipoDeCambio;
            }
            else
            {   // FALLO, no existe el TC de la fecha mencionada*/ !!!!!!!
                presupError="Se solicito usar TC de la base TC_CDA, pero el dia de la fecha no tiene valor asignado";
                return null;
            }
        }

        // ################################### INIT ######################################
        
        
        // Expando el EstimateDB a un EstimateV2
        //myEstV2=myDBhelper.transferDataFromDBType(miEst);
        // Cargo las constnates de calculo.
        myEstV2.constantes=await _cnstService.getConstantesLastVers();
        if(myEstV2.constantes==null)
        {
            presupError="No existe una instancia de constantes en tabla";
            return null;
        }

        // Ingresa las constantes a la varieble local de _estService.
         _estService.setConstants(myEstV2.constantes);
         // est service carga los datos del contenedor referenciado en una variable de la clase para su posterior uso
         _estService.loadContenedor(myEstV2);

        // Traduzco el id del pais / region en un string de 3 caracteres normalizado.
        // Sera clave para bifurcar la logica del calculo segun los diferentes paises
         myEstV2=await getCountry(myEstV2);
         if(myEstV2.pais=="")
         {
            presupError="Pais no identificado";
            return null;
         }



        // Busca las tarifas, calcula cada uno de los gastos locales y los pasa al header segun sea
        // necesario. Luego popula todas las columnas de gastos locales ponderando por el FP.
        // El FP solo estara disponible luego de calculado el FOB TOTAL.
         myEstV2=await _estService.loadTarifas(myEstV2);
         if(myEstV2==null)
         {
            presupError=_estService.getLastError();
            return null;
         }

        // Si mando archivos con los IDs en 0 que otrohora tenian la opcion de recent activada, cuando los mandos
        // sin la misma pueden ocasionar un 500. Voy a quedar bien detectando el error.
        if(myEstV2.estHeader.tarifasbancos_id==0 ||
           myEstV2.estHeader.tarifasdepositos_id==0 ||
           myEstV2.estHeader.tarifasdespachantes_id==0 ||
           myEstV2.estHeader.tarifasflete_id==0 ||
           myEstV2.estHeader.tarifasfwd_id==0 ||
           myEstV2.estHeader.tarifasgestdigdoc_id==0 ||
           myEstV2.estHeader.tarifaspolizas_id==0 ||
           myEstV2.estHeader.tarifasterminales_id==0)
           {
                presupError="Uno o mas IDs (FKs) a las Tablas Tarfias es 0. Elija todas las tarfias antes de congelar !!!!";
                return null;
           }


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
        return myEstV2;
    }

    // UPDATE PRESUPUESTO
    public async Task<EstimateV2>submitPresupuestoUpdated(int estNumber,EstimateDB miEst,string permisos)
    {
        var result=0;
        dbutils dbhelper=new dbutils(_unitOfWork);
        EstimateV2 ret=new EstimateV2();
        EstimateV2 myEstV2=new EstimateV2();
        dbutils myDBhelper=new dbutils(_unitOfWork);
        EstimateDB resultEDB=new EstimateDB();
        EstimateHeaderDB estHDBPrevia=new EstimateHeaderDB();
        EstimateHeaderDB readBackHeader=new EstimateHeaderDB();
        Cliente cli=new Cliente();
        // No usar el estNumber del JSON. OJO. Puede ser cualquiera. En gral no se le da bola. Para traer el previo
        // consultar usando el estNumber pasado como parametro en el POST.
        // Leo la ultima version del estimate. Se usara para comparar algunos cambios de valores o preservarlos ante
        // cambios no deseados.
        estHDBPrevia=await _unitOfWork.EstimateHeadersDB.GetByEstNumberLastVers_1ROW_Async(estNumber);


        // Controlo que no me retrocedan el estado, salvo el jefe.
        if(miEst.estHeaderDB.status<estHDBPrevia.status && !cli.isGranted(permisos,cli.boss))
        {
            presupError="No se puede retroceder el estado. Operacion Rechazada";
            return null;
        }

        // ##############################################################################
        // Control del estado VS los permisos del usuario que esta accediendo.
        // Controlo que al estado 1 solo acceda sourcingo  jefe
        if(miEst.estHeaderDB.status<2 && !(cli.isGranted(permisos,cli.sourcing)||cli.isGranted(permisos,cli.boss)))
        {
            presupError="Usuario no habilitado para operar en el presente estado";
            return null;
        }
        // Al estado 2 solo acceden comex o jefe
        if(miEst.estHeaderDB.status==2 && !(cli.isGranted(permisos,cli.comex)||cli.isGranted(permisos,cli.boss)))
        {
            presupError="Usuario no habilitado para operar en el presente estado";
            return null;
        }
        // Al estado 3 solo acceden finanzas o jefe.
        if(miEst.estHeaderDB.status==3 && !(cli.isGranted(permisos,cli.finanzas)||cli.isGranted(permisos,cli.boss)))
        {
            presupError="Usuario no habilitado para operar en el presente estado";
            return null;
        }
        // ##############################################################################

        // Nueva version usando el numero anterior.
        miEst.estHeaderDB.estnumber=estNumber;
        miEst.estHeaderDB.estvers=await _unitOfWork.EstimateHeadersDB.GetNextEstVersByEstNumber(estNumber);

        if(miEst.estHeaderDB.estvers==0)
        {
            return null;
        }

        // Grabo la estampa de tiempo en el header
        miEst.estHeaderDB.htimestamp=DateTime.Now;

        // Expando
        myEstV2=myDBhelper.transferDataFromDBType(miEst);


        myEstV2=await calcularPresupuestoNewOrUpdate(myEstV2);
        if(myEstV2==null)
        {
            return null;
        }
        // Le pongo la fecha / hora !!!!!!
        //ret.estHeader.hTimeStamp=DateTime.Now;



        // Atenti con la primera pregunta. Me fijo en como vino el estimate original
        // Sio su estatus era 0, pasa a 1 automaticamente por que este fue el primer upgrade
        // IGNORO el JSON.
        // Todas las demas preguntas seran en base al valor que venga del json.
        if(estHDBPrevia.status==0)
        {   
            myEstV2.estHeader.status=1;
        }

        if(myEstV2.estHeader.status==1)
        {   // Estado 0 y 1 perteneces a sourcing. No leo los extragastos del json
            // los piso con 0
            myEstV2=myDBhelper.ClearExtraGastosComex(myEstV2);
            myEstV2=myDBhelper.ClearExtraGastosFinanzas(myEstV2);
        }
        else if(myEstV2.estHeader.status==2)
        {   // Estado 2, provisorio. Usado x Comex. Pueden congelar las tarfias pero no descongelarlas.
            // salvo que sea jefe.
            if(myEstV2.estHeader.tarifupdate>estHDBPrevia.tarifupdate && !cli.isGranted(permisos,cli.boss))
            {
                presupError="No se pueden actualizar tarifas de gloc previamente congelados. Accion Rechazada";
                return null;
            }
            // En el estado 2, Los extragastos finanzas (numericos y formulas .. NO SE PUEDEN TOCAR. Los piso con 0)
            myEstV2=myDBhelper.ClearExtraGastosFinanzas(myEstV2);
            // Preservo los extrag de sourcing:
            myEstV2.estHeader.extrag_src1=estHDBPrevia.extrag_src1;
            myEstV2.estHeader.extrag_src2=estHDBPrevia.extrag_src2;
            myEstV2.estHeader.extrag_src_notas=estHDBPrevia.extrag_src_notas;
        }
        else if(myEstV2.estHeader.status==3)
        {   // Estdo 3, definitivo,  pertenece a Finanzas. Ellos solo pueden tocar sus extragastos numericos y formulas.
            // Protejo los extragastos de COMEX. estHDBPrevia es el hader de la version anterior que la tengo cargada para
            // respaldar o enmascarar aquellos datos que no no corresponda editar.
            // el update.
            
            // Preservo la seleccion de las tarifas por ID. Finanzas no puede cambiar las tarifas, solo editart los gloc
            myEstV2.estHeader.tarifasbancos_id=estHDBPrevia.tarifasbancos_id;
            myEstV2.estHeader.tarifasdepositos_id=estHDBPrevia.tarifasdepositos_id;
            myEstV2.estHeader.tarifasdespachantes_id=estHDBPrevia.tarifasdespachantes_id;
            myEstV2.estHeader.tarifasflete_id=estHDBPrevia.tarifasflete_id;
            myEstV2.estHeader.tarifasfwd_id=estHDBPrevia.tarifasfwd_id;
            myEstV2.estHeader.tarifasgestdigdoc_id=estHDBPrevia.tarifasgestdigdoc_id;
            myEstV2.estHeader.tarifaspolizas_id=estHDBPrevia.tarifaspolizas_id;
            myEstV2.estHeader.tarifasterminales_id=estHDBPrevia.tarifasterminales_id;
        
            // Finanzas no puede ordenar una actualizacion de tarfias desde la DB.
            // Solo habilito los bits 8 y 9 para que puedan ajustar los costos de flete y seguro.
            // El resto de los flags de update, se los anulo. Aqui no tengo en cuenta el JSON.
            myEstV2.estHeader.tarifupdate=(1<<(int)tarifaControl.freight_cost)+(1<<(int)tarifaControl.freight_insurance_cost);
            myEstV2.estHeader.tarifrecent=0;

            // Preservo los gastos de comex. Ignoro los que vienen del JSON.
            myEstV2.estHeader.extrag_src1=estHDBPrevia.extrag_src1;
            myEstV2.estHeader.extrag_src2=estHDBPrevia.extrag_src2;
            myEstV2.estHeader.extrag_src_notas=estHDBPrevia.extrag_src_notas;
            myEstV2.estHeader.extrag_comex1=estHDBPrevia.extrag_comex1;
            myEstV2.estHeader.extrag_comex2=estHDBPrevia.extrag_comex2;
            myEstV2.estHeader.extrag_comex3=estHDBPrevia.extrag_comex3;
            myEstV2.estHeader.extrag_comex_notas=estHDBPrevia.extrag_comex_notas;
        }
        else
        {
            presupError="ESTADO INVALIDO !!!. Proceso DETENIDO";
            return null;
        }

        myEstV2.estHeader.own=myEstV2.estHeader.own + $" - [PERMISOS:{permisos}]";

        
        // lo que me deuvelve la rutina de calculo es un EstimateV2, cuyo Detail es mucho mas extenso
        // En la base no se guardan calculos,  por lo que debi convertir el estimate V2 a estimate DB y guardarlo.
        resultEDB=myDBhelper.transferDataToDBType(myEstV2);
        // Guardo el header.
        result=await _unitOfWork.EstimateHeadersDB.AddAsync(resultEDB.estHeaderDB);
        // Veo que ID le asigno la base:
        readBackHeader=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(resultEDB.estHeaderDB.estnumber,miEst.estHeaderDB.estvers);
        // El enumerador, su valor base, es DIFERENTE en cada version. Con esto garantizo que si debo asignar un ID a un prod nuevo (agregado)
        // jamas usare un ID que figure en versiones antriores, ya que seria el ID -  1000.
        int enumerador=111200000+(1000*resultEDB.estHeaderDB.estvers);
        // Ahora si, inserto los detail uno a uno ne la base
        foreach(EstimateDetailDB ed in resultEDB.estDetailsDB)
        {
            // Durante un update, pueden haber AGREGADO UN PRODUCTO. Es IMPERATIVO que el front lo mande detail order en 0 para indicar 
            // esto y poder darle un ID;
            // Le asigno un numero. Enumerador es funcion de la version. Jamas concidira con codigos usados en versiones anteriores.
            if(ed.detailorder==0)    
            {
                ed.detailorder=enumerador;
            }

            enumerador++;

            if(miEst.estHeaderDB.status<2)
            {
                ed.extrag_comex1=0;
                ed.extrag_comex2=0;
                ed.extrag_comex3=0;
                ed.extrag_comex_notas="";
            }
            if(miEst.estHeaderDB.status<3)
            {
                ed.extrag_finan1=0;
                ed.extrag_finan2=0;
                ed.extrag_finan3=0;
                ed.extrag_finan_notas="";
            }

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
        EstimateHeaderDBVista miEstHeaderV=new EstimateHeaderDBVista();
        List<EstimateDetailDBVista> miEstDetV=new List<EstimateDetailDBVista>();

        // Levanto el header segun numero y version
        miEstHeaderV=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersVistaAsync(estNumber,estVers);
        if(miEst.estHeaderDB ==null)
        {   // OJO
            presupError=$"No de puede recuperar el estimate {estNumber}, vers {estVers}";
             return null;
        }

        string miPais=await getCountry(miEstHeaderV);

        // Con el ID del header levanto el estDetail.
        if(miPais=="MEX")
        {
            miEstDetV=(await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeaderVistaMexsync(miEstHeaderV.id)).ToList();
        }
        else
        {
            miEstDetV=(await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeaderVistasync(miEstHeaderV.id)).ToList();
        }
        if(miEstDetV==null)
        {
            presupError=$"No de puede recuperar la version {estVers} del estimate {estNumber}";
            return null;
        }

        // Expando el EstimateDB a un EstimateV2
        myEstV2=dbhelper.transferDataFromDBTypeWithVista(miEstHeaderV,miEstDetV);
        // Cargo las constnates de calculo.
        myEstV2.constantes=await _cnstService.getConstantesLastVers();
        if(myEstV2.constantes==null)
        {
            presupError="No existe una instancia de constantes en tabla";
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
        // Traduzco el id del pais / region en un string de 3 caracteres normalizado.
        // Sera clave para bifurcar la logica del calculo segun los diferentes paises
         myEstV2=await getCountry(myEstV2);
         if(myEstV2.pais=="")
         {
            presupError="Pais no identificado";
            return null;
         }

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

    public async Task<string> getCountry(EstimateHeaderDBVista miEstHV)
    {
        PaisRegion pais=await _unitOfWork.PaisesRegiones.GetByIdAsync(miEstHV.paisregion_id);
        string pais_str=pais.description;
        if(pais_str.ToUpper().Contains("BRA"))
        {
            return "BRA";
        }
        else if(pais_str.ToUpper().Contains("MEX"))
        {
            return "MEX";       
        }
        else if(pais_str.ToUpper().Contains("ARG"))
        {
            return "ARG";
        }
        else if(pais_str.ToUpper().Contains("USA")||(pais_str.ToUpper().Contains("ESTADOS")))
        {
            return "USA";
        }
        else if(pais_str.ToUpper().Contains("COL"))
        {
            return "COL";
        }
        else
        {
        return "";
        }
    }

}