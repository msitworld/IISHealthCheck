
namespace HealthCheck.Models
{
    public class SiteDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhysicalPath { get; set; }
        public Status Status { get; set; }
        public ApplicationPool ApplicationPool { get; set; }

    }

    public class ApplicationPool
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }

    }
}
