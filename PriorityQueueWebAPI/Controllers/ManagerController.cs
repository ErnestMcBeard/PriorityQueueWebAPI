using PriorityQueueWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;

namespace PriorityQueueWebAPI.Controllers
{
    public class ManagerController : ODataController
    {
        ManagerContext db = new ManagerContext();

        private bool ManagerExists(Guid key)
        {
            return db.Managers.Any(p => p.Id == key);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Manager> Get()
        {
            return db.Managers;
        }

        [EnableQuery]
        public SingleResult<Manager> Get([FromODataUri] Guid key)
        {
            IQueryable<Manager> result = db.Managers.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Manager manager)
        {
            manager.Id = Guid.NewGuid();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Managers.Add(manager);
            await db.SaveChangesAsync();
            return Created(manager);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<Manager> manager)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await db.Managers.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }

            manager.Patch(entity);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManagerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(entity);
        }

        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Manager update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.Id)
            {
                return BadRequest();
            }
            db.Entry(update).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManagerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(update);
        }

        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            var manager = await db.Managers.FindAsync(key);

            if (manager == null)
            {
                return NotFound();
            }

            db.Managers.Remove(manager);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}