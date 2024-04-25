using System;
using System.Linq;
using System.Collections.Generic;
using WebGate.Azure.TableUtils.Converter;
using System.Runtime.CompilerServices;
using Azure.Data.Tables;
using Microsoft.VisualBasic;
using System.Security.Cryptography;

namespace WebGate.Azure.TableUtils;
public static class ObjectBuilder
{
    public static T Build<T>(TableEntity tableEntity)
    {
        T result = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
        ProcessObject(result, null, tableEntity);
        return result;

    }
    private static void ProcessObject(object obj, string? path, TableEntity tableEntity)
    {
        obj.GetType().GetProperties().Where(propertyInfo => propertyInfo.CanRead && propertyInfo.CanWrite).ToList().ForEach(propertyInfo =>
        {
            string id = propertyInfo.Name;
            string entityName = BuildEntityName(path, id);
            if (tableEntity.TryGetValue(entityName, out var value))
            {
                Type pType = propertyInfo.PropertyType;
                IConverter? converter = ConverterFactory.FindConverter(pType);
                if (converter != null)
                {
                    propertyInfo.SetValue(obj, converter.BuildValue(value != null ? value.ToString():null, pType), index: null);
                }
                else
                {
                    if (value.GetType().FullName == "System.DateTimeOffset" && pType.FullName == "System.DateTime") {
                        DateTimeOffset dtoValue = (DateTimeOffset)value;
                        propertyInfo.SetValue(obj,dtoValue.DateTime, index: null);
                        return;
                    }
                    if (pType.IsValueType || pType.Name == "Byte[]" || pType.Name == "String")
                    {
                        propertyInfo.SetValue(obj, value, index: null);
                    } else {
                        object child = RuntimeHelpers.GetUninitializedObject(pType);
                        ProcessObject(child, id, tableEntity);
                    }
                }
            } else {
                Type pType = propertyInfo.PropertyType;
                if (!pType.IsValueType && pType.Name != "Byte[]" || pType.Name != "String") {
                    if (HasChildObjectInformations(id,tableEntity)) {
                        object child = RuntimeHelpers.GetUninitializedObject(pType);
                        ProcessObject(child, id, tableEntity);
                        propertyInfo.SetValue(obj, child, index: null);
                    }
                }
            }

        });

    }

    private static bool HasChildObjectInformations(string id, TableEntity tableEntity)
    {
        return tableEntity.Where(p=>p.Key.StartsWith(id+"_") && p.Value != null).Count() > 0;
    }

    private static string BuildEntityName(string? path, string id)
    {
        if (string.IsNullOrEmpty(path))
        {
            return id;
        }
        return path + "_" + id;
    }

}
