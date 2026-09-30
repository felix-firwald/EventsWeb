
using EventsWeb.Api.Middleware;
using EventsWeb.Core.Interface;
using EventsWeb.Services.Services;
using System.Runtime.CompilerServices;

namespace EventsWeb.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();
            Program.RegisterServicesLayer(builder.Services);
            builder.Host.UseDefaultServiceProvider(options =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            });
            WebApplication app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            app.MapControllers();
            app.Run();
        }
        private static void RegisterServicesLayer(IServiceCollection col)
        {
            col.AddSingleton<IEventService, EventService>();
            col.AddSingleton<IBookingService, BookingService>();
        }
    }
}
