using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MiniServer
{
    public static class Extensions
    {
        // Credits: https://stackoverflow.com/a/35920244/12771343
        public static ICollection<RangeItemHeaderValue> GetRanges(this HttpRequest request)
        {
            List<RangeItemHeaderValue> rangeHeaders = new();

            request.Headers.TryGetValue("Range", out StringValues values);
            foreach (string val in values)
            {
                if (val is null)
                    continue;

                string[] ranges = val.Replace("bytes=", string.Empty).Split(',');
                foreach (string range in ranges)
                {
                    string[] currentRange = range.Split('-');

                    long? start = null, end = null;
                    if (long.TryParse(currentRange[0], out long tmpStart))
                        start = tmpStart;

                    if (long.TryParse(currentRange[1], out long tmpEnd))
                        end = tmpEnd;

                    rangeHeaders.Add(new RangeItemHeaderValue(start, end));
                }
            }

            return rangeHeaders;
        }
    }
}
