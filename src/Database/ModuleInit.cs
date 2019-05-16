using Autofac;
using Microsoft.EntityFrameworkCore.Design;
using Database.EntityFrameworkCore;
using Database.Repositories;
using Database.UnitOfWork;

namespace Database
{
    public class ModuleInit : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork.UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<DbContextFactory>().As<IDesignTimeDbContextFactory<APIDbContext>>().InstancePerLifetimeScope();

            // There are 2 types of generic repository: IRepository<TEntity> and IRepository<TEntity, in TPrimaryKey>
            // That's why we need to register both generic types here
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>)).InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
