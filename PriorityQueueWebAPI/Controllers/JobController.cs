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
    public class JobController : ODataController
    {
        JobContext db = new JobContext();

        private bool JobExists(Guid key)
        {
            return db.Jobs.Any(p => p.Id == key);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Job> Get()
        {
            return db.Jobs;
        }

        [EnableQuery]
        public SingleResult<Job> Get([FromODataUri] Guid key)
        {
            IQueryable<Job> result = db.Jobs.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Job job)
        {
            job.Id = Guid.NewGuid();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Jobs.Add(job);
            await db.SaveChangesAsync();
            return Created(job);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<Job> job)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await db.Jobs.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }

            job.Patch(entity);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(key))
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

        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Job update)
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
                if (!JobExists(key))
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

        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            var job = await db.Jobs.FindAsync(key);

            if (job == null)
            {
                return NotFound();
            }

            db.Jobs.Remove(job);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}