using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using masterdatalab.domain.Services;
using masterdatalab.domain.Repositories;

namespace masterdatalab.domain.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<PedidoService>();
            services.AddScoped<PedidoItemService>();
            services.AddScoped<CadastroService>();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection repositories)
        {
            repositories.AddScoped<ClienteRepository>();
            repositories.AddScoped<ProdutoRepository>();
            repositories.AddScoped<TabelaPrecoRepository>();
            repositories.AddScoped<SituacaoRepository>();
            repositories.AddScoped<PedidoRepository>();
            repositories.AddScoped<ItemPedidoRepository>();
            return repositories;
        }
    }
}
