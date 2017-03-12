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
    public class RunItemController : TableController<RunItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            eBuddyContext context = new eBuddyContext();
            DomainManager = new EntityDomainManager<RunItem>(context, Request);
        }

        // GET tables/RunItem
        public IQueryable<RunItem> GetAllRunItem()
        {
            return Query(); 
        }

        // GET tables/RunItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<RunItem> GetRunItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/RunItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<RunItem> PatchRunItem(string id, Delta<RunItem> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/RunItem
        public async Task<IHttpActionResult> PostRunItem(RunItem item)
        {
            RunItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/RunItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteRunItem(string id)
        {
             return DeleteAsync(id);
        }
    }
}
