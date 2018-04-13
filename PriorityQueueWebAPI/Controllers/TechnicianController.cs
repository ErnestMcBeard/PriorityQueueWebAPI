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
    public class TechnicianController : ODataController
    {
        TechnicianContext db = new TechnicianContext();

        private bool TechnicianExists(Guid key)
        {
            return db.Technicians.Any(p => p.Id == key);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Technician> Get()
        {
            return db.Technicians;
        }

        [EnableQuery]
        public SingleResult<Technician> Get([FromODataUri] Guid key)
        {
            IQueryable<Technician> result = db.Technicians.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Technician technician)
        {
            technician.Id = Guid.NewGuid();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Technicians.Add(technician);
            await db.SaveChangesAsync();
            return Created(technician);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<Technician> technician)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await db.Technicians.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }

            technician.Patch(entity);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TechnicianExists(key))
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

        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Technician update)
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
                if (!TechnicianExists(key))
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
            var technician = await db.Technicians.FindAsync(key);

            if (technician == null)
            {
                return NotFound();
            }

            db.Technicians.Remove(technician);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}