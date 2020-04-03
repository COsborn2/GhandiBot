using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GhandiBot.Mixins
{
    public static class IServiceProviderExtensions
    {
        public static object CreateInstance(this IServiceProvider provider, Type type)
        {
            var constructorInfo = type.GetConstructors();
            if (constructorInfo.Length > 1)
            {
                throw new ArgumentException("Type must only have 1 parameter");
            }
            var constructor = constructorInfo.FirstOrDefault();
            
            List<object> services = new List<object>();
            var parameters = constructor.GetParameters();
            foreach (ParameterInfo parameter in parameters)
            {
                var curType = parameter.ParameterType;
                var service = provider.GetService(curType);
                if (service is null)
                {
                    throw new Exception($"Could not find service of type '{curType}'");
                }
                
                services.Add(provider.GetService(curType));
            }
            return Activator.CreateInstance(type, services.ToArray());
        }
    }
}