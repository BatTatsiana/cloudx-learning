using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using OrderItemsReserver.Models;

namespace OrderItemsReserver
{
    public class OrderItemsReserver
    {
        [FunctionName(nameof(OrderItemsReserver))]
        [StorageAccount("Blobstorage")]
        public async Task Run(
            [ServiceBusTrigger("orderreservation", Connection = "ServiceBusConnection")] RequestModel myQueueItem,
            [Blob("orders/{DateTime}.json", FileAccess.Write)] Stream content, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            await using var writer = new StreamWriter(content);
            await writer.WriteAsync(JsonConvert.SerializeObject(myQueueItem));
        }
    }
}
