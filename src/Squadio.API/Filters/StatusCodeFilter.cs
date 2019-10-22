using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Squadio.Common.Models.Responses;

namespace Squadio.API.Filters
{
    public class StatusCodeFilter: IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            try
            {
                if (context.Result is ObjectResult objResult)
                {
                    if (objResult.Value is Response response)
                    {
                        context.HttpContext.Response.StatusCode = (int) response.HttpStatusCode;
                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            
        }
    }
}