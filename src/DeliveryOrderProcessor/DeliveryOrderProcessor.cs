using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Web.Http;
using DeliveryOrderProcessor.Requests;
using Microsoft.AspNetCore.Http;

namespace DeliveryOrderProcessor
{
    public static class DeliveryOrderProcessor
    {
        [FunctionName(nameof(DeliveryOrderProcessor))]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest request,
            [CosmosDB(
                databaseName: "EShop",
                containerName: "DeliveryOrders",
                Connection = "CosmosDBConnection")]
            out dynamic document,
            ILogger log)
        {
            document = null;
            try
            {
                var requestContent = request.ReadAsStringAsync().Result;
                log.Log(LogLevel.Information, requestContent);
                var deliveryOrderRequest =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<DeliveryOrderRequest>(requestContent);
                if (deliveryOrderRequest is null || deliveryOrderRequest.Items is null ||
                    !deliveryOrderRequest.Items.Any() || deliveryOrderRequest.Address is null)
                {
                    return new BadRequestObjectResult("Reserve order items request is invalid");
                }

                document = new
                {
                    id = deliveryOrderRequest.OrderId.ToString(),
                    items = deliveryOrderRequest.Items,
                    shippingAddress = deliveryOrderRequest.Address,
                    finalPrice = deliveryOrderRequest.Price
                };

                return new OkObjectResult("Delivery Information has been saved");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error occurred during function execution");
                return new InternalServerErrorResult();
            }
        }
    }
    
}
