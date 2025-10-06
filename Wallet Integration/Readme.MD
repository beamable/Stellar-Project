## Running Wallet-integration project

The following will give you a glimpse of the Stellar wallet integration project.

```shell
npm install
npm run dev
npm run build
```

## Project hosting

The built project should be hosted on an HTTPS domain to ensure secure communication with Wallet Connect.


## Usage

Clients requests a wallet connection by providing following inputs:
- message: transaction to sign
- network: Stellar network (testnet/public)
- projectId: Wallet Connect projectId obtainable [here](https://reown.com/)
- cid: Beamable customer ID
- pid: Beamable project ID


