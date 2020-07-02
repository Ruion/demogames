using System.Reflection;
using System;

public static class FieldExtension
{
    public static void SetFieldValue(this object obj, string propName, object value)
    {
        obj.GetType().GetField(propName).SetValue(obj, value);
    }

    public static object GetFieldValue(this object obj, string fieldName)
    {
        return obj.GetType().GetField(fieldName).GetValue(obj);
    }
}