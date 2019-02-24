using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XNetCoreCommon
{
    public static class ServerDICommon
    {
        public static void AddTransientIDCommon(this IServiceCollection services, string assemblyName)
        {
            if (!String.IsNullOrEmpty(assemblyName))
            {
                Assembly assembly = Assembly.Load(assemblyName);
                List<Type> ts = assembly.GetTypes().ToList();
                foreach (var item in ts.Where(s => !s.IsInterface))
                {
                    var interfaceType = item.GetInterfaces();
                    if (item.IsGenericType)
                        continue;

                    services.AddTransient(item);
                }
            }
        }

        public static void AddTransientIDCommon(string assemblyName)
        {
            if (!String.IsNullOrEmpty(assemblyName))
            {
                Assembly assembly = Assembly.Load(assemblyName);
                List<Type> ts = assembly.GetTypes().ToList();
                var result = new Dictionary<Type, Type[]>();
                foreach (var item in ts.Where(s => !s.IsInterface))
                {
                    var interfaceType = item.GetInterfaces();
                    if (item.IsGenericType)
                        continue;
                    if (item.Name != "ServerDICommon")
                        result.Add(item, interfaceType);
                }
            }

        }
    }

}
