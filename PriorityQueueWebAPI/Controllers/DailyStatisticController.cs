using PriorityQueueWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;

namespace PriorityQueueWebAPI.Controllers
{
    public class DailyStatisticController : ODataController
    {
        DailyStatisticContext db = new DailyStatisticContext();

        private bool DailyStatisticExists(Guid key)
        {
            return db.DailyStatistics.Any(p => p.Id == key);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<DailyStatistic> Get()
        {
            return db.DailyStatistics;
        }
        
        [EnableQuery]
        public SingleResult<DailyStatistic> Get([FromODataUri] Guid key)
        {
            IQueryable<DailyStatistic> result = db.DailyStatistics.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(DailyStatistic dailyStatistic)
        {
            dailyStatistic.Id = Guid.NewGuid();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.DailyStatistics.Add(dailyStatistic);
            await db.SaveChangesAsync();
            return Created(dailyStatistic);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<DailyStatistic> dailyStatistic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await db.DailyStatistics.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }

            dailyStatistic.Patch(entity);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DailyStatisticExists(key))
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

        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, DailyStatistic update)
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
                if (!DailyStatisticExists(key))
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
            var dailyStatistic = await db.DailyStatistics.FindAsync(key);

            if (dailyStatistic == null)
            {
                return NotFound();
            }

            db.DailyStatistics.Remove(dailyStatistic);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}