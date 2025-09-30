using System.Numerics;
using Beamable.Server;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Beamable.StellarFederation.Extensions;

public static class MongoExtensions
{
    public static void SetupMongoExtensions(this IServiceInitializer _)
    {
        BsonSerializer.RegisterSerializer(new BigIntegerNullableSerializer());
        BsonSerializer.RegisterSerializer(new BigIntegerSerializer());
    }
}

public class BigIntegerNullableSerializer : SerializerBase<BigInteger?>
{
    public override BigInteger? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.String)
        {
            var bigIntegerString = context.Reader.ReadString();
            return BigInteger.Parse(bigIntegerString);
        }

        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
        }
        return null;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, BigInteger? value)
    {
        if (value is not null)
            context.Writer.WriteString(value.Value.ToString());
        else
            context.Writer.WriteNull();
    }
}

public class BigIntegerSerializer : SerializerBase<BigInteger>
{
    public override BigInteger Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.String)
        {
            var bigIntegerString = context.Reader.ReadString();
            return BigInteger.Parse(bigIntegerString);
        }

        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
        }

        return default;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, BigInteger value)
    {
        context.Writer.WriteString(value.ToString());
    }
}