using System.Collections.Generic;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;

public class Read
{
    public static async IAsyncEnumerable<Record> ReadRecords(
        IPluginClient client,
        Schema schema,
        int sampleSize = 0)
    {
        var elementTag = schema.Name;
        var dataElements = client.GetData(elementTag, sampleSize);

        await foreach (var element in dataElements)
        {
            var elementId = element.Attribute("ID")?.Value ?? "";
            var jsonObject = JsonConvert.SerializeXNode(element, Formatting.Indented);

            var recordMap = new Dictionary<string, object>
            {
                ["ID"] = elementId,
                ["JSONObject"] = jsonObject
            };
            
            var record = new Record
            {
                Action = Record.Types.Action.Upsert,
                DataJson = JsonConvert.SerializeObject(recordMap)
            };

            yield return record;
        }
    }
}