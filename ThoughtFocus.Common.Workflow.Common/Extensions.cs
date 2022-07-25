using log4net;
using OptimaJet.Common;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq; 
using Microsoft.AspNetCore.Http;

namespace ThoughtFocus.Common
{
    public static class Extensions
    {
        public static XElement SingleOrDefault(this XElement element, string name)
        {
            return element.Name == name ? element : element.Elements(name).SingleOrDefault();
        }

        public static string Body(this HttpRequest request)
        {
            //During migration HttpRequestBase was not found in core so added HttpRequest
            string body;

            using (var reader = new StreamReader(request.Body))
            {
                //During migration request.Inputstream was not found in core so added request.Body
                //  request.Inputstream.Position = 0;
                request.Body.Position = 0;
                body = reader.ReadToEnd();
            }

            return body;
        }

        public static Type ToNullableType(this Type type)
        {
            var newType = Nullable.GetUnderlyingType(type) ?? type;
            return newType.IsValueType ? typeof(Nullable<>).MakeGenericType(newType) : newType;
        }

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static Type GetUnderlyingType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static string ToLowerCaseString(this bool value)
        {
            return value.ToString(CultureInfo.InvariantCulture).ToLower();
        }

        public static bool ExtendedEquals(this object value, object valueToCompare)
        {
            if (valueToCompare == null)
            {
                return value == null;
            }

            if (value.GetType() == valueToCompare.GetType() && value is IEnumerable)
            {
                var valueArray = (value as IEnumerable).Cast<object>().ToArray();
                var valueToCompareArray = (valueToCompare as IEnumerable).Cast<object>().ToArray();

                if (valueArray.Length != valueToCompareArray.Length)
                    return false;

                for (int i = 0;i<valueArray.Length; i++)
                {
                    if (valueArray[i] == null &&  valueToCompareArray[i] != null)
                        return false;
                     if (valueArray[i] != null &&  valueToCompareArray[i] == null)
                        return false;
                     if (valueArray[i] != null && valueToCompareArray[i] != null)
                         if (!valueArray[i].Equals(valueToCompareArray[i]))
                             return false;
                }

                return true;
            }

            return value.Equals(valueToCompare);
        }

        public static LogMessage GetMessage(string message, object debugData)
        {
            return new LogMessage() { message = message, json = debugData };
        }

        public static void Debug(this ILog log, string message, object debugData)
        {
            log.Debug(GetMessage(message, debugData));
        }

        public static void Info(this ILog log, string message, object debugData)
        {
            log.Info(GetMessage(message, debugData));
        }

        public static void Warn(this ILog log, string message, object debugData)
        {
            log.Warn(GetMessage(message, debugData));
        }

        public static void Warn(this ILog log, string message, object debugData, Exception exception)
        {
            log.Warn(GetMessage(message, debugData), exception);
        }

        public static void Error(this ILog log, string message, object debugData)
        {
            log.Error(GetMessage(message, debugData));
        }

        public static void Error(this ILog log, string message, object debugData, Exception exception)
        {
            log.Error(GetMessage(message, debugData), exception);
        }

        public static void Fatal(this ILog log, string message, object debugData)
        {
            log.Fatal(GetMessage(message, debugData));
        }

        public static void Fatal(this ILog log, string message, object debugData, Exception exception)
        {
            log.Fatal(GetMessage(message, debugData), exception);
        }

        public static void LogMessage(this ILog log, Exception ex)
        {
            string errorMessage = String.Empty;
            errorMessage = ex.ToString();
            log.Error(errorMessage);
            //Log To Database
        }
    }
}
