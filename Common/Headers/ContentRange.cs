using System;
using System.Text.RegularExpressions;

namespace HttpServer.Common
{
    public class ContentRange
    {
        private const string Unit = "bytes";
        private static readonly Regex regex = new Regex(@"(\d+-\d+)/\d+|\*", RegexOptions.Compiled);

        public static bool TryParse(string contentRangeHeaderString, out ContentRange contentRange)
        {
            contentRange = null;
            if (string.IsNullOrEmpty(contentRangeHeaderString))
            {
                return false;
            }
            var prefixLength = contentRangeHeaderString.IndexOf(" ", StringComparison.Ordinal);
            if (prefixLength != -1)
            {
                contentRangeHeaderString = contentRangeHeaderString.Substring(prefixLength + 1);
            }
            if (string.IsNullOrEmpty(contentRangeHeaderString) || !regex.IsMatch(contentRangeHeaderString))
            {
                return false;
            }
            var splittedContentRange = contentRangeHeaderString.Split(new[] { '-', '/' },
                StringSplitOptions.RemoveEmptyEntries);
            if (splittedContentRange.Length != 3)
            {
                return false;
            }
            long startIndex;
            long endIndex;
            long? totalLength;
            if (!long.TryParse(splittedContentRange[0], out startIndex) ||
                !long.TryParse(splittedContentRange[1], out endIndex) ||
                !TryParseTotalLength(splittedContentRange[2], out totalLength))
            {
                return false;
            }
            if (startIndex > endIndex || endIndex >= totalLength || startIndex < 0 || totalLength < 0)
            {
                return false;
            }
            contentRange = new ContentRange(startIndex, endIndex, totalLength);
            return true;
        }

        private static bool TryParseTotalLength(string value, out long? result)
        {
            result = 0;
            if (value == "*")
            {
                result = null;
                return true;
            }
            long parsedResult;
            if (!long.TryParse(value, out parsedResult))
                return false;
            result = parsedResult;
            return true;
        }

        public readonly long StartIndex;
        public readonly long EndIndex;
        public readonly long? TotalLength;
        public long ContentLength => EndIndex - StartIndex + 1;

        public ContentRange(long startIndex, long endIndex, long? totalLength = null)
        {
            Preconditions.EnsureCondition(startIndex >= 0, "startIndex", "StartIndex cannot be less then zero");
            Preconditions.EnsureCondition(endIndex >= startIndex, "startIndex", "EndIndex cannot be less then StartIndex");
            Preconditions.EnsureCondition(totalLength > endIndex, "totalLength", "TotalLength cannot be less or equals to EndIndex");
            StartIndex = startIndex;
            EndIndex = endIndex;
            TotalLength = totalLength;
        }

        public static bool operator ==(ContentRange left, ContentRange right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ContentRange left, ContentRange right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            var contentRange = obj as ContentRange;
            if (contentRange == null)
            {
                return false;
            }
            return StartIndex == contentRange.StartIndex &&
                   EndIndex == contentRange.EndIndex &&
                   TotalLength == contentRange.TotalLength;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StartIndex.GetHashCode();
                hashCode = (hashCode * 397) ^ EndIndex.GetHashCode();
                hashCode = (hashCode * 397) ^ TotalLength.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}-{2}/{3}", Unit, StartIndex, EndIndex, TotalLength?.ToString() ?? "*");
        }
    }
}