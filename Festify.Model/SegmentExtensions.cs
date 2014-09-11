using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UpdateControls.Correspondence;

namespace Festify.Model
{
    public static class SegmentExtensions
    {
        public static string JoinSegments(this IEnumerable<DocumentSegment> segments)
        {
            if (segments == null)
                return null;
            return String.Join("", segments
                .Select(segment => segment.Text)
                .ToArray());
        }

        public static async Task<List<DocumentSegment>> DocumentSegmentsAsync(this ICommunity community, string text)
        {
            List<DocumentSegment> segments = new List<DocumentSegment>();
            while (!String.IsNullOrEmpty(text))
            {
                int segmentLength = Math.Min(512, text.Length);
                DocumentSegment segment = await community.AddFactAsync(new DocumentSegment(text.Substring(0, segmentLength)));
                segments.Add(segment);
                text = text.Substring(segmentLength);
            }
            return segments;
        }
    }
}
