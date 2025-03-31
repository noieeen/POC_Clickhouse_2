using Microsoft.EntityFrameworkCore;

namespace Core.Factory.Base
{
    public abstract class Module_Entities_Factory<TContext> where TContext : DbContext
    {
        public abstract string GetConnectionString(string brand_ref);
        public abstract Task<string> GetConnectionStringAsync(string brand_ref);


        public void ConfigDummy(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("blank",
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
        }

        public virtual TContext Create(DbContextOptions<TContext> options)
        {
            return (TContext)Activator.CreateInstance(typeof(TContext), options);
        }

        public virtual TContext Create(string Brand_Ref)
        {
            DbContextOptionsBuilder<TContext> optionsBuilder = Get_DefaultConfig(Brand_Ref);

            TContext entities = Create(optionsBuilder.Options);
            return entities;
        }

        public virtual async Task<TContext> CreateAsync(string Brand_Ref)
        {
            DbContextOptionsBuilder<TContext> optionsBuilder = await Get_DefaultConfigAsync(Brand_Ref);

            TContext entities = Create(optionsBuilder.Options);
            return entities;
        }

        public virtual DbContextOptionsBuilder<TContext> Get_DefaultConfig(string Brand_Ref)
        {
            DbContextOptionsBuilder<TContext> optionsBuilder = new DbContextOptionsBuilder<TContext>();

            //string connStr = "Server=tcp:dev-chococdp2.database.windows.net,1433;Initial Catalog=CDP_4_BL6ZLW8PXBXA;Persist Security Info=False;User ID=chococdp;Password=azuremZj2AjccpVERR5Xdxvus[s2kp0Zz8NC3blWMwdVZaz8rdX-dev;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string connStr = GetConnectionString(Brand_Ref);
            Config_Default(optionsBuilder, connStr);

            return optionsBuilder;
        }

        public virtual async Task<DbContextOptionsBuilder<TContext>> Get_DefaultConfigAsync(string Brand_Ref)
        {
            DbContextOptionsBuilder<TContext> optionsBuilder = new DbContextOptionsBuilder<TContext>();

            string connStr = await GetConnectionStringAsync(Brand_Ref);
            Config_Default(optionsBuilder, connStr);

            return optionsBuilder;
        }

        public bool Config_Default(DbContextOptionsBuilder options, string connStr)
        {
            options.UseSqlServer(connStr,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });

            return true;
        }
    }
}