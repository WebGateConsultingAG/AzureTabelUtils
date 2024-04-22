using System;
using System.Linq;
using System.Collections.Generic;
using WebGate.Azure.TableUtils.Converter;
using System.Runtime.CompilerServices;
using Azure.Data.Tables;
using Microsoft.VisualBasic;

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
                    if (pType.IsValueType || pType.Name == "Byte[]" || pType.Name == "String")
                    {
                        propertyInfo.SetValue(obj, value, index: null);
                        //throw new ConverterException("No convertor found for: " + id + " / " + pType.ToString());
                    } else {
                        object child = RuntimeHelpers.GetUninitializedObject(pType);
                        ProcessObject(child, id, tableEntity);
                    }
                }
            }

        });

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
