#if !DISABLE_DI

using System;
using Castle.DynamicProxy;

namespace XrmFramework.DependencyInjection
{
    public class DynamicProxyLoggingDecorator
    {
        // ProxyGenerator is used to create DynamicProxy proxy objects
        private static readonly ProxyGenerator Generator = new ProxyGenerator();

        // CreateInterfaceProxyWithTarget uses composition-based proxying to wrap a target object with
        // a proxy object implementing the desired interface. Calls are passed to the target object
        // after running interceptors. This model is similar to DispatchProxy.
        public static object Decorate(Type interfaceType, object target)
            => Generator.CreateInterfaceProxyWithTargetInterface(interfaceType, target, new DynamicProxyLoggingInterceptor(interfaceType.Name));

        public static T Decorate<T>(object target) where T : IService
            => (T) Decorate(typeof(T), target);
    }
}

#endif