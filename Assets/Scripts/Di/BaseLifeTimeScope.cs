using VContainer;
using VContainer.Unity;

namespace Di
{
    public class BaseLifeTimeScope : LifetimeScope
    {
        protected IContainerBuilder Builder;

        protected void RegisterWithArgument<T, TParam>(Lifetime lifetime, TParam param) where T : class
        {
            Builder.Register<T>(lifetime).AsImplementedInterfaces().WithParameter(param).AsSelf();
        }
        
        protected void Register<T>(Lifetime lifetime) where T : class
        {
            Builder.Register<T>(lifetime).AsImplementedInterfaces().AsSelf();
        }
        
        protected void RegisterInstance<T>(T instance) where T : class
        {
            Builder.RegisterInstance(instance).AsImplementedInterfaces().AsSelf();
        }
    }
}