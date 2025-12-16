using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using StellarDotnetSdk;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Operations;
using StellarDotnetSdk.Xdr;
using Int64 = StellarDotnetSdk.Xdr.Int64;
using XdrSorobanAuthorizationEntry = StellarDotnetSdk.Xdr.SorobanAuthorizationEntry;
using SdkSorobanAuthorizationEntry = StellarDotnetSdk.Operations.SorobanAuthorizationEntry;

namespace Beamable.StellarFederation.Features.Stellar;

public class SorobanAuthSigner(StellarService stellarService) : IService
{
    public async Task<SdkSorobanAuthorizationEntry[]> SignAuthEntries(
        SdkSorobanAuthorizationEntry[] authEntries,
        Dictionary<string, KeyPair> signersByAddress,
        uint validUntilLedger)
    {
        var signedEntries = new List<SdkSorobanAuthorizationEntry>();

        foreach (var entry in authEntries)
        {
            var signedEntry = await SignAuthEntry(entry, signersByAddress, validUntilLedger);
            signedEntries.Add(signedEntry);
        }

        return signedEntries.ToArray();
    }

    private async Task<SdkSorobanAuthorizationEntry > SignAuthEntry(
        SdkSorobanAuthorizationEntry  entry,
        Dictionary<string, KeyPair> signersByAddress,
        uint validUntilLedger)
    {
        var xdrEntry = entry.ToXdr();
        // Only sign ADDRESS credentials, not SOURCE_ACCOUNT credentials
        if (xdrEntry.Credentials.Discriminant.InnerValue !=
            SorobanCredentialsType.SorobanCredentialsTypeEnum.SOROBAN_CREDENTIALS_ADDRESS)
        {
            return entry;
        }

        var addressCredentials = xdrEntry.Credentials.Address;

        // Get the address string from the credentials
        var addressString = GetAddressString(addressCredentials.Address);

        // Check if we have a signer for this address
        if (!signersByAddress.TryGetValue(addressString, out var keyPair))
        {
            throw new InvalidOperationException(
                $"No signer found for address {addressString}");
        }

        // Update the expiration ledger
        addressCredentials.SignatureExpirationLedger = new Uint32(validUntilLedger);

        // Build the preimage for signing
        var preimage = new HashIDPreimage
        {
            Discriminant = new EnvelopeType
            {
                InnerValue = EnvelopeType.EnvelopeTypeEnum.ENVELOPE_TYPE_SOROBAN_AUTHORIZATION
            },
            SorobanAuthorization = new HashIDPreimage.HashIDPreimageSorobanAuthorization
            {
                NetworkID = await stellarService.GetNetworkHash(),
                Nonce = addressCredentials.Nonce,
                SignatureExpirationLedger = new Uint32(validUntilLedger),
                Invocation = xdrEntry.RootInvocation
            }
        };

        // Hash the preimage with SHA-256
        var preimageBytes = SerializeToXdr(preimage);
        var hash = SHA256.HashData(preimageBytes);

        // Sign the hash with the keypair
        var signature = keyPair.Sign(hash);

        // Build the signature SCVal in the required format for Stellar accounts
        addressCredentials.Signature = BuildAccountSignature(keyPair.PublicKey, signature);

        return SdkSorobanAuthorizationEntry.FromXdr(xdrEntry);
    }

    private SCVal BuildAccountSignature(byte[] publicKey, byte[] signature)
    {
        // Create the AccountEd25519Signature structure
        var sigStruct = new SCVal
        {
            Discriminant = new SCValType { InnerValue = SCValType.SCValTypeEnum.SCV_MAP },
            Map = new SCMap([
                new SCMapEntry
                {
                    Key = new SCVal
                    {
                        Discriminant = new SCValType { InnerValue = SCValType.SCValTypeEnum.SCV_SYMBOL },
                        Sym = new SCSymbol("public_key")
                    },
                    Val = new SCVal
                    {
                        Discriminant = new SCValType { InnerValue = SCValType.SCValTypeEnum.SCV_BYTES },
                        Bytes = new SCBytes(publicKey)
                    }
                },
                new SCMapEntry
                {
                    Key = new SCVal
                    {
                        Discriminant = new SCValType { InnerValue = SCValType.SCValTypeEnum.SCV_SYMBOL },
                        Sym = new SCSymbol("signature")
                    },
                    Val = new SCVal
                    {
                        Discriminant = new SCValType { InnerValue = SCValType.SCValTypeEnum.SCV_BYTES },
                        Bytes = new SCBytes(signature)
                    }
                }
            ])
        };

        // Wrap in a Vec
        return new SCVal
        {
            Discriminant = new SCValType { InnerValue = SCValType.SCValTypeEnum.SCV_VEC },
            Vec = new SCVec([sigStruct])
        };
    }

    private byte[] SerializeToXdr(HashIDPreimage preimage)
    {
        var stream = new XdrDataOutputStream();
        HashIDPreimage.Encode(stream, preimage);
        return stream.ToArray();
    }

    private string GetAddressString(SCAddress address)
    {
        return address.Discriminant.InnerValue switch
        {
            SCAddressType.SCAddressTypeEnum.SC_ADDRESS_TYPE_ACCOUNT =>
                KeyPair.FromPublicKey(address.AccountId.InnerValue.Ed25519.InnerValue).AccountId,
            _ => throw new ArgumentException("Unknown address type")
        };
    }
}