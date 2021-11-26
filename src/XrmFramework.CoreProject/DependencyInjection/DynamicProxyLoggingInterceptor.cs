#if !DISABLE_DI

using System.Reflection;
using Castle.DynamicProxy;
using Serilog;
using Serilog.Core;

namespace XrmFramework.DependencyInjection
{
    // Simple sample DispatchProxy-based logging proxy
    public class DynamicProxyLoggingInterceptor : IInterceptor
    {
        // The Serilog logger to be used for logging.
        private readonly Logger _logger;

        // Constructor that initializes the Logger the interceptor will use
        public DynamicProxyLoggingInterceptor(string typeName = null)
        {
            // Setup the Serilog logger
            _logger = new LoggerConfiguration()
                .WriteTo.Console().CreateLogger();
            _logger.Verbose($"New logging decorator created{(string.IsNullOrWhiteSpace(typeName) ? string.Empty : " for object of type {TypeName}")}", typeName);
        }

        // The Intercept method is where the interceptor decides how to handle calls to the proxy object
        public void Intercept(IInvocation invocation)
        {
            try
            {
                // Perform the logging that this proxy is meant to provide
                _logger.Information("Calling method {TypeName}.{MethodName} with arguments {Arguments}", invocation.Method.DeclaringType.Name, invocation.Method.Name, invocation.Arguments);

                // Invocation.Proceeds goes on to the next interceptor or, if there are no more interceptors, invokes the method.
                // The details of how the method are invoked will depend on the proxying model used. The interceptor does not need
                // to know those details.
                invocation.Proceed();

                // A little more logging.
                _logger.Information("Finished calling method {TypeName}.{MethodName}", invocation.Method.DeclaringType?.Name, invocation.Method.Name);
            }
            catch (TargetInvocationException exc)
                when (exc.InnerException != null)
            {
                // If the subsequent invocation fails, log a warning and then rethrow the exception
                _logger.Warning(exc.InnerException, "Method {TypeName}.{MethodName} threw exception: {Exception}", invocation.Method.DeclaringType?.Name, invocation.Method.Name, exc.InnerException);

                throw exc.InnerException;
            }
        }
    }
}

#endif