using System;

namespace ThoughtFocus.Common.Workflow.Core.EnumerationHelpers
{
    static class ExceptionHelpers
    {
        public static void ThrowIfNotEnum<TEnum>()
            where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new Exception("");
            }
        }
    }
}
