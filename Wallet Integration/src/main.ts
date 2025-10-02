import { Buffer } from 'buffer';
window.Buffer = Buffer;

import {
  StellarWalletsKit,
  WalletNetwork,
  FreighterModule
} from '@creit.tech/stellar-wallets-kit';

import {
  WalletConnectAllowedMethods,
  WalletConnectModule,
  WALLET_CONNECT_ID
} from '@creit.tech/stellar-wallets-kit/modules/walletconnect.module';


// Get DOM elements
const connectBtn = document.getElementById('connect-btn') as HTMLButtonElement;
const disconnectBtn = document.getElementById('disconnect-btn') as HTMLButtonElement;
const statusText = document.getElementById('status-text') as HTMLParagraphElement;
const walletInfo = document.getElementById('wallet-info') as HTMLDivElement;
const publicKeyEl = document.getElementById('public-key') as HTMLParagraphElement;

const urlParams = new URLSearchParams(window.location.search);
const messageToSign = urlParams.get('message');
const network = urlParams.get("network");
const projectId = urlParams.get("projectId");
const cid = urlParams.get("cid");
const pid = urlParams.get("pid");

if (!messageToSign || !network || !cid  || !pid || !projectId) {
  disableAll();
} else {
  updateUi(null);  
}


const kit = new StellarWalletsKit({
  network: selectedNetwork(),
  selectedWalletId: WALLET_CONNECT_ID, 
  modules: [
    new FreighterModule(),
    new WalletConnectModule({
      url: 'www.beamable.com',
      projectId: projectId!,
      method: WalletConnectAllowedMethods.SIGN,
      description: `Stellar Wallet Connect Example Dapp`,
      name: 'Stellar Wallet Connect',
      icons: ['A LOGO/ICON TO SHOW TO YOUR USERS'],
      network: selectedNetwork(),
    }),
  ],
});



function disableAll() {
  connectBtn.disabled = true;
  connectBtn.classList.add('hidden');
  statusText.textContent = 'Input data missing';
}

function selectedNetwork(): WalletNetwork {
    switch (network) {
        case "public":
            return WalletNetwork.PUBLIC;
        case "testnet":
            return WalletNetwork.TESTNET;
    }
    return WalletNetwork.TESTNET;
}


function updateUi(publicKey: string | null) {
  if (publicKey) {
    // Wallet is connected
    walletInfo.classList.remove('hidden');
    statusText.classList.add('hidden');
    connectBtn.classList.add('hidden');
    disconnectBtn.classList.remove('hidden');

    // Display the public key (shortened for readability)
    const shortenedKey = `${publicKey.substring(0, 8)}...${publicKey.substring(publicKey.length - 8)}`;
    publicKeyEl.textContent = shortenedKey;
    publicKeyEl.title = publicKey; // Show full key on hover

  } else {
    // Wallet is disconnected
    connectBtn.disabled = false;
    statusText.textContent = 'Not Connected';
    walletInfo.classList.add('hidden');
    statusText.classList.remove('hidden');
    connectBtn.classList.remove('hidden');
    disconnectBtn.classList.add('hidden');

    publicKeyEl.textContent = '';
    publicKeyEl.title = '';
  }
}

// Connect button event listener
connectBtn.addEventListener('click', async () => {
  try {
    statusText.textContent = 'Connecting...';
    connectBtn.disabled = true;
    
    await kit.openModal({
      onWalletSelected: async (option) => {
        kit.setWallet(option.id);
        const { address } = await kit.getAddress();
        
        // Update UI
        publicKeyEl.textContent = address;
        updateUi(address);

        if (messageToSign) {
          await signTransaction(messageToSign);
        }
      },
      onClosed: () => {
        updateUi(null);  
      }      
    });
    
  } catch (error) {
    console.error('Connection error:', error);
    statusText.textContent = 'Connection failed. Please try again.';
    connectBtn.disabled = false;
  }
});

// Disconnect button event listener
disconnectBtn.addEventListener('click', async () => {
  await kit.disconnect();  
  updateUi(null);
  console.log('Wallet disconnected');
});

async function signMessage(message: string) {
  try {
    statusText.classList.remove('hidden');
    statusText.textContent = 'Signing message...';
    
    const { signedMessage } = await kit.signMessage(message);
    
    console.log('Message signed:', signedMessage);
    statusText.textContent = `Message signed successfully!`;
    
  } catch (error) {
    console.error('Signing error:', error);
    statusText.textContent = 'Failed to sign message';
  }
}

async function signTransaction(message: string) {
  try {
    const publicKey = await kit.getAddress();
    const { signedTxXdr } = await kit.signTransaction(message, {
        address: publicKey.address,
        networkPassphrase: selectedNetwork()
    });
    await postSignature(message, signedTxXdr);
    
  } catch (error) {
    console.error('Signing error:', error);
    statusText.textContent = 'Failed to sign message';
  }
}

async function postSignature(message: string, signature: string) {
  try {
    await fetch(`https://api.beamable.com/basic/${cid}.${pid}.micro_StellarFederation/ExternalSignature`, {
        method: "POST",
        headers: {
        "Content-Type": "application/json",
        "x-de-scope": `${cid}.${pid}`
        },
        body: JSON.stringify({ message: message, signature: signature }),
  });
    
  } catch (error) {
    console.error('postSignature error:', error);
  }
}


console.log('Stellar Wallet app loaded!');