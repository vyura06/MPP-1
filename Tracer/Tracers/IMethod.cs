using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Tracers
{
    public interface IMethod
    {
        [JsonIgnore]
        //класс в котором находится наш метод
        Type Class { get; }

        [JsonIgnore]
        //наш текущий метод
        MethodBase MethodBase { get; }

        [JsonIgnore]
        //время выполнения метода
        TimeSpan DeltaTime { get; }

        [JsonPropertyName("class")]//имя класса для json
        string ClassName => Class.Name;

        [JsonPropertyName("name")]//имя метода для json
        string MethodName => MethodBase.Name;

        [JsonPropertyName("time")]//время для json
        string DeltaTimeString => DeltaTime.ToString();

        [JsonPropertyName("methods")]//вложенные методы
        IEnumerable<IMethod> Methods { get; }
    }
}
