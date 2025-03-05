using Supabase;

namespace WeatherBackend.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterSupabaseClient(this IServiceCollection services, IConfiguration configuration)
        {
            var supabaseUrl = configuration["Supabase:Url"];
            var supabaseKey = configuration["Supabase:Key"];

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };

            var client = new Supabase.Client(supabaseUrl, supabaseKey, options);

            services.AddSingleton(client);

            return services;
        }
    }
}