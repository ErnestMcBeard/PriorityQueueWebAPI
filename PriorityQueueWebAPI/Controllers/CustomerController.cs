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
    public class CustomerController : ODataController
    {
        CustomerContext db = new CustomerContext();

        private bool CustomerExists(Guid key)
        {
            return db.Customers.Any(p => p.Id == key);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Customer> Get()
        {
            return db.Customers;
        }

        [EnableQuery]
        public SingleResult<Customer> Get([FromODataUri] Guid key)
        {
            IQueryable<Customer> result = db.Customers.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Customer customer)
        {
            customer.Id = Guid.NewGuid();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Customers.Add(customer);
            await db.SaveChangesAsync();
            return Created(customer);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<Customer> customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await db.Customers.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }

            customer.Patch(entity);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(key))
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

        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Customer update)
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
                if (!CustomerExists(key))
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
            var customer = await db.Customers.FindAsync(key);

            if (customer == null)
            {
                return NotFound();
            }

            db.Customers.Remove(customer);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}