using System.Text.Json;

namespace ChatService
{
    public static class JsonSerializerOption
    {
        public static readonly JsonSerializerOptions Option = new JsonSerializerOptions {
            IncludeFields = true,
            IgnoreNullValues = true,
        };
    }
}
