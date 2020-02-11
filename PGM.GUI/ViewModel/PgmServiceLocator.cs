using System.Reflection;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PGM.GUI.AutoMapper;
using PGM.GUI.View;
using PGM.Service;
using IContainer = Autofac.IContainer;

namespace PGM.GUI.ViewModel
{
    public class PgmServiceLocator
    {
        private static AutofacServiceLocator _autofacServiceLocator;
        private static IContainer _container;
        private static ILifetimeScope _currentScope;
        private static bool _isServiceLocatorSet;

        public static IServiceLocator Current => _isServiceLocatorSet ? ServiceLocator.Current : null;
        
        public static void Reset()
        {
            _currentScope?.Dispose();
            InitServiceLocator();
        }

        public static void Initialise()
        {
            Reset();
        }

        private static void InitServiceLocator()
        {
            if (_autofacServiceLocator == null)
            {
                ContainerBuilder containerBuilder = new ContainerBuilder();
                InitContainer(containerBuilder);
                _container = containerBuilder.Build();
            }

            Messenger.Reset();

            _currentScope = _container.BeginLifetimeScope();
            _autofacServiceLocator = new AutofacServiceLocator(_currentScope);
            _isServiceLocatorSet = true;
            ServiceLocator.SetLocatorProvider(() => _autofacServiceLocator);
        }

        private static void InitContainer(ContainerBuilder containerBuilder)
        {
            Assembly assembly = typeof(PgmServiceLocator).Assembly;

            containerBuilder.RegisterAssemblyTypes(assembly)
                .AssignableTo<ViewModelBase>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Orchestrator"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<MapperVoToModel>().As<IMapperVoToModel>();

            containerBuilder.RegisterType<MainWindow>();

            containerBuilder.RegisterType<ViewModelLocator>();

            containerBuilder.RegisterModule<ServiceAutofacModule>();
        }
    }
}