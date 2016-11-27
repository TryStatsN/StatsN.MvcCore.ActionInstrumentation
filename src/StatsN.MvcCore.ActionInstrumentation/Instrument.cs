using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StatsN.MvcCore.ActionInstrumentation
{
    public class Instrument : ActionFilterAttribute
    {
        private readonly string metricName;
        private readonly string dictionaryKey;

        public Instrument(string metricName)
        {
            this.metricName = metricName;
            this.dictionaryKey = "Instrument." + metricName; 
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var istatsd = context.HttpContext.RequestServices.GetService(typeof(IStatsd)) as IStatsd;
            if(istatsd == null) return;
            var stopwatch = new Stopwatch();
            context.HttpContext.Items[dictionaryKey] = stopwatch;
            stopwatch.Start();
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                var istatsd = context.HttpContext.RequestServices.GetService(typeof(IStatsd)) as IStatsd;
                if(istatsd == null) return;
                var stopwatch = context.HttpContext.Items[dictionaryKey] as Stopwatch;
                if(stopwatch == null) return;
                stopwatch.Stop();
                istatsd.TimingAsync(metricName, stopwatch.ElapsedMilliseconds);
            }
            catch(Exception e)
            {
                //this should not kill the app
                Trace.Fail("Error instrumenting action", e.Message);
            }
        }
    }
}
