using Utf8Json;

namespace WorldImageMerger
{
    public static class Serialize
    {
        public static byte[] ToJson(this LdtkJson self) => JsonSerializer.Serialize(self);
    }
}