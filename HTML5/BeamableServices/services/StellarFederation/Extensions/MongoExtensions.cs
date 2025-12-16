using System.Numerics;
using Beamable.Server;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
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

        BsonClassMap.RegisterClassMap<ContractBase>(cm => {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });

        BsonClassMap.RegisterClassMap<CoinContract>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<GoldContract>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<ItemContract>(cm => cm.AutoMap());


        BsonClassMap.RegisterClassMap<QueuedTransactionBase>(cm => {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });

        BsonClassMap.RegisterClassMap<CurrencyAddInventoryRequest>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<CurrencySubtractInventoryRequest>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<ItemAddInventoryRequest>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<ItemUpdateInventoryRequest>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<ItemDeleteInventoryRequest>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<AccountCreateRequest>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<AccountCloseRequest>(cm => cm.AutoMap());
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