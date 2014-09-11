using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UpdateControls.Correspondence;

namespace Festify.Model
{
    public partial class Session
    {
        public async Task SetDescription(string description)
        {
            var predecessors = await Description.EnsureAsync();
            var predecessorList = await ((PredecessorList<DocumentSegment>)predecessors).EnsureAsync();
            var segments = await Task.WhenAll(predecessorList
                .Select(p => p.EnsureAsync()));
            var oldDecsription = segments.JoinSegments();
            if (description != oldDecsription)
                Description = await Community.DocumentSegmentsAsync(description);
        }
    }
}
