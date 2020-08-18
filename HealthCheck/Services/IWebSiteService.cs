using HealthCheck.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthCheck.Services
{
    public interface IWebSiteService
    {
        Task<IList<WebSiteDto>> GetWebSitesListAsync();
        Task<SiteDto> GetWebSiteAsync(string id);
        Task<bool> SetWebSiteStatus(string id, Status status);
        Task<bool> SetAppPoolStatus(string id, Status status);
        void SetServerAddress(string address, string token);
    }
}
