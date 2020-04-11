using System;
using System.ComponentModel;
using System.ServiceProcess;

namespace services.Utilities
{
    public class ServiceCreator
    {
        public string MachineName { get; set; }

        public string PathName { get; set; }

        public string ServiceName { get; set; }

        public string DisplayName { get; set; }

        public ServiceStartMode StartType { get; set; } = ServiceStartMode.Automatic;

        public bool DelayedAutoStart { get; set; } = true;

        public ServiceCreator(string pathName, string serviceName)
            : this(pathName, serviceName, serviceName, ".")
        {
        }

        public ServiceCreator(string pathName, string serviceName, string displayName)
            : this(pathName, serviceName, displayName, ".")
        {
        }

        public ServiceCreator(string pathName, string serviceName, string displayName, string machineName)
        {
            PathName = pathName;
            ServiceName = serviceName;
            DisplayName = displayName;
            MachineName = machineName;
        }

        public void CreateService()
        {
            using (var scmHandle = OpenSCManager(NativeMethods.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE))
            {
                using (var serviceHandle = CreateService(scmHandle))
                {
                    if (serviceHandle.IsInvalid)
                    {
                        throw new Win32Exception();
                    }

                    if (!DelayedAutoStart)
                    {
                        var result = NativeMethods.StartService(serviceHandle, 0, null);
                        if (result == 0)
                        {
                            throw new Win32Exception();
                        }
                    }
                }
            }
        }

        public void RemoveService()
        {
            using (var scmHandle = OpenSCManager(NativeMethods.SCM_ACCESS.SC_MANAGER_ALL_ACCESS))
            {
                using (var serviceHandle = NativeMethods.OpenService(scmHandle, ServiceName, NativeMethods.SERVICE_ACCESS.DELETE))
                {
                    if (serviceHandle.IsInvalid)
                    {
                        throw new Win32Exception();
                    }

                    var result = NativeMethods.DeleteService(serviceHandle);
                    if (result == 0)
                    {
                        throw new Win32Exception();
                    }
                }
            }
        }

        private ServiceControlHandle OpenSCManager(NativeMethods.SCM_ACCESS access)
        {
            var result = NativeMethods.OpenSCManager(MachineName, null, access);
            
            if (result.IsInvalid)
            {
                throw new Win32Exception();
            }

            return result;
        }
        
        private ServiceControlHandle CreateService(ServiceControlHandle managerHandle)
        {
            var startType = GetStartType();

            return NativeMethods.CreateService(managerHandle, ServiceName, DisplayName,
                NativeMethods.SERVICE_ACCESS.SERVICE_ALL_ACCESS,
                NativeMethods.SERVICE_TYPES.SERVICE_WIN32_OWN_PROCESS,
                startType,
                NativeMethods.SERVICE_ERROR_CONTROL.SERVICE_ERROR_NORMAL,
                PathName,
                null, IntPtr.Zero, null, null, null);
        }

        private NativeMethods.SERVICE_START_TYPES GetStartType()
        {
            switch (StartType)
            {
                //???
                //case ServiceStartMode.Manual:
                //    return SERVICE_START_TYPES.;

                case ServiceStartMode.Automatic:
                    if (DelayedAutoStart)
                    {
                        return NativeMethods.SERVICE_START_TYPES.SERVICE_DEMAND_START;
                    }

                    return NativeMethods.SERVICE_START_TYPES.SERVICE_AUTO_START;

                case ServiceStartMode.Disabled:
                    return NativeMethods.SERVICE_START_TYPES.SERVICE_DISABLED;

                case ServiceStartMode.Boot:
                    return NativeMethods.SERVICE_START_TYPES.SERVICE_BOOT_START;

                case ServiceStartMode.System:
                    return NativeMethods.SERVICE_START_TYPES.SERVICE_SYSTEM_START;

                default:
                    throw new NotImplementedException("Missing StartType.");
            }
        }

        //public enum SERVICE_START_TYPES : int
        //{
        //    /// <summary>
        //    /// A service started automatically by the service control manager during system startup. For more information, see Automatically Starting Services.
        //    /// </summary>
        //    SERVICE_AUTO_START = 0x00000002,

        //    /// <summary>
        //    /// A device driver started by the system loader. This value is valid only for driver services.
        //    /// </summary>
        //    SERVICE_BOOT_START = 0x00000000,

        //    /// <summary>
        //    /// A service started by the service control manager when a process calls the StartService function. For more information, see Starting Services on Demand.
        //    /// </summary>
        //    SERVICE_DEMAND_START = 0x00000003,

        //    /// <summary>
        //    /// A service that cannot be started. Attempts to start the service result in the error code ERROR_SERVICE_DISABLED.
        //    /// </summary>
        //    SERVICE_DISABLED = 0x00000004,

        //    /// <summary>
        //    /// A device driver started by the IoInitSystem function. This value is valid only for driver services.
        //    /// </summary>
        //    SERVICE_SYSTEM_START = 0x00000001

        //}
    }
}
