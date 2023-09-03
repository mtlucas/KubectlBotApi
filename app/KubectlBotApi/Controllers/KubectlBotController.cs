using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static SimpleExec.Command;
using KubectlBotApi.Api.WebHost.Attributes;

namespace KubectlBotApi.Controllers
{
    [ApiKey]
    [ApiController]
    [Route("api/[controller]")]
    public class KubectlBotController : ControllerBase
    {
        /// <summary>
        /// KUBECTL will be executed with querystring parameters
        /// </summary>
        /// <param name="KubectlCMDRequest"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> KubectlCMD([FromQuery] string KubectlCMDRequest)
        {
            // Psuedo code:
            // 1. Execute KUBECTL with querystring data, parsing and limited data validation
            // 2. Forbid deleting nodes
            // 3. Return status of command

            Log.Information($"GET query data: {KubectlCMDRequest}");
            //////////////////////////
            /// Execute KubectlCMD ///
            //////////////////////////
            Log.Information($"Executing kubectl CMD...");
            Log.Information($"QueryString: {KubectlCMDRequest}");
            if (KubectlCMDRequest.ToLower().Contains("delete"))
            {
                Log.Warning("WARNING: Forbidden to delete");
                return StatusCode(501, "Forbidden to delete");
            }
            string[] QueryString = KubectlCMDRequest.Split(' ');
            int exitCode = 0;
            var (lookupResult, lookupError) = await ReadAsync("/app/kubectl",
                QueryString,
                handleExitCode: code => (exitCode = code) < 2);
            if (exitCode != 0)
            {
                Log.Error($"kubectl RESULT: {lookupResult}");
                Log.Error($"kubectl ERROR: {lookupError}");
                return StatusCode(500, $"RESULT returned from: \"kubectl {KubectlCMDRequest}\":\n{lookupResult}\nERROR: {lookupError}\n");
            }
            if (String.IsNullOrEmpty(lookupResult))
            {
                Log.Error("kubectl returned null or empty string.");
                return NotFound("kubectl returned null or empty string.");
            }
            Log.Information($"kubectl RESULT: {lookupResult}");
            return Ok(lookupResult);
        }
    }
}
