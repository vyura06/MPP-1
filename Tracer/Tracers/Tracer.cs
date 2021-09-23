using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Tracers
{
    class ThreadNode : INode //реализация интерфейса INode
    {
        private Thread _thread;
        private TimeSpan _deltaTime;
        private IEnumerable<IMethod> _methods;

        public ThreadNode(Thread thread, TimeSpan deltaTime, IEnumerable<IMethod> methods)
        {
            _thread = thread;
            _deltaTime = deltaTime;
            _methods = methods;
        }

        public Thread Thread => _thread;

        public TimeSpan DeltaTime => _deltaTime;

        public IEnumerable<IMethod> Methods => _methods;
    }

    class Method : IMethod
    {
        public StackTrace stackTrace;
        public DateTime start, end;
        public List<Method> children;

        public Type Class => MethodBase.ReflectedType;

        public MethodBase MethodBase => stackTrace.GetFrame(1).GetMethod(); //GetFrame получаем родительский метод, который нас вызвал

        public TimeSpan DeltaTime => end - start;

        public IEnumerable<IMethod> Methods => children;
    }

    class Node
    {
        public readonly Stack<Method> stack = new Stack<Method>();
        public readonly List<Method> methods = new List<Method>();
    }

    public class Tracer : ITracer
    {
        private readonly IDictionary<Thread, Node> pairs = new Dictionary<Thread, Node>();

        public void StartTrace()
        {
            lock(pairs)
            {
                var method = new Method();
                var thread = Thread.CurrentThread;
                Node node;
                if (pairs.ContainsKey(thread))
                {
                    node = pairs[thread];
                }
                else
                {
                    node = new Node();
                    pairs.Add(thread, node);
                }

                var stack = node.stack;
                stack.Push(method);
                method.stackTrace = new StackTrace();
                method.children = new List<Method>();
                method.start = DateTime.Now;
            }
        }

        public void StopTrace()
        {
            lock(pairs)
            {
                var endTime = DateTime.Now;

                var thread = Thread.CurrentThread;
                var node = pairs[thread];

                var stack = node.stack;
                if (stack.Count == 0)
                {
                    throw new Exception("Stop trace called without start trace method");
                }

                var method = stack.Peek();
                var startStackTrace = method.stackTrace;
                var currentStackTrace = new StackTrace();

                if (!EqualsCallerStackTrace(startStackTrace, currentStackTrace))
                {
                    throw new Exception($"Start trace and stop trace called in differen methods:\n{startStackTrace}\n{currentStackTrace}");
                }

                method = stack.Pop();
                method.end = endTime;

                if (stack.Count == 0)
                {
                    node.methods.Add(method);
                }
                else
                {
                    var parent = stack.Peek();
                    parent.children.Add(method);
                }
            }
        }

        private bool EqualsCallerStackTrace(StackTrace s1, StackTrace s2) //сравнение StackTrace
        {
            if (s1.FrameCount != s2.FrameCount)
            {
                return false;
            }
            int len = s1.FrameCount;
            for (int i = 1; i < len; i++)
            {
                if (!Equals(s1.GetFrame(i)?.GetMethod(),
                    s2.GetFrame(i)?.GetMethod()))
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerable<INode> GetResult()
        {
            lock(pairs)
            {
                var nodes = new List<INode>();
                foreach (KeyValuePair<Thread, Node> pair in pairs)
                {
                    var deltaTime = TimeSpan.Zero;
                    var methods = pair.Value.methods;
                    foreach (IMethod method in methods) {
                        deltaTime += method.DeltaTime;
                    }
                    nodes.Add(new ThreadNode(pair.Key, deltaTime, methods));
                }
                return nodes;
            }
        }

        public void ClearMyHistory()
        {
            lock (pairs)
            {
                var thread = Thread.CurrentThread;
                if (pairs.ContainsKey(thread))
                {
                    var node = pairs[thread];
                    node.stack.Clear();
                    node.methods.Clear();
                }
            }
        }
    }
}
