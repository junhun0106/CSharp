using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApplication2
{
    public static class ModelBindingTester
    {
        public static void Test()
        {
            var options = new System.Text.Json.JsonSerializerOptions();
            options.Converters.Add(new DictionaryIntIntConverter());
            options.Converters.Add(new DictionaryUlongIntConverter());

            // 만약 ModelBinding을 사용하고 있다면, 바인딩이 불가능한 구조체가 있는지 미리 판별하자 
            //JsonDeserializeTester.Call();
            var types = new List<Type>();
            foreach (var type in types) {
                try {
                    var inst = Activator.CreateInstance(type);
                    // NOTE : Json 포맷으로 만들어주기 위한 작업, 어떤 시리얼라이즈던 상관 없다
                    var serialize = Newtonsoft.Json.JsonConvert.SerializeObject(inst);
                    var desrialize = System.Text.Json.JsonSerializer.Deserialize(serialize, type, options);
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }

            {
                var dicintint = new Dictionary<int, int> { [1] = 1, [2] = 2 };
                var j = Newtonsoft.Json.JsonConvert.SerializeObject(dicintint);
                var d = JsonSerializer.Deserialize<Dictionary<int, int>>(j, options);
                var element1 = d.ElementAt(0);
                System.Diagnostics.Debug.Assert(element1.Key == 1 && element1.Value == 1);
                var element2 = d.ElementAt(1);
                System.Diagnostics.Debug.Assert(element2.Key == 2 && element2.Value == 2);
            }
            {
                var diculongint = new Dictionary<ulong, int> { [1] = 1, [2] = 2 };
                var j = Newtonsoft.Json.JsonConvert.SerializeObject(diculongint);
                var d = JsonSerializer.Deserialize<Dictionary<ulong, int>>(j, options);
                var element1 = d.ElementAt(0);
                System.Diagnostics.Debug.Assert(element1.Key == 1 && element1.Value == 1);
                var element2 = d.ElementAt(1);
                System.Diagnostics.Debug.Assert(element2.Key == 2 && element2.Value == 2);
            }
        }
    }

    public class DictionaryUlongIntConverter : JsonConverter<Dictionary<ulong, int>>
    {
        public override Dictionary<ulong, int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) {
                throw new JsonException();
            }

            var value = new Dictionary<ulong, int>();

            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndObject) {
                    return value;
                }

                string keyString = reader.GetString();

                if (!ulong.TryParse(keyString, out var itemKey)) {
                    throw new JsonException($"Unable to convert \"{keyString}\" to System.Int32.");
                }

                reader.Read();
                var itemValue = reader.GetInt32();
                value.Add(itemKey, itemValue);
            }

            throw new JsonException("Error Occured");
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<ulong, int> value, JsonSerializerOptions options) => throw new NotImplementedException();
    }

    public class DictionaryIntIntConverter : JsonConverter<Dictionary<int, int>>
    {
        public override Dictionary<int, int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) {
                throw new JsonException();
            }

            var value = new Dictionary<int, int>();

            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndObject) {
                    return value;
                }

                var keyString = reader.GetString();
                if (!int.TryParse(keyString, out int itemKey)) {
                    throw new JsonException($"Unable to convert \"{keyString}\" to System.Int32.");
                }
                reader.Read();
                var itemValue = reader.GetInt32();
                value.Add(itemKey, itemValue);
            }

            throw new JsonException("Error Occured");
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<int, int> value, JsonSerializerOptions options) => throw new NotImplementedException();
    }

    public static class BindExtensions
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions();

        static BindExtensions()
        {
            // Newtonsoft.Json보다 성능이 보다 좋다는 System.Text.Json을 사용하자
            // 주의할점은 Dictionary를 사용 할때는 Converter를 따로 만들어주어야 한다
            _options.Converters.Add(new DictionaryIntIntConverter());
            _options.Converters.Add(new DictionaryUlongIntConverter());
        }

        public static async Task<T> Bind<T>(this HttpRequest request)
        {
            try {
                return await JsonSerializer.DeserializeAsync<T>(request.Body, _options).ConfigureAwait(false);
            } catch (Exception e) {
                //Logger.Log.Error(nameof(BindExtensions), $"Binding error - {request.Path}, {request.Method}, {e}");
                var type = typeof(T);
                return !type.IsValueType && type != typeof(string)
                     ? Activator.CreateInstance<T>()
                     : default(T);
            }
        }
    }

    public static class ModelBinderHelper
    {
        public static string ToString(Stream s)
        {
            using var sr = new StreamReader(s);
            return sr.ReadToEnd();
        }

        private static bool HasRequestBody(string httpMethod)
        {
            switch (httpMethod) {
                case "GET":
                case "DELETE":
                case "HEAD":
                case "OPTIONS":
                    return false;
                case "POST":
                case "PUT":
                case "PATCH":
                    break;
            }

            return true;
        }

        public static async Task<T> BindOrDefault<T>(this HttpRequest request)
        {
            if (HasRequestBody(request.Method)) {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var result = await request.Bind<T>().ConfigureAwait(false);
                //LogicStatistics.Collect("ModelBinderHelper", $"{request.Path}.{request.Method} BindOrDefault", sw);
                return result;
            } else {
                var model = Activator.CreateInstance<T>();
                var modelType = model.GetType();
                foreach (var fi in modelType.GetFields()) {
                    if (request.Query.TryGetValue(fi.Name, out var sv) && sv.Count > 0) {
                        var valueStr = sv[0];
                        var ft = fi.FieldType;
                        if (ft == typeof(String)) {
                            fi.SetValue(model, valueStr);
                        } else if (ft == typeof(Boolean)) {
                            fi.SetValue(model, Boolean.Parse(valueStr));
                        } else if (ft == typeof(Char)) {
                            fi.SetValue(model, Char.Parse(valueStr));
                        } else if (ft == typeof(SByte)) {
                            fi.SetValue(model, SByte.Parse(valueStr));
                        } else if (ft == typeof(Byte)) {
                            fi.SetValue(model, Byte.Parse(valueStr));
                        } else if (ft == typeof(Int16)) {
                            fi.SetValue(model, Int16.Parse(valueStr));
                        } else if (ft == typeof(UInt16)) {
                            fi.SetValue(model, UInt16.Parse(valueStr));
                        } else if (ft == typeof(Int32)) {
                            fi.SetValue(model, Int32.Parse(valueStr));
                        } else if (ft == typeof(UInt32)) {
                            fi.SetValue(model, UInt32.Parse(valueStr));
                        } else if (ft == typeof(Int64)) {
                            fi.SetValue(model, Int64.Parse(valueStr));
                        } else if (ft == typeof(UInt64)) {
                            fi.SetValue(model, UInt64.Parse(valueStr));
                        } else if (ft == typeof(Single)) {
                            fi.SetValue(model, Single.Parse(valueStr));
                        } else if (ft == typeof(Double)) {
                            fi.SetValue(model, Double.Parse(valueStr));
                        } else if (ft == typeof(Decimal)) {
                            fi.SetValue(model, Decimal.Parse(valueStr));
                        } else if (ft == typeof(DateTime)) {
                            fi.SetValue(model, DateTime.Parse(valueStr));
                        } else if (ft == typeof(BigInteger)) {
                            fi.SetValue(model, BigInteger.Parse(valueStr));
                        } else if (ft == typeof(Guid)) {
                            fi.SetValue(model, Guid.Parse(valueStr));
                        } else if (ft == typeof(DateTimeOffset)) {
                            fi.SetValue(model, DateTimeOffset.Parse(valueStr));
                        } else if (ft.IsEnum) {
                            try {
                                fi.SetValue(model, Enum.Parse(ft, valueStr));
                            } catch (Exception e) {
                                var loggerFactory = request.HttpContext.RequestServices.GetService<ILoggerFactory>();
                                var logger = loggerFactory.CreateLogger("BindOrDefault");
                                var url = request.GetDisplayUrl();
                                logger.LogError($" {url}, {modelType.Name}, {fi.Name}, ex:{e.Message}");
                            }
                        } else {
                            var loggerFactory = request.HttpContext.RequestServices.GetService<ILoggerFactory>();
                            var logger = loggerFactory.CreateLogger("BindOrDefault");
                            var url = request.GetDisplayUrl();
                            logger.LogError($" {url}, {modelType.Name}, {fi.Name}");
                        }
                    }
                }

                foreach (var pi in modelType.GetProperties()) {
                    if (request.Query.TryGetValue(pi.Name, out var sv) && sv.Count > 0) {
                        var valueStr = sv[0];
                        var pt = pi.PropertyType;
                        if (pt == typeof(String)) {
                            pi.SetValue(model, valueStr);
                        } else if (pt == typeof(Boolean)) {
                            pi.SetValue(model, Boolean.Parse(valueStr));
                        } else if (pt == typeof(Char)) {
                            pi.SetValue(model, Char.Parse(valueStr));
                        } else if (pt == typeof(SByte)) {
                            pi.SetValue(model, SByte.Parse(valueStr));
                        } else if (pt == typeof(Byte)) {
                            pi.SetValue(model, Byte.Parse(valueStr));
                        } else if (pt == typeof(Int16)) {
                            pi.SetValue(model, Int16.Parse(valueStr));
                        } else if (pt == typeof(UInt16)) {
                            pi.SetValue(model, UInt16.Parse(valueStr));
                        } else if (pt == typeof(Int32)) {
                            pi.SetValue(model, Int32.Parse(valueStr));
                        } else if (pt == typeof(UInt32)) {
                            pi.SetValue(model, UInt32.Parse(valueStr));
                        } else if (pt == typeof(Int64)) {
                            pi.SetValue(model, Int64.Parse(valueStr));
                        } else if (pt == typeof(UInt64)) {
                            pi.SetValue(model, UInt64.Parse(valueStr));
                        } else if (pt == typeof(Single)) {
                            pi.SetValue(model, Single.Parse(valueStr));
                        } else if (pt == typeof(Double)) {
                            pi.SetValue(model, Double.Parse(valueStr));
                        } else if (pt == typeof(Decimal)) {
                            pi.SetValue(model, Decimal.Parse(valueStr));
                        } else if (pt == typeof(DateTime)) {
                            pi.SetValue(model, DateTime.Parse(valueStr));
                        } else if (pt == typeof(BigInteger)) {
                            pi.SetValue(model, BigInteger.Parse(valueStr));
                        } else if (pt == typeof(Guid)) {
                            pi.SetValue(model, Guid.Parse(valueStr));
                        } else if (pt == typeof(DateTimeOffset)) {
                            pi.SetValue(model, DateTimeOffset.Parse(valueStr));
                        } else if (pt.IsEnum) {
                            try {
                                pi.SetValue(model, Enum.Parse(pt, valueStr));
                            } catch (Exception e) {
                                var loggerFactory = request.HttpContext.RequestServices.GetService<ILoggerFactory>();
                                var logger = loggerFactory.CreateLogger("BindOrDefault");
                                var url = request.GetDisplayUrl();
                                logger.LogError($" {url}, {modelType.Name}, {pi.Name}, ex:{e.Message}");
                            }
                        } else {
                            var loggerFactory = request.HttpContext.RequestServices.GetService<ILoggerFactory>();
                            var logger = loggerFactory.CreateLogger("BindOrDefault");
                            var url = request.GetDisplayUrl();
                            logger.LogError($" {url}, {modelType.Name}, {pi.Name}");
                        }
                    }
                }

                return model;
            }
        }
    }
}
