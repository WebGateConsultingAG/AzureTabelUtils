using System.Net;
using Azure.Data.Tables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebGate.Azure.TableUtils;
using WebGate.Azure.TableUtils.Test;
namespace AzureTableUtils.IntegrationTests;

[TestClass]
public class CRUDIntegrationTest
{
    public TestContext TestContext { get; set; }
    public string ConnectionString { get; set; }

    [TestInitialize]
    public void TestInitialize()
    {
        ConnectionString = (string)TestContext.Properties["CONNECTION_STRING"];
    }

    [TestMethod]
    public async Task TestPojoCrud()
    {
        var tableClient = new TableClient(ConnectionString,"SimplePojoTable");
        await tableClient.CreateIfNotExistsAsync();
        var typedTableClient = new TypedAzureTableClient<SimplePoco>(tableClient);
        var poco = SimplePoco.CreateInitializedPoco();
        var id = Guid.NewGuid().ToString();
        var response = await typedTableClient.InsertOrReplaceAsync(id, "poco",poco);
        Assert.AreEqual(204, response.Status);
        var tableEntityRespose = await typedTableClient.GetByIdAsync(id,"poco");
        Assert.IsNotNull(tableEntityRespose);
        Assert.AreEqual(poco,tableEntityRespose.Entity);
        var deleteResponse = await typedTableClient.DeleteEntityAsync(id,"poco");
        Assert.AreEqual(204,deleteResponse.Status);
    }
}