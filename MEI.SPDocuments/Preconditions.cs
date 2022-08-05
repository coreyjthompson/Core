using System;
using System.Collections.Generic;

namespace MEI.SPDocuments
{
    internal static class Preconditions
    {
        internal static T CheckNotNull<T>(string paramName, T argument)
            where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return argument;
        }

        internal static string CheckNotNullOrEmpty(string paramName, string argument)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(paramName);
            }

            return argument;
        }

        internal static long CheckRange(string paramName, long argument, long minInclusive, long maxInclusive)
        {
            if ((argument < minInclusive) || (argument > maxInclusive))
            {
                throw new ArgumentOutOfRangeException(paramName,
                    argument,
                    string.Format(Resources.Default.Value_should_be_in_range___0___1, minInclusive, maxInclusive));
            }

            return argument;
        }

        internal static int CheckRange(string paramName, int argument, int minInclusive, int maxInclusive)
        {
            if ((argument < minInclusive) || (argument > maxInclusive))
            {
                throw new ArgumentOutOfRangeException(paramName,
                    argument,
                    string.Format(Resources.Default.Value_should_be_in_range___0___1, minInclusive, maxInclusive));
            }

            return argument;
        }

        internal static bool CheckArgument(string paramName, bool argument, string message)
        {
            if (argument == false)
            {
                throw new ArgumentException(message, paramName);
            }

            return true;
        }

        internal static string CheckLength(string paramName, string argument, int minInclusive, int maxInclusive)
        {
            if ((argument.Length < minInclusive) || (argument.Length > maxInclusive))
            {
                throw new ArgumentException(string.Format(Resources.Default.Value_should_be_of_length___0___1, minInclusive, maxInclusive),
                    paramName);
            }

            return argument;
        }

        internal static T CheckEnum<T>(string paramName, T argument, T nullValue)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException(Resources.Default.T_must_be_an_enumerated_type);
            }

            if (argument.Equals(nullValue))
            {
                throw new ArgumentNullException(paramName);
            }

            return argument;
        }

        internal static IList<T> CheckCount<T>(string paramName, IList<T> argument, int minInclusiveCount, int maxInclusiveCount)
        {
            if ((argument.Count < minInclusiveCount) || (argument.Count > maxInclusiveCount))
            {
                throw new ArgumentException(
                    string.Format(Resources.Default.Value_Count_must_be_in_range___0___1, minInclusiveCount, maxInclusiveCount),
                    paramName);
            }

            return argument;
        }
    }
}
