using Autofac;
using TestTaskApi.Data.Repository;

namespace TestTaskApi
{
    public class AutofacModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PersonRepository>()
                .As<IPersonRepository>().InstancePerLifetimeScope();
            builder.RegisterType<AddressRepository>()
                .As<IAddressRepository>().InstancePerLifetimeScope();

            builder.RegisterType<SimpleJsonParser>()
                .As<IJsonParser>().InstancePerLifetimeScope();
        }
    }
}
