using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using eBuddyService.DataObjects;
using eBuddyService.Models;

namespace eBuddyService.Controllers
{
    public class ScheduledRunItemController : TableController<ScheduledRunItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            eBuddyContext context = new eBuddyContext();
            DomainManager = new EntityDomainManager<ScheduledRunItem>(context, Request);
        }

        // GET tables/ScheduledRunItem
        public IQueryable<ScheduledRunItem> GetAllScheduledRunItem()
        {
            return Query(); 
        }

        // GET tables/ScheduledRunItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<ScheduledRunItem> GetScheduledRunItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/ScheduledRunItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<ScheduledRunItem> PatchScheduledRunItem(string id, Delta<ScheduledRunItem> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/ScheduledRunItem
        public async Task<IHttpActionResult> PostScheduledRunItem(ScheduledRunItem item)
        {
            ScheduledRunItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/ScheduledRunItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteScheduledRunItem(string id)
        {
             return DeleteAsync(id);
        }
    }
}
