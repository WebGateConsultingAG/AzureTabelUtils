using Azure.Data.Tables;

namespace WebGate.Azure.TableUtils;

public class ExtendedAzureTableClientService(string connectionString) {
    private readonly string _connectionString = connectionString;
    private Dictionary<Type,object> _tableClients = new();

    public TypedAzureTableClient<T> CreateAndRegisterTableClient<T>(string tableName) {
        var tableClient = new TableClient(_connectionString, tableName);
        var typedClient = new TypedAzureTableClient<T>(tableClient);
        _tableClients.Add(typeof(T), typedClient);
        typedClient.GetTableClient().CreateIfNotExists();
        return typedClient;
    }
    public void AddInitializedTableClient<T>(TableClient tableClient) {
        var typedClient = new TypedAzureTableClient<T>(tableClient);
        _tableClients.Add(typeof(T), typedClient);
        typedClient.GetTableClient().CreateIfNotExists();
    }

    public TypedAzureTableClient<T> GetTypedTableClient<T>(){
        if(_tableClients.TryGetValue(typeof(T), out var table)) {
            return (TypedAzureTableClient<T>)table;
        }
        throw new ArgumentOutOfRangeException(typeof(T).Name + " not found as registered TypedTableClient");
    }
}