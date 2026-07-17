using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SentinelApp.Application.Core
{
    public abstract class ControllerBaseResponse : ControllerBase
    {
        public IActionResult GenerateBaseResponse<T>(ServiceResponse<T> serviceResponse)
        {
            if (serviceResponse.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound(serviceResponse);
            if (serviceResponse.HttpStatusCode == System.Net.HttpStatusCode.BadRequest)
                return BadRequest(serviceResponse);         
            return new ObjectResult(serviceResponse) { 
                StatusCode = serviceResponse.HttpStatusCodeNumber
            };
        }
    }
}
