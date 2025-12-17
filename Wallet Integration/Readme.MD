# Stellar Wallet Integration Bridge

A lightweight web application that serves as a bridge between Beamable games and external Stellar wallets. This app enables players to connect self-custody wallets (like Freighter) to authenticate and sign transactions for the Stellar Federation microservice.

## Overview

This bridge application is a critical component of the `StellarWeb3ExternalIdentity` implementation in the Stellar Federation microservice. It provides a secure, browser-based interface for players to:

- Connect their external Stellar wallets to the game
- Sign authentication challenges
- Sign blockchain transactions
- Maintain persistent wallet connections via localStorage

## How It Works

### Authentication Flow

1. **Game Initiates**: The game opens this web app in a browser with specific URL parameters
2. **Wallet Connection**: Player connects their Stellar wallet (e.g., Freighter browser extension)
3. **Challenge Signing**: If a message/transaction needs signing, the wallet prompts the player
4. **Callback**: Signed data is posted back to the Beamable microservice via API
5. **Session Persistence**: Connection details are stored in localStorage for future use

### Integration with Microservice

The microservice's `WalletConnectBridgeUrl` configuration setting points to this application's HTTPS URL. When external authentication is required, the microservice constructs a URL with all necessary parameters and redirects the player here.

## URL Parameters

The application expects the following query parameters:

| Parameter | Required | Description | Example |
|-----------|----------|-------------|---------|
| `network` | Yes | Stellar network ("testnet" or "public") | `testnet` |
| `cid` | Yes | Beamable Customer ID | `1234567890123` |
| `pid` | Yes | Beamable Project ID | `DE_dev` |
| `gamerTag` | Yes | Player's Beamable gamer tag | `player123` |
| `message` | No | Message or transaction XDR to sign | Base64-encoded string |

### Example URL

```
https://your-domain.com/?network=testnet&cid=1234567890123&pid=DE_dev&gamerTag=player123&message=AAAAAgAAAA...
```

## Setup & Development

### Prerequisites

- Node.js 16+ and npm
- A Stellar wallet browser extension (Freighter recommended)

### Installation

```bash
npm install
```

### Development Server

Run the development server with hot module replacement:

```bash
npm run dev
```

The app will be available at `http://localhost:5173` (or next available port).

### Production Build

Build the application for production:

```bash
npm run build
```

This creates optimized static files in the `dist/` directory.

## Deployment

### HTTPS Requirement

**CRITICAL**: This application MUST be hosted on an HTTPS domain. Wallet extensions require secure contexts for cryptographic operations.

### Deployment Options

#### Static Hosting Services
Deploy to any static hosting service that supports HTTPS:

- **Netlify**: Drag & drop the `dist/` folder or connect to Git
- **Vercel**: Import the repository and deploy with zero config
- **GitHub Pages**: Enable HTTPS in repository settings
- **Cloudflare Pages**: Connect repository for automatic deployments
- **AWS S3 + CloudFront**: Static hosting with CDN

#### Example: Netlify Deployment

```bash
# Build the project
npm run build

# Install Netlify CLI (one-time)
npm install -g netlify-cli

# Deploy
netlify deploy --prod --dir=dist
```

### Post-Deployment Configuration

After deploying, update the microservice configuration:

1. Navigate to [Beamable Portal](https://portal.beamable.com/login/)
2. Go to your project's Configuration page
3. Find the `stellar-federation` namespace
4. Set `WalletConnectBridgeUrl` to your deployed URL (e.g., `https://your-app.netlify.app`)

## Technical Details

### Technology Stack

| Technology | Purpose |
|------------|---------|
| TypeScript | Type-safe application code |
| Vite | Fast build tool and dev server |
| Stellar Wallets Kit | Wallet connection abstraction |
| Stellar SDK | Stellar blockchain interaction |
| Tailwind CSS | Responsive UI styling |

### Dependencies

```json
{
  "@creit.tech/stellar-wallets-kit": "^1.9.5",
  "stellar-sdk": "^13.3.0",
  "buffer": "^6.0.3"
}
```

### Supported Wallets

Currently supports:

- **Freighter**: Browser extension wallet for Stellar
- Extensible to other wallets via Stellar Wallets Kit modules

### API Endpoints

The app communicates with two Beamable microservice endpoints:

#### POST `/ExternalAddress`
Called when a wallet is first connected.

**Request Body:**
```json
{
  "address": "GCXYZ...ABC",
  "gamerTag": "player123"
}
```

#### POST `/ExternalSignature`
Called after a message or transaction is signed.

**Request Body:**
```json
{
  "address": "GCXYZ...ABC",
  "message": "original message or XDR",
  "signature": "signed result"
}
```

Both endpoints require:
- Header: `x-de-scope: {cid}.{pid}`
- Header: `Content-Type: application/json`

### Browser Compatibility

- Modern browsers with ES2020+ support
- Requires browser extension support for wallet integration
- LocalStorage must be enabled

## Security Considerations

### Private Key Management

This application **never** accesses or stores private keys. All cryptographic operations are handled by the connected wallet extension within its secure context.

### Session Persistence

Only the public address and wallet ID are stored in localStorage:
- `stellarAddress`: Public Stellar address (G-address)
- `walletId`: Wallet extension identifier

These values are non-sensitive and can be safely stored client-side.

### HTTPS Enforcement

Wallet extensions will refuse to operate in non-HTTPS contexts. Always deploy to secure domains in production.

### CORS Configuration

The Beamable API endpoints are configured to accept cross-origin requests from authorized domains. No additional CORS configuration is needed in this application.

## User Experience

### Connection Flow

1. Player is redirected from game to this web app
2. Clean, dark-themed UI presents "Connect Wallet" button
3. Clicking opens wallet selection modal
4. After selecting wallet, extension prompts for connection approval
5. If signing is required, extension prompts for signature approval
6. Success message displayed: "Connected. Please go back to the game."

### Error Handling

- **Missing Parameters**: App displays error and disables connection
- **Connection Failed**: User can retry connection
- **Signing Cancelled**: Error message displayed, player can try again
- **Network Mismatch**: Wallet extension handles network switching

### Disconnection

Players can disconnect their wallet using the "Disconnect" button, which:
- Closes the wallet connection
- Clears localStorage
- Resets UI to initial state

## Development Patterns

### Code Structure

```
src/
└── main.ts         # Main application logic
    ├── Wallet connection setup
    ├── Event handlers
    ├── UI updates
    └── API communication

index.html          # Entry point with Tailwind CSS
vite.config.ts      # Build configuration
```

### Key Functions

- `restoreWalletSession()`: Checks localStorage for existing connection
- `setupEventListeners()`: Handles connect/disconnect actions
- `signMessage()`: Signs authentication challenges
- `signTransaction()`: Signs blockchain transactions
- `postAddress()`: Notifies microservice of wallet connection
- `postSignature()`: Sends signed data to microservice

### Extending Wallet Support

To add support for additional wallets:

1. Install the wallet module via npm
2. Import the module in `main.ts`
3. Add to `modules` array in `StellarWalletsKit` initialization

Example:
```typescript
import { XBullModule, XBULL_ID } from '@creit.tech/stellar-wallets-kit';

const kit = new StellarWalletsKit({
  network: selectedNetwork(),
  selectedWalletId: FREIGHTER_ID,
  modules: [
    new FreighterModule(),
    new XBullModule()  // Add new wallet
  ]
});
```

## Troubleshooting

### "Missing required URL parameters"

Ensure all required parameters (`network`, `cid`, `pid`, `gamerTag`) are present in the URL. This typically indicates the microservice is misconfigured or the URL was manually modified.

### "Connection failed"

- Verify wallet extension is installed and enabled
- Check that wallet extension supports the specified network
- Ensure browser allows extension communication

### "Failed to sign message"

- Player may have cancelled the signing request
- Wallet may be locked and requires password
- Network mismatch between app and wallet settings

### CORS Errors

The Beamable API is configured for cross-origin requests. If you encounter CORS errors, verify:
- Request includes correct `x-de-scope` header
- CID and PID values are correct
- Request is from an HTTPS origin

## Testing

### Local Testing

For local testing with wallet extensions:

1. Run dev server: `npm run dev`
2. Construct test URL with valid parameters
3. Open in browser with wallet extension installed

**Note**: Some wallet extensions may require HTTPS even in development. Use a tool like [ngrok](https://ngrok.com/) or [local-ssl-proxy](https://www.npmjs.com/package/local-ssl-proxy) to test with HTTPS locally.

### Testing with Microservice

1. Deploy this app to HTTPS domain
2. Configure microservice `WalletConnectBridgeUrl` setting
3. In game, trigger external wallet authentication
4. Verify redirect to bridge app with correct parameters
5. Complete wallet connection and signing
6. Verify callback to microservice succeeds

## Related Documentation

- [Stellar Federation Microservice](../../Unity/BeamableServices/README.md)
- [Beamable Documentation](https://docs.beamable.com/)
- [Stellar Wallets Kit](https://github.com/Creit-Tech/Stellar-Wallets-Kit)
- [Freighter Wallet](https://www.freighter.app/)
- [Stellar Developer Docs](https://developers.stellar.org/)

