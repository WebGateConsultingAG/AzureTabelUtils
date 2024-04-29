# WebGate.Azure.TableUtils

WebGate.Azure.TablesUtils provides assets to support Azure.Data.TableClient, which allows direct access in the form of CRUD operation to the entities.
Complex entities, arrays and IEnumerable are also supported.

The mainfocus for this implementation is the usage in Azure Functions. Therefor access to the tables is mainly done by the ConnectionString. SAS and other authentication methodes are so far not supported. But could be implemented based on the demand.

---
## ExtendedAzureTableClientService
The ExtendedAzureTableClientService provides a class to register and acces TypedAczureTableClients and also MultiEntityAzureTableClients.

### Create a new ExtendedAzureTableClientService
Given you have a valid connectionString to a AzureStorage Account V2 the creation is straightforward.

```c#
var connectionString = "MY_STRING";
var extendedTableService = new ExtendedAzureTableClientService(connectionString);
```

We recommend to initalize your ExtendedAzureTableClientSevice in the Startup/Programm.cs in an AzureFunction. The registration of TypedAzureTableClients is straightforward.

### Create and register some TypedAzureTableClients
You may have 2 Poco you want to save in 2 different tables. A SimpelPoco and a ParentPoco. To register these 2 Poco with a table for each, do the following:

```c#
var simplePocoAzureTableClient = extendedTableService.CreateAndRegisterTableClient<SimplePoco>("simplePojoTable");
var parentPocoAzureTableClient = extendedTableService.CreateAndRegisterTableClient<ParentPoco>("parentPojoTable");
```
### Accessing a TypedAzureTableClient
To get a specific TypeAzureTableClient, use the following approach with the ExtendedAzureTabbleClientService:
```c#
var simplePocoAzureTableClient = extendedTableService.GetTypedTableClient<SimplePoco>();
```
### Create and register a MultiEntityAzureTableClient
The purpose for a MultiEntityAzureTableClient is to store Entities of different types / kinds in one table. The registration of these Types is very easy.
```c#
var multiEntityTableClient = _extendedTableService.CreateAndRegisterMultiEntityTableClient("allpocos");
multiEntityTableClient.RegisterType<SimplePoco>();
multiEntityTableClient.RegisterType<MainWithParent>("mwp");
multiEntityTableClient.RegisterType<PocoWihtListChildren>();
```
The example registers a MultiEntityAzureTableClient bound to the Table called "allpocos". The types are registered with there TypeName as prefix for the rowkey, with one exception. MainWithParent is registered with "mwp" as prefix.

### Access a the MultiEntityAzureTableClient
Use the following code to access the registered MultiEntityAzureTableClient:
```c#
var multiEntityTableClient = _extendedTableService.GetMultiEntityAzureTableClientByTableName("allpocos");
```
The name of the table during registration is your key. Details about the usage of the client, see below.

## TableEntityResult<T>
Results form TypedAzureTableClient and MultiEntityAzureTableClient are wrapped into the class TableEntityResult. For a result from a TypedAzureTalbeClient is the generic T as the type of the Client, while in a result form a MultiEntityAzureTableClient the Type is always object. The class has the following signature:
```c#
public class TableEntityResult<T>(ITableEntity tableEntity, T entity)
{
    public string RowKey { get; set; } = tableEntity.RowKey;
    public string PartitionKey { set; get; } = tableEntity.PartitionKey;
    public ETag ETag { get; set; } = tableEntity.ETag;
    public DateTimeOffset? Timestamp { get; set; } = tableEntity.Timestamp;
    public T Entity { get; set; } = entity;
 }
```

## TypedAzureTableClient
The TypedAzureTableClient is a Decorator to the AzurTableClient. The main purpose is to extend conversion from and to the defined Entity with the capability for complex entities, arrays and IEnumerable. The client can be initalized via ExtendAzureTableClientService or direct in the code, using the following pattern.

Get the client from the service:
```c#
var typedTableClient = _extendedTableService.GetTypedTableClient<MyPoco>();
```

Initialize Inline:
```c#
var connectionString = "MY_STRING"; //String to Azure Storage Account V2
var tableClient = new TableClient(connectionString, "MyPoco"); // From Azure.Data.Table
await tableClient.CreateIfNotExistsAsync();
var typedTableClient = new TypedAzureTableClient<MyPoco>(tableClient);
```

For all Examples, we are using a TypedAzureTableClient bound to MyPoco as generic.
The following operations are provided:

### GetAllAsync()

```c#
List<TableEntityResult<MyPoco>> pocos = await typedTableClient.GetAllAsync();
```

Gets all data from a table and convert them into the specified Object. No partition key is applied.

### GetAllAsync<T>(string partition)

```c#
List<TableEntityResult<MyPoco>> pocos = await typedTableClient.GetAllAsync('mypoco');
```

Gets all data from a table and convert them into the specified Object. A partitionkey is applied. The current example applies 'mypoco' as partitionkey.

### GetByIdAsync<T>(string id)

```c#
TableEntityResult<MyPoco>? poco = await typedTableClient.GetByIdAsync('1018301');
```

Gets as specific entity from the table and convert it to the specified object. The name of the type is used as partitionkey. In the current example 'MyPoco'.
If the id+partitionkey combination finds now object null is returned.

### GetByIdAsync<T>(string id, string partition)

```c#
TableEntityResult<MyPoco>? poco = await typedTableClient.GetByIdAsync('9201u819','mypoco');
```

Gets as specific enitity form the table and convert it to the specified object. The partionkey is the 2nd argument.
If the id+partitionkey combination finds now object null is returned.

### GetAllByQueryAsync(TableQuery query)

```c#
var query = $"PartitionKey eq '{partitionKey}'";
List<TableEntityResult<MyPoco>> pocos = await typedTableClient.GetAllByQueryAsync(query);
```

Gets alls entites that matches the query.

### InsertOrMergeAsync(string id, string partition, object obj)

```c#
MyPoco poco = new MyPoco();
// Do magicStuff with poco
Azure.Response result = await typedTableClient.InsertOrMergeAsync("001", "SimplePoco", poco);
```

Creates or merges a specific object into the table. The selection is done by id and partitionkey.

### InsertOrReplaceAsync(string id, string partition, object obj)

```c#
MyPoco poco = new MyPoco();
// Do magicStuff with poco
Azure.Response result = await typedTableClient.InsertOrReplaceAsync("001", "SimplePoco", poco);
```

Creates or replace a specific object into the table. The selection is done by id and partitionkey.

### DeleteEntryAsync(string id, string partition)

```c#
Azure.Response result = await typedTableClient.DeleteEntityAsync("001", "SimplePoco");
```

Deletes a specific object in the table. The selection is done by id and partitionkey.

---

## Code Quality Check SonarCloud.io

[![Quality gate](https://sonarcloud.io/api/project_badges/quality_gate?project=CloudTableUtils&token=b8ea0b7d7b29c7e13fb260bae8cf0d3eb36597ec)](https://sonarcloud.io/dashboard?id=CloudTableUtils)

---

## Licence

Apache V 2.0

---

## Copyright

2024, WebGate Consulting AG
