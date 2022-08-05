using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MEI.SPDocuments
{
    //public class ServiceProvider
    //    : IServiceProvider
    //{
    //    private readonly IServiceProvider _next;
    //    private readonly IServiceProvider _serviceProvider;
    //    private readonly IHttpContextAccessor _accessor;
    //    private readonly ConcurrentDictionary<Type, bool> _typesCannotResolve = new ConcurrentDictionary<Type, bool>();
    //    private const int typesCannotResolveCacheCap = 100000;

    //    public ServiceProvider(IServiceProvider next, IServiceProvider serviceProvider, IHttpContextAccessor accessor)
    //    {
    //        _next = next;
    //        _serviceProvider = serviceProvider;
    //        _accessor = accessor;
    //    }

    //    public object GetService(Type serviceType)
    //    {
    //        if (_typesCannotResolve.ContainsKey(serviceType))
    //        {
    //            return DefaultCreateInstance(serviceType);
    //        }

    //        object result = null;

    //        try
    //        {
    //            result = ScopeLifetimeResolve(serviceType);
    //        }
    //        catch
    //        {}

    //        if (result != null)
    //        {
    //            return result;
    //        }

    //        result = _next?.GetService(serviceType);

    //        if (result != null)
    //        {
    //            return result;
    //        }

    //        result = DefaultCreateInstance(serviceType);

    //        if (result == null)
    //        {
    //            return null;
    //        }

    //        if (_typesCannotResolve.Count < typesCannotResolveCacheCap)
    //        {
    //            _typesCannotResolve.TryAdd(serviceType, true);
    //        }

    //        return result;
    //    }

    //    protected virtual object DefaultCreateInstance(Type type)
    //    {
    //        return Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
    //    }

    //    private object ScopeLifetimeResolve(Type type)
    //    {
    //        IServiceScope lifetimeScope;
    //        HttpContext currentHttpContext = _accessor.HttpContext;

    //        if (currentHttpContext != null)
    //        {
    //            lifetimeScope = (IServiceScope)currentHttpContext.Items[typeof(IServiceScope)];

    //            if (lifetimeScope == null)
    //            {
    //                void CleanScope(object sender, EventArgs args)
    //                {
    //                    if (sender is HttpApplication application)
    //                    {
    //                        application.RequestCompleted -= CleanScope;

    //                        lifetimeScope.Dispose();
    //                    }
    //                }

    //                lifetimeScope = _serviceProvider.CreateScope();
    //                currentHttpContext.Items.Add(typeof(IServiceScope), lifetimeScope);
    //                currentHttpContext.ApplicationInstance.RequestCompleted += CleanScope;
    //            }
    //        }
    //        else
    //        {
    //            lifetimeScope = _serviceProvider.CreateScope();
    //        }

    //        return ActivatorUtilities.GetServiceOrCreateInstance(lifetimeScope.ServiceProvider, type);
    //    }
    //}
}