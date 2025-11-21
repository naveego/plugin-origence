using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Naveego.Sdk.Logging;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;

public class Read
{
    public static async IAsyncEnumerable<Record> ReadRecords(
        IPluginClient client,
        Schema schema,
        int sampleSize = 0)
    {
        var elementTag = schema.Id;
        var xDocuments = client.GetDocuments();

        await foreach (var xDoc in xDocuments)
        {
            var count = 0;

            var ns = xDoc?.Root?.Name.Namespace ?? "";
            xDoc?.Descendants(ns + "Raw")?.Remove();
            var elementsWithNs = xDoc?.Descendants(ns + elementTag)?.ToList() ?? new List<XElement>();

            foreach (var element in elementsWithNs)
            {
                if (sampleSize > 0 && count >= sampleSize)
                {
                    yield break;
                }
                count++;
                
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
}