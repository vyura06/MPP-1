using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;

namespace Tracers
{
    public interface INode
    {
        [JsonIgnore]//игнор для JsonSaver
        //определяем какой поток выполнял наши методы
        Thread Thread { get; }

        [JsonIgnore]
        //получаем временя, которое нам потребовалось на выполнение
        TimeSpan DeltaTime { get; }

        [JsonPropertyName("name")]
        //если имя = null получаем его из хэш-кода
        string ThreadName => Thread.Name ?? Thread.GetHashCode().ToString();

        [JsonPropertyName("time")]//для JsonSaver
        string DeltaTimeString => DeltaTime.ToString();

        [JsonPropertyName("methods")]
        //список методов, которые поток выполнил
        IEnumerable<IMethod> Methods { get; }
    }
}
