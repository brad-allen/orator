using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MemoryCache;
using MemoryCache.Tests.TestObjects;

namespace Orator
{
	
    public class MvcApplication : System.Web.HttpApplication
    {
		public static OverrideCache<TestKey, TestValue> TestCache = new OverrideCache<TestKey, TestValue>();
		
		public class OverrideCache<Tkey, Tvalue> : Cache<Tkey, Tvalue>
		{
			public override void CustomReplacementAlgorithm(CacheSet<Tvalue> cacheSet)
			{
				var newestAdded = cacheSet.Blocks.OrderBy(m => m.Key).FirstOrDefault();
				cacheSet.Blocks.Remove(newestAdded.Key);
			}
		}

		protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
