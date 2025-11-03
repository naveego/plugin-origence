using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Naveego.Sdk.Plugins;

public class Discover
{
    private static readonly string[] AllowedSchemaNames = new string[]
    {
        "CUDLVehicleLoanApplication"
    };

    private static RepeatedField<Property> GetBaseProperties()
    {
        return new RepeatedField<Property>
        {
            new Property
            {
                Id = "ID",
                Name = "ID",
                IsKey = true,
                IsNullable = false,
                Type = PropertyType.String,
            },
            new Property
            {
                Id = "JSONObject",
                Name = "JSONObject",
                IsKey = false,
                IsNullable = false,
                Type = PropertyType.Blob,
            }
        };
    }

    public static async IAsyncEnumerable<Schema> GetAllSchemas(IPluginClient client, int sampleSize = 0)
    {
        var schemas = new List<Schema>();
        foreach (var schemaName in AllowedSchemaNames)
        {
            schemas.Add(new Schema
            {
                Id = schemaName,
                Name = schemaName,
                Description = $"Schema for {schemaName}",
                Properties = { GetBaseProperties() }
            });
        }

        foreach (var schema in schemas)
        {
            if (sampleSize > 0)
            {
                var records = Read.ReadRecords(client, schema, sampleSize);
                schema.Sample.AddRange(await records.ToListAsync());
            }
            yield return schema;
        }

    }

    public static async IAsyncEnumerable<Schema> GetRefreshSchemas(IPluginClient client,RepeatedField<Schema> refreshSchemas, int sampleSize=0)
    {
        foreach (var schema in refreshSchemas)
        {
            if (!AllowedSchemaNames.Contains(schema.Name))
            {
                continue;
            }

            if (sampleSize > 0)
            {
                var records = Read.ReadRecords(client, schema, sampleSize);
                schema.Sample.AddRange(await records.ToListAsync());
            }

            schema.Properties.Clear();
            schema.Properties.AddRange(GetBaseProperties());

            yield return schema;
        }
    }
}