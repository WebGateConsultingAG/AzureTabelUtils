using Azure;
using Azure.Data.Tables;

namespace WebGate.Azure.TableUtils.Models;

public class TableEntityResult<T> {
    public string RowKey { get; set; }
    public string PartitionKey {set;get;}
    public ETag ETag { get; set;}
    public DateTimeOffset? Timestamp { get; set; }
    public T Entity { get; set; }

    public TableEntityResult(ITableEntity tableEntity, T entity) {
        this.PartitionKey = tableEntity.PartitionKey;
        this.Entity = entity;
        this.ETag = tableEntity.ETag;
        this.RowKey = tableEntity.RowKey;
        this.Timestamp =tableEntity.Timestamp;
    }

    public static TableEntityResult<TCreate> BuildTableEntityResult<TCreate>(TableEntity tableEntity) {
        var businessEntity =  ObjectBuilder.Build<TCreate>(tableEntity);
        return new TableEntityResult<TCreate>(tableEntity, businessEntity);
    }
}