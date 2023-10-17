namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(
                      IIIBBrepository IIBBrepository,
                      INCMrepository NCMrepository,
                      INCM_MexRepository NCM_MexRepo,
                      IBancoRepository BancoRepository,
                      IFwdtteRepository FwdtteRepository,
                      ITerminalRepository TermimalRepository,
                      IFleteRepository FleteRepository,
                      ICustodiaRepository CustodiaRepository,
                      IGestDigitalDocRepository GestDigDocRepository,
                      IEmpresaRepository EmpresaRepository,
                      ITruckSemiRepository TruckSemiRepo,
                      IDepositoRepository DepositoRepository,
                      IPolizaRepository PolizaRepository,
                      IImpuestoRepository ImpuestoRepository,
                      ICanalRepository CanalRepository,
                      IProveedorRepository ProveedorRepository,
                      IEstimateHeaderDBRepository EstimateHeaderDBRepository,
                      IEstimateDetailDBRepository EstimateDetailDBRepository,
                      ITarifasDepositoRepository tarifasDepositoRepository,
                      ITarifasFwdRepository MiTarifRepo,
                      ICargaRepository CargaRepo,
                      ITarifasPolizaRepository TarPolizas,
                      //ITarifasSeguroRepository tarifasSeguroRepository,
                      //ITarifasTerminalRepository tarifasTerminalRepository,
                      //ITarifasTteRepository tarifasTteRepository,
                      ITipoDeCambioRepository tipoDeCambioRepository,
                      ITarifasTerminalRepository MiTarifTerminalRepo,
                      ICnstRepository cnstRepository,
                      IUsuarioRepository UsuarioRepository,
                      IDespachanteRepository DespachanteRepository,
                      IPaisRegionRepository PaisRegionRepo,
                      ITarifasBancoRepository MiTarifBancos,
                      ITarifasDespachanteRepository MiTarifDespa, 
                      ITarifasFleteRepository MiTarifFlete,
                      ITarifasGestDigDocRepository MiTarifGestDigDoc,  
                      //ITarifonMexRepository MiTarifonMX  
                      IProductoRepository misProds                 
                      )
    {
        IIBBs = IIBBrepository;
        NCMs = NCMrepository;
        NCM_MEXs=NCM_MexRepo;
        Bancos = BancoRepository;
        Fwds = FwdtteRepository;
        Terminales = TermimalRepository;
        Fletes = FleteRepository;
        Custodias = CustodiaRepository;
        GestDigDoc = GestDigDocRepository;
        Empresas = EmpresaRepository;
        Camiones = TruckSemiRepo;
        Depositos = DepositoRepository;
        Polizas = PolizaRepository;
        Impuestos = ImpuestoRepository;
        Canales = CanalRepository;
        Proveedores = ProveedorRepository;
        EstimateHeadersDB = EstimateHeaderDBRepository;
        EstimateDetailsDB = EstimateDetailDBRepository;        
        TarifasDepositos = tarifasDepositoRepository;
        TarifFwd = MiTarifRepo;
        micarga = CargaRepo;
        TarifFlete = MiTarifFlete;
        TarifPoliza = TarPolizas;
        //TarifasSeguros = tarifasSeguroRepository;
        //TarifasTerminales = tarifasTerminalRepository;
        //TarifasTransportes = tarifasTteRepository;

        //EstimateDetails = EstimateDetailRepository;
        TiposDeCambio = tipoDeCambioRepository;
        TarifTerminal = MiTarifTerminalRepo;
        Constantes = cnstRepository;
        Usuarios = UsuarioRepository;
        Despachantes = DespachanteRepository;
        PaisesRegiones = PaisRegionRepo;
        TarifBancos = MiTarifBancos;
        TarifDespa = MiTarifDespa;
        TarifGestDigDoc = MiTarifGestDigDoc;
        //TarifonMX = MiTarifonMX;
        Productos = misProds;

    }

    public IIIBBrepository IIBBs { get; } 
    public INCMrepository NCMs { get;}
    public INCM_MexRepository NCM_MEXs { get;}
    public IBancoRepository Bancos {get;}
    public IFwdtteRepository Fwds {get;}
    public ITerminalRepository Terminales {get; }
    public IFleteRepository Fletes {get; }
    public ICustodiaRepository Custodias {get; }
    public IGestDigitalDocRepository GestDigDoc {get; }
    public IEmpresaRepository Empresas { get; }
    public ITruckSemiRepository Camiones { get; }
    public IDepositoRepository Depositos { get; }
    public IPolizaRepository Polizas { get; }
    public IImpuestoRepository Impuestos { get; }
    public IEstimateHeaderDBRepository EstimateHeadersDB { get;}
    public IEstimateDetailDBRepository EstimateDetailsDB { get;}
    public ICanalRepository Canales {get;}
    public IProveedorRepository Proveedores {get;}
    
    public ICargaRepository micarga {get;}
    
    //public ITarifasSeguroRepository TarifasSeguros {get;}
    //public ITarifasTerminalRepository TarifasTerminales{get;}
    //public ITarifasTteRepository TarifasTransportes {get;}
    public ITipoDeCambioRepository TiposDeCambio { get; }
    public ITarifasTerminalRepository TarifTerminal { get;}
    public ITarifasPolizaRepository TarifPoliza {get;}
    public ITarifasFleteRepository TarifFlete {get;}
    public ITarifasDepositoRepository TarifasDepositos {get;}
    public ITarifasFwdRepository TarifFwd {get;}
    public ITarifasBancoRepository TarifBancos {get;}
    public ITarifasDespachanteRepository TarifDespa {get;}
    public ITarifasGestDigDocRepository  TarifGestDigDoc {get;}

    //public ITarifonMexRepository TarifonMX {get;}
    public ICnstRepository Constantes{get;}
    public IUsuarioRepository Usuarios { get; }
    public IDespachanteRepository Despachantes { get;}
    public IPaisRegionRepository PaisesRegiones {get;}

    public IProductoRepository Productos {get;}

}