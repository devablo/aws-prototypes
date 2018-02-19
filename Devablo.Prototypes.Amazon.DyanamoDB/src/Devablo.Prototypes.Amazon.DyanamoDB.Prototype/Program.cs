using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;

namespace Devablo.Prototypes.Amazon.DynamoDB.Prototype
{
    class Program
    {
        protected static string TableName = "Stations";
        protected static AmazonDynamoDBClient Client;

        static void Main(string[] args)
        {
            var clientConfig = new AmazonDynamoDBConfig();
            clientConfig.RegionEndpoint = RegionEndpoint.APSoutheast2;
            Client = new AmazonDynamoDBClient(clientConfig);

            try
            {
                var stationCatalog = Table.LoadTable(Client, TableName);
                CreateStation(stationCatalog);
                GetStationsByUid("0a4e988b-e776-44ef-9a20-fd5f271e3935");
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }

            Console.ReadLine();   
        }

        private static void CreateStationTable()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            string tableName = "ProductCatalog";

            var request = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                      AttributeName = "uid",
                      AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                      AttributeName = "uid",
                      KeyType = "HASH"  //Partition key
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 5
                }
            };

            var response = client.CreateTableAsync(request);
        }
        private static void CreateStation(Table table)
        {
            var station = new CreatStationCommand()
            {
                Name = "2Day FM",
                Code = "2dayfm",
                State = "NSW",
                City = "Sydney",
                Tags = new string[]
                {
                    "Music",
                    "Sydney"
                }
            };

            var stationDocument = new Document();
            stationDocument["uid"] = Guid.NewGuid();
            stationDocument["name"] = station.Name;
            stationDocument["code"] = station.Code;
            stationDocument["state"] = station.State;
            stationDocument["tags"] = station.Tags;
            stationDocument["metro"] = new DynamoDBBool(true);

            var result = table.PutItemAsync(stationDocument).Result;
        }
        private static void GetStationsByUid(string uid)
        {
            var context = new DynamoDBContext(Client);

            var item = context.LoadAsync<CreatStationCommand>(uid).Result;
        }
    }
    public class CreatStationCommand
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string[] Tags { get; set; }
    }
}
