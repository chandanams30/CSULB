using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ThoughtFocus.Common.Workflow.Core.EnumerationHelpers
{
    public static class ExtensionHelper
    {
        public static string GetEnumDescription<TEnum>(this TEnum item)
        {

            if (item.GetType()
                   .GetField(item.ToString())
                   .GetCustomAttributes(typeof(DescriptionAttribute), false)
                   .Cast<DescriptionAttribute>()
                   .FirstOrDefault() != null)
            {
                return item.GetType()
                   .GetField(item.ToString())
                   .GetCustomAttributes(typeof(DescriptionAttribute), false)
                   .Cast<DescriptionAttribute>()
                   .FirstOrDefault().Description;

            }
            else
            {
                return string.Empty;
            }
        }

        public static void SeedEnumValues<T, TEnum>(this DbSet<T> dbSet, Func<TEnum, T> converter)
        where T : class
        {
            Enum.GetValues(typeof(TEnum))
                                  .Cast<object>()
                                  .Select(value => converter((TEnum)value))
                                  .ToList()
                                  .ForEach(instance => 
                                  
                                 //During migration AddOrUpdate was not found in EF core so added Update insted 
                                 // dbSet.AddOrUpdate(instance)

                                  dbSet.Update(instance) 
                                
                                  );
            return;
        }
    }
}
