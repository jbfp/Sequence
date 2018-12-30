using Newtonsoft.Json;
using Sequence.Core;
using System;

namespace Sequence.Api
{
    internal sealed class GameIdJsonConverter : JsonConverter
    {
        public override bool CanRead { get; } = true;
        public override bool CanWrite { get; } = true;

        public override bool CanConvert(Type objectType) => typeof(GameId) == objectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var str = reader.ReadAsString();
            var guid = Guid.Parse(str);
            return new GameId(guid);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    internal sealed class PlayerHandleJsonConverter : JsonConverter
    {
        public override bool CanRead { get; } = true;
        public override bool CanWrite { get; } = true;

        public override bool CanConvert(Type objectType) => typeof(PlayerHandle) == objectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new PlayerHandle(reader.ReadAsString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    internal sealed class PlayerIdJsonConverter : JsonConverter
    {
        public override bool CanRead { get; } = true;
        public override bool CanWrite { get; } = true;

        public override bool CanConvert(Type objectType) => typeof(PlayerId) == objectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new PlayerId(reader.ReadAsInt32().Value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((PlayerId)value).ToInt32());
        }
    }

    internal sealed class TileJsonConverter : JsonConverter
    {
        public override bool CanRead { get; } = false;
        public override bool CanWrite { get; } = true;

        public override bool CanConvert(Type objectType) => typeof((Suit, Rank)?) == objectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
            }
            else if (value is ValueTuple<Suit, Rank> tile)
            {
                writer.WriteStartArray();
                serializer.Serialize(writer, tile.Item1);
                serializer.Serialize(writer, tile.Item2);
                writer.WriteEndArray();
            }
            else
            {
                throw new JsonSerializationException($"Cannot write value: '{value}'.");
            }
        }
    }
}