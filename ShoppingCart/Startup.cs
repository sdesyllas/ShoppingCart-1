using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ShoppingCart.Repository;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Mappers;
using ShoppingCart.Shared.Model;

namespace ShoppingCart
{
    public class Startup
    {
        IHostingEnvironment envoirment;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient(typeof(IDataProvider<Cart>), typeof(StaticCartProvider));
            services.AddSingleton(typeof(IRepository<Cart>), typeof(InMemoryCartRepository));
            services.AddTransient(typeof(IDataProvider<Product>), x=> new ProductDataProvider(x.GetService<IFileProvider>(), Configuration.GetValue<string>("SourceFiles")));
            services.AddSingleton(typeof(IQueryableByIdRepository<Product>), typeof(InMemoryProductReposiotry));
            services.AddTransient(typeof(IFileProvider), x => envoirment.ContentRootFileProvider);

            services.AddTransient(typeof(IValueResolver<CartItem, CartItemDto, CartProductDto>), typeof(ProductDtoResolver));
            services.AddTransient(typeof(IMapperProvider<Product, CartProductDto>), typeof(ProductMapperProvider));
            services.AddTransient(typeof(IMapperProvider<Cart, CartDto>), typeof(CartMapperProvider));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseMvc();
            envoirment = env;
        }
    }

    public class Config
    {
        public string Products { get; set; }
    }
}
