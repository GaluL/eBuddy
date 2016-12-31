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
    [Authorize]
    public class EBuddyUserController : TableController<eBuddyUser1>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            eBuddyContext context = new eBuddyContext();
            DomainManager = new EntityDomainManager<eBuddyUser1>(context, Request);
        }

        // GET tables/TodoItem
        public IQueryable<eBuddyUser1> GetAllTodoItems()
        {
            return Query();
        }

        // GET tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<eBuddyUser1> GetTodoItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<eBuddyUser1> PatchTodoItem(string id, Delta<eBuddyUser1> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/TodoItem
        public async Task<IHttpActionResult> PostTodoItem(eBuddyUser1 item)
        {
            eBuddyUser1 current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteeBuddyUser1Item(string id)
        {
            return DeleteAsync(id);
        }
    }
}