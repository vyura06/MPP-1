using System.Collections.Generic;
using System.IO;
using Tracers;

namespace Savers
{
    public interface ISaver
    {
        void Save(Stream output, IEnumerable<INode> traceResult);
    }
}
