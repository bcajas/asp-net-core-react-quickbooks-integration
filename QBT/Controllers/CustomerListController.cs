using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Intuit.Ipp.Core;
using Intuit.Ipp.Exception;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QBT.Models;

namespace QBT.Controllers
{
    [Authorize]
    [ApiController]
    [Route("customer")]

    
    public class CustomerListController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IList<Models.QuickBooksOnline.Customer> Customers { get; private set; }

        public CustomerListController(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task Get(string realmId)
        {
         

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var accessToken = await _userManager.GetAuthenticationTokenAsync(user, "QuickBooks", "access_token");
            var baseUrl = $"https://sandbox-quickbooks.api.intuit.com/v3/company/{realmId}/query?query=select * from customer";
            HttpWebRequest qboApiRequest = (HttpWebRequest)WebRequest.Create(baseUrl);
            qboApiRequest.Method = "GET";
            qboApiRequest.Headers["Authorization"] = string.Format("Bearer {0}", accessToken);
            qboApiRequest.ContentType = "application/json;charset=UTF-8";
            qboApiRequest.Accept = "application/json";
            try
            {
                // get the response
                var response = await qboApiRequest.GetResponseAsync();
                HttpWebResponse qboApiResponse = (HttpWebResponse)response;
                //read qbo api response
                using (var qboApiReader = new StreamReader(qboApiResponse.GetResponseStream()))
                {
                    var result = qboApiReader.ReadToEnd();
                    var rootObj = JsonConvert.DeserializeObject<Models.QuickBooksOnline.RootObject>(result);
                    Customers = rootObj.QueryResponse.Customer.ToList();
                             
                }
            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("401"))
                {
                }
                else
                {
                }
            }
        }
    }
}