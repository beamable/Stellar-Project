using Beamable.StellarFederation.Features.Common;

namespace Beamable.StellarFederation.Features.WalletManager.Models;

public readonly record struct WalletCoinBalance(
    string Wallet,
    StellarAmount Balance);