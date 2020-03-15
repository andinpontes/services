using System.ServiceProcess;

namespace services.Models
{
    public class ServiceInfo
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ServiceControllerStatus Status { get; set; }
        public string PathName { get; set; }
        public string Description { get; set; }
        public ServiceStartMode StartType { get; set; }
        public ServiceType ServiceType { get; set; }
        public uint ProcessId { get; set; }
        public bool IsInstalled { get; set; }
    }
}
