using System.Collections.Generic;

namespace Tracers
{
    public interface ITracer
    {
        void StartTrace();

        void StopTrace();

        //двигаемся и производим перечисление, получаем элементы 

        IEnumerable<INode> GetResult();
    }
}
