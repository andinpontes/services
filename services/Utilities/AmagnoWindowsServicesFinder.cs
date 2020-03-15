using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace services.Utilities
{
    public static class AmagnoWindowsServicesFinder
    {
        public static IEnumerable<string> FindServices(string searchPath)
        {
            if (string.IsNullOrWhiteSpace(searchPath))
            {
                throw new ArgumentNullException(nameof(searchPath));
            }

            return Directory
                .EnumerateFiles(searchPath, "*.exe", SearchOption.AllDirectories)
                .Where(IsAmagnoServiceFile);
        }

        public static bool IsAmagnoServiceFile(string filename)
        {
            var assembly = LoadAssembly(filename);
            if (assembly == null)
            {
                return false;
            }

            return GetTypes(assembly)
                .Any(HasAmagnoServiceAttribute);
        }

        private static IEnumerable<Type> GetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (Exception exception)
            {
                return new Type[] { };
            }
        }

        private static bool HasAmagnoServiceAttribute(Type type)
        {
            try
            {
                var attributes = type.GetTypeInfo().GetCustomAttributes();
                if (attributes.Any(IsServiceAttribute))
                {
                    return true;
                }

                return false;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        private static bool IsServiceAttribute(Attribute attribute)
        {
            var type = attribute.GetType();
            return type.FullName.Contains("Amagno.Service.Shared.Service.AmagnoWindowsServiceAttribute");
        }

        private static Assembly LoadAssembly(string filename)
        {
            try
            {
                return Assembly.LoadFrom(filename);
            }
            catch (Exception exception)
            {
                return null;
            }
        }
    }
}
