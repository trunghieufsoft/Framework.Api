using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Common.Core.Models;

namespace WebAPI.Infrastructure.Middleware
{
    public class ResponseHandler
    {
        private readonly RequestDelegate _next;

        public ResponseHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.ToString().Contains("/api/"))
            {
                await _next(context);
                return;
            }

            Stream currentBody = context.Response.Body;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                await _next(context);

                //reset the body
                context.Response.Body = currentBody;
                memoryStream.Seek(0, SeekOrigin.Begin);

                string readToEnd = new StreamReader(memoryStream).ReadToEnd();

                if (context.Response.StatusCode != (int)HttpStatusCode.OK)
                {
                    ResponseModel errorModel = JsonConvert.DeserializeObject<ResponseModel>(readToEnd);
                    string re = JsonConvert.SerializeObject(errorModel, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    await context.Response.WriteAsync(re);

                    return;
                }

                object data;
                try
                {
                    data = JsonConvert.DeserializeObject(readToEnd);
                }
                catch (JsonReaderException)
                {
                    data = readToEnd;
                }

                await context.Response.WriteAsync(JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
            }
        }
    }
}