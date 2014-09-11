using Festify.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpdateControls.Correspondence;

namespace Festify.Model
{
    public partial class Speaker
    {
        public async Task SetBio(string bio)
        {
            var predecessors = await Bio.EnsureAsync();
            var predecessorList = await ((PredecessorList<DocumentSegment>)predecessors).EnsureAsync();
            var segments = await Task.WhenAll(predecessorList
                .Select(p => p.EnsureAsync()));
            var oldBio = segments.JoinSegments();
            if (bio != oldBio)
                Bio = await Community.DocumentSegmentsAsync(bio);
        }
    }
}
