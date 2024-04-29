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


## TypedAzureTableClient

`using WebGate.Azure.CloudTableUtils.CloudTableExtension`

This Extension enables you to do CRUD Operations with your Poco direct to the CloudTable. The Poco do not have to extend TableEntity. The Object De/Serialisation is done inside of the functions and DynamicTableEntity are used to store and retrieve data.

The following operations are provided:

### GetAllAsync<T>()

```c#
List<MyPoco> pocos = await cloudTable.GetAllAsync<MyPoco>();
```

Gets all data from a table and convert them into the specified Object. No partition key is applied.

### GetAllAsync<T>(string partition)

```c#
List<MyPoco> pocos = await cloudTable.GetAllAsync<MyPoco>('mypoco');
```

Gets all data from a table and convert them into the specified Object. A partitionkey is applied. The current example applies 'mypoco' as partitionkey.

### GetByIdAsync<T>(string id)

```c#
MyPoco poco = await.GetByIdAsync<MyPoco>('1018301');
```

Gets as specific entity from the table and convert it to the specified object. The name of the type is used as partitionkey. In the current example 'MyPoco'.
If the id+partitionkey combination finds now object null is returned (using default(T))

### GetByIdAsync<T>(string id, string partition)

```c#
MyPoco poco = await.GetByIdAsync<MyPoco>('9201u819','mypoco');
```

Gets as specific enitity form the table and convert it to the specified object. The partionkey is the 2nd argument.
If the id+partitionkey combination finds now object null is returned (using default(T))

### GetAllByQueryAsync(TableQuery query)

```c#
TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>();
List<MyPoco> pocos = await cloudTable.GetAllByQueryAsync<MyPoco>(query);
```

Gets alls entites that matches the query.

### InsertOrMergeAsync(string id, string partition, object obj)

```c#
MyPoco poco = new MyPoco();
// Do magicStuff with poco
TableResult result = await cloudTable.InsertOrMergeAsync("001", "SimplePoco", poco);
```

Creates or merges a specific object into the cloud table. The selection is done by id and partitionkey.

### InsertOrReplaceAsync(string id, string partition, object obj)

```c#
MyPoco poco = new MyPoco();
// Do magicStuff with poco
TableResult result = await cloudTable.InsertOrReplaceAsync("001", "SimplePoco", poco);
```

Creates or replace a specific object into the cloud table. The selection is done by id and partitionkey.

### DeleteEntryAsync(string id, string partition)

```c#
TableResult result = await cloudTable.DeleteAsync("001", "SimplePoco");
```

Deletes a specific object in the cloud table. The selection is done by id and partitionkey.

---

## Code Quality Check SonarCloud.io

[![Quality gate](https://sonarcloud.io/api/project_badges/quality_gate?project=CloudTableUtils&token=b8ea0b7d7b29c7e13fb260bae8cf0d3eb36597ec)](https://sonarcloud.io/dashboard?id=CloudTableUtils)

---

## Licence

Apache V 2.0

---

## Copyright

2024, WebGate Consulting AG
