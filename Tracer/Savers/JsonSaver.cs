using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Tracers;

namespace Savers
{
    public class JsonSaver : ISaver
    {
        public void Save(Stream output, IEnumerable<INode> traceResult)
        {
            using var writer = new Utf8JsonWriter(output);
            JsonSerializer.Serialize(writer, traceResult);
        }
    }
}
