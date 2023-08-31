using WebApiSample.Infrastructure;
using WebApiSample.Core;
using WebApiSample.Models;

public static class ServiceRegistration
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IIIBBrepository, IIBBRepository>();
        services.AddTransient<INCMrepository, NCMrepository>();
        services.AddTransient<IBancoRepository, BancoRepository>();
        services.AddTransient<IFwdtteRepository, FwdtteRepository>();
        services.AddTransient<ITerminalRepository, TerminalRepository>();
        services.AddTransient<IFleteRepository, FleteRepository>();
        services.AddTransient<ICustodiaRepository, CustodiaRepository>();
        services.AddTransient<IGestDigitalDocRepository, GestDigitalDocRepository>();
        services.AddTransient<IEmpresaRepository, EmpresaRepository>();
        services.AddTransient<ITruckSemiRepository, TruckSemiRepository>();
        services.AddTransient<ICanalRepository, CanalRepository>();
        services.AddTransient<IProveedorRepository, ProveedorRepository>();
        services.AddTransient<IDepositoRepository, DepositoRepository>();
        services.AddTransient<IPolizaRepository, PolizaRepository>();
        services.AddTransient<IImpuestoRepository, ImpuestoRepository>();
        //services.AddTransient<IProductRepository, ProductRepositoryMemory>();
        services.AddTransient<IEstimateDetailDBRepository, EstimateDetailDBRepository>(); 
        services.AddTransient<IEstimateHeaderDBRepository, EstimateHeaderDBRepository>();
        services.AddTransient<ITarifasDepositoRepository, TarifasDepositoRepository>();
        services.AddTransient<ITarifasFwdRepository, TarifasFwdRepository>();
        services.AddTransient<ICargaRepository, CargaRepository>();
        services.AddTransient<ITipoDeCambioRepository, TipoDeCambioRepository>();
        services.AddTransient<ITarifasTerminalRepository, TarifasTerminalRepository>();
        services.AddTransient<ITarifasPolizaRepository, TarifasPolizaRepository>();
        services.AddTransient<ITarifasFleteRepository, TarifasFleteRepository>();
        services.AddTransient<IEstimateService,EstimateService>();
        services.AddTransient<IEstimateDetailService, EstimateDetailService>();
        services.AddTransient<IPresupuestoService, PresupuestoService>();
        services.AddTransient<ICnstService, CnstService>(); 
        services.AddTransient<ICnstRepository, CnstRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IUsuarioRepository, UsuarioRepository>();
        services.AddTransient<IDespachanteRepository, DespachanteRepository>();
        services.AddTransient<IPaisRegionRepository, PaisRegionRepository>();
        services.AddTransient<ITarifasBancoRepository, TarifasBancoRepository>();
        services.AddTransient<ITarifasDespachanteRepository, TarifasDespachanteRepository>();
        services.AddTransient<ITarifasGestDigDocRepository, TarifasGestDigDocRepository>();
    }
}