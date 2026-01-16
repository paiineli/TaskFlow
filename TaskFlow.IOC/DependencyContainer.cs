using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Data;
using TaskFlow.Repository;

namespace TaskFlow.IOC
{
    public class DependencyContainer
    {
        public static void RegisterContainers(IServiceCollection services)
        {
            // Data Access
            services.AddScoped<AcessaDados>();

            // Repositories - Chamados
            services.AddScoped<ChamadoREP>();
            services.AddScoped<ChamadoComentarioREP>();
            services.AddScoped<ChamadoAnexoREP>();
            services.AddScoped<ChamadoHistoricoREP>();
            services.AddScoped<ChamadoAvaliacaoREP>();
            
            // Repositories - Cadastros Base
            services.AddScoped<CategoriaREP>();
            services.AddScoped<FuncionarioREP>();
            services.AddScoped<JustificativaREP>();
            services.AddScoped<StatusREP>();
            services.AddScoped<EmpresaREP>();
        }
    }
}
