using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SentinelApp.Application.DTO;
using SentinelApp.Application.Interfaces;

namespace SentinelApp.API.Middleware;

public class EncryptionFilter : ActionFilterAttribute
{
	private readonly IConfiguration _config;
	bool blnEnableResponseEncryption = false;
	private readonly IEncryptionService _encryptionService;
    private static readonly List<string> SkipEncryptionPaths = new List<string>
	{
		"/api/citypay/postback",
        "/api/auth/auto-authenticate"
    };

    public EncryptionFilter(IConfiguration config, IEncryptionService encryptionService)
	{
		_config = config;
		blnEnableResponseEncryption = _config.GetValue<string>("Filters:EnableApiEncryptionResponse") == "Y";
		_encryptionService = encryptionService;
	}

    private bool ShouldSkipEncryption(ActionContext context)
    {
        var path = context.HttpContext.Request.Path.Value?.ToLower();

        if (string.IsNullOrEmpty(path))
            return false;

        return SkipEncryptionPaths.Any(p => path.Contains(p));
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		if (!ShouldSkipEncryption(context))
		{
			foreach (var argument in context.ActionArguments)
			{
				if (argument.Value is Ref_RequestData rq && (context.HttpContext.Request.Method == HttpMethods.Post || context.HttpContext.Request.Method == HttpMethods.Delete || context.HttpContext.Request.Method == HttpMethods.Put))
				{
					rq.SetData(_encryptionService.DecryptStringFromBytes_Aes(rq.RequestData));
					context.ActionArguments[argument.Key] = rq;
				}
			}
		}
		// Continue to the next middleware/action
		// Proceed with the action execution and get the result
		var executedContext = await next();

		// Encrypt the response data if it is an ObjectResult with a string value
		if (!ShouldSkipEncryption(context) && executedContext.Result is ObjectResult objectResult && blnEnableResponseEncryption)
		{
			var jsonResponse = JsonConvert.SerializeObject(objectResult.Value);
			var encryptedResponse = _encryptionService.EncryptStringToBytes_Aes(jsonResponse);
			objectResult.Value = Convert.ToBase64String(encryptedResponse);
		}
	}
	public override void OnActionExecuting(ActionExecutingContext context)
	{
		// This method is useful for synchronous interception of arguments
		if (!ShouldSkipEncryption(context))
		{
			foreach (var argument in context.ActionArguments)
			{
				if (argument.Value is Ref_RequestData rq)
				{
					rq.SetData(_encryptionService.DecryptStringFromBytes_Aes(rq.RequestData));
					var requestData = JsonConvert.DeserializeObject<Ref_RequestData>(rq.GetData());
					context.ActionArguments[argument.Key] = requestData;
				}
			}
		}
		base.OnActionExecuting(context);
	}

	public override void OnActionExecuted(ActionExecutedContext context)
	{
		// Check if the response is an ObjectResult (typical for API responses)
		if (!ShouldSkipEncryption(context) && context.Result is ObjectResult objectResult && objectResult.Value is string responseValue)
		{
			// Encrypt the response data
			var jsonResponse = JsonConvert.SerializeObject(objectResult.Value);
			var encryptedResponse = _encryptionService.EncryptStringToBytes_Aes(jsonResponse);
			objectResult.Value = Convert.ToBase64String(encryptedResponse);
		}
		base.OnActionExecuted(context);
	}
}
