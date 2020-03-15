using services.Models;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Management;
using System.ServiceProcess;

namespace services
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (IsListCommand(args))
            {
                WriteServicesList(args);
            }
            else if (IsInstallCommand(args))
            {
                InstallServices(args);
            }

            Console.ReadKey();

            //services list [searchpath]:
            //  Lists all AmagnoServices and their status and path found in searchpath and subfolders of searchpath.
            //  If no searchpath is specified, lists only already installed services.
            //
            //  Example:
            //      services list ../server
            //
            //  AmagnoLocalCleanupService   Running         c:/foo/AmagnoLocalCleanupService.exe
            //  AmagnoStreamService         Stopped         c:/foo/AmagnoStreamService.exe
            //  AmagnoSearchIndexService    Not installed   c:/foo/AmagnoSearchIndexService.exe

            //services install <searchpath>:
            //  Installs all AmagnoServices found in searchpath and subfolders of searchpath.
            //  AmagnoServices are exe files that contain a Program class with the attribute [AmagnoWindowsService].
            //
            //  Example:
            //      services install ../server

            //services uninstall <searchpath>:
            //  Uninstalls all AmagnoServices found in searchpath and subfolders of searchpath.
            //
            //  Example:
            //      services uninstall ../server

            //services start [searchpath]:
            //  Starts all AmagnoServices found in searchpath and subfolders of searchpath that are currently not running.
            //  If no searchpath is specified, starts all already installed services which are not running and name begins with 'Amagno'.
            //
            //  Example:
            //      services start
            //      services start ../server

            //services stop [searchpath]:
            //  Stops all AmagnoServices found in searchpath and subfolders of searchpath that are currently running.
            //  If no searchpath is specified, stops all already installed services which are running and name begins with 'Amagno'.
            //
            //  Examples:
            //      services stop
            //      services stop ../server


            //var b = System.Environment.UserInteractive;
        }

        private static bool IsListCommand(string[] args)
        {
            return args.Length > 0 && args[0].ToLowerInvariant().Equals("list");
        }

        private static void WriteServicesList(string[] args)
        {
            var serviceInfos = GetAllAmagnoServices(args);

            foreach (var info in serviceInfos)
            {
                Console.WriteLine($"{info.Name, -25} {info.Status, -10} {info.PathName}");
            }
        }

        private static bool IsInstallCommand(string[] args)
        {
            return args.Length > 0 && args[0].ToLowerInvariant().Equals("install");
        }

        private static void InstallServices(string[] args)
        {
            var serviceInfos = GetAllAmagnoServices(args);

            foreach (var info in serviceInfos)
            {
                Console.Write($"Installing {info.Name}...");

                try
                {
                    if (info.IsInstalled)
                    {
                        Console.WriteLine(" : Skipped (already installed)");
                    }
                    else
                    {
                        //TODO: InstallService(info.PathName);
                        Console.WriteLine(" : Success");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(" : Failed");
                }
            }
        }


        private static ServiceInfo[] GetAllAmagnoServices(string[] args)
        {
            var serviceInfos = GetInstalledServices();

            //TODO:
            // Select the correct services and add some infos for not yet installed services.

            return serviceInfos;
        }


        private static string[] FindAmagnoServices(string[] args)
        {
            //TODO:
            //var serviceInfos = GetAllServiceInfos();

            //TODO:
            return new string[] { };
        }

        private static void InstallService(string filename)
        {
            ManagedInstallerClass.InstallHelper(new string[] { filename });
        }

        private static void UninstallService(string filename)
        {
            ManagedInstallerClass.InstallHelper(new string[] { "/u", filename });
        }

        private static ServiceInfo[] GetInstalledServices()
        {
            var result = new List<ServiceInfo>();

            var wmiInfos = FindWmiServiceInfos();

            var services = ServiceController.GetServices();
            foreach (var service in services)
            {
                var wmiInfo = wmiInfos.FirstOrDefault(e => e["Name"] as string == service.ServiceName);
                var pathName = NormalizePath(wmiInfo["PathName"] as string);
                var description = wmiInfo["Description"] as string;
                var processId = Convert.ToUInt32(wmiInfo["ProcessId"]);

                var serviceInfo = new ServiceInfo
                {
                    Name = service.ServiceName,
                    DisplayName = service.DisplayName,
                    Status = service.Status,
                    PathName = pathName,
                    Description = description,
                    StartType = service.StartType,
                    ServiceType = service.ServiceType,
                    ProcessId = processId,
                    IsInstalled = true
                };

                result.Add(serviceInfo);
            }

            return result
                .OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static ManagementObject[] FindWmiServiceInfos()
        {
            var result = new List<ManagementObject>();

            var query = $"SELECT * FROM Win32_Service";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection collection = searcher.Get();
            
            foreach (ManagementObject obj in collection)
            {
                result.Add(obj);
            }

            return result.ToArray();
        }

        private static string NormalizePath(string path)
        {
            if (path == null)
            {
                return string.Empty;
            }

            string result = path.Trim();
            //TODO:

            return result;
        }
    }
}
