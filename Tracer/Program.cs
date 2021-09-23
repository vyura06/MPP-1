using System.IO;
using System.Threading;
using Tracers;
using Savers;

namespace TracerApp
{

    public class Foo
    {
        private ITracer _tracer;

        public Foo(ITracer tracer)
        {
            _tracer = tracer;
        }

        private void M1()
        {
            _tracer.StartTrace();
            Thread.Sleep(100);
            _tracer.StopTrace();
        }

        private void M2()
        {
            _tracer.StartTrace();
            Thread.Sleep(200);
            _tracer.StopTrace();
        }

        public void M3()
        {
            M1();
            M2();
        }

        public void M4()
        {
            _tracer.StartTrace();
            M1();
            _tracer.StopTrace();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tracer = new Tracer();

            var foo = new Foo(tracer);
            foo.M3();

            var file = "TraceResult";
            SaveToJson(file, tracer);
            SaveToXML(file, tracer);
        }
 

        static void SaveToJson(string fileName, ITracer tracer)
        {
            using var fs = new FileStream(fileName + ".json", FileMode.OpenOrCreate);
            ISaver saver = new JsonSaver();
            saver.Save(fs, tracer.GetResult());
        }

        static void SaveToXML(string fileName, ITracer tracer)
        {
            using var fs = new FileStream(fileName + ".xml", FileMode.OpenOrCreate);
            ISaver saver = new XmlSaver();
            saver.Save(fs, tracer.GetResult());
        }
    }
}

