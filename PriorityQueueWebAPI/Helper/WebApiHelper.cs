using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System.Text;

namespace MODELPriorityQueue.Models
{
    public static class WebApiHelper
    {
        private static string serverDomain = "http://localhost:51578";
        private static readonly HttpClient client = new HttpClient();
        
        private static string BaseUrl<T>()
        {
            return string.Format("{0}/{1}", serverDomain, typeof(T).Name);
        }

        /// <summary>
        /// Selects all of the object from DbSet<T>
        /// </summary>
        /// <returns>Returns a list of T object, or null if something fucked up</returns>
        public static async Task<List<T>> Get<T>()
        {
            string response = await client.GetStringAsync(BaseUrl<T>());
            if (!string.IsNullOrEmpty(response))
            {
                return JsonConvert.DeserializeObject<ODataResponse<T>>(response).Value;
            }
            return null;
        }

        /// <summary>
        /// Selects an object from DbSet<T> with the given Id
        /// </summary>
        /// <param name="id">The Guid of the desired object</param>
        /// <returns>An object from DbSet<T> with the given id, or null if it does not exist</returns>
        public static async Task<T> Get<T>(Guid id)
        {
            string response = await client.GetStringAsync(string.Format("{0}({1})", BaseUrl<T>(), id));
            if (!string.IsNullOrEmpty(response))
            {
                return JsonConvert.DeserializeObject<ODataResponse<T>>(response).Value.FirstOrDefault();
            }
            return default(T);
        }

        /// <summary>
        /// Selects an object from DbSet<T> with the given Id
        /// </summary>
        /// <param name="query">The filter, orderby, etc. See this method definition for examples</param>
        /// <returns>An object from DbSet<T> with the given id, or null if it does not exist</returns>
        public static async Task<T> Get<T>(string query)
        {
            //Basic Query Examples
            //Filter:   $filter=Firstname eq 'John' and TimesServiced eq 2
            //Orderby:  $orderby=Priority desc
            //Combo:    $filter=Firstname eq 'John'&$orderby=Priority desc

            //For more query options try google. There's a shit-ton

            string response = await client.GetStringAsync(string.Format("{0}?{1}", BaseUrl<T>(), query));
            if (!string.IsNullOrEmpty(response))
            {
                return JsonConvert.DeserializeObject<ODataResponse<T>>(response).Value.FirstOrDefault();
            }
            return default(T);
        }

        /// <summary>
        /// Inserts this object into the database
        /// </summary>
        /// <returns>The object in its state after inserting it</returns>
        public static async Task<T> Post<T>(T model)
        {
            string json = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(BaseUrl<T>(), content);

            if (response.IsSuccessStatusCode)
            {
                string message = await response.Content.ReadAsStringAsync();
                T updatedObject = JsonConvert.DeserializeObject<T>(message);
                return updatedObject;
            }

            return default(T);
        }

        /// <summary>
        /// Updates this instance of the object in the database
        /// </summary>
        /// <returns>True if it succeeded, false otherwise</returns>
        public static async Task<bool> Update<T>(T model, Guid id)
        {
            var method = new HttpMethod("PATCH");
            var json = JsonConvert.SerializeObject(model);

            var request = new HttpRequestMessage(method, string.Format("{0}({1})", BaseUrl<T>(), id))
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };

            HttpResponseMessage response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Deletes the object from DbSet<T> with the same Id as this object
        /// </summary>
        /// <returns>True if it succeeded, false otherwise</returns>
        public static async Task<bool> Delete<T>(Guid id)
        {
            HttpResponseMessage response = await client.DeleteAsync(string.Format("{0}({1})", BaseUrl<T>(), id));
            return response.IsSuccessStatusCode;
        }
    }
}
