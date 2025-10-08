import { Buffer } from 'buffer';
if (typeof window !== 'undefined') {
  (window as any).Buffer = Buffer;
}

import {
  StellarWalletsKit,
  WalletNetwork,
  FreighterModule,
  FREIGHTER_ID
} from '@creit.tech/stellar-wallets-kit';


// DOM elements
const connectBtn = document.getElementById('connect-btn') as HTMLButtonElement;
const disconnectBtn = document.getElementById('disconnect-btn') as HTMLButtonElement;
const statusText = document.getElementById('status-text') as HTMLParagraphElement;
const walletInfo = document.getElementById('wallet-info') as HTMLDivElement;
const publicKeyEl = document.getElementById('public-key') as HTMLParagraphElement;

const urlParams = new URLSearchParams(window.location.search);
const messageToSign = urlParams.get('message');
const network = urlParams.get('network');
const cid = urlParams.get('cid');
const pid = urlParams.get('pid');
const gamerTag = urlParams.get('gamerTag');
const requiredParamsMissing = !network || !cid || !pid || !gamerTag;

(async function main() {
  if (requiredParamsMissing) {
    disableAll();
    return;
  }
  

  const kit = new StellarWalletsKit({
    network: selectedNetwork(),
    selectedWalletId: FREIGHTER_ID,
    modules: [new FreighterModule()]
  });

  await restoreWalletSession(kit);
  setupEventListeners(kit);
})();


function disableAll() {
  connectBtn.disabled = true;
  connectBtn.classList.add('hidden');
  statusText.textContent = 'Missing required URL parameters';
}

function selectedNetwork(): WalletNetwork {
  switch (network) {
    case 'public':
      return WalletNetwork.PUBLIC;
    case 'testnet':
      return WalletNetwork.TESTNET;
    default:
      return WalletNetwork.TESTNET;
  }
}

async function restoreWalletSession(kit: StellarWalletsKit) {
  if (requiredParamsMissing) {
    return;
  }
  const savedAddress = localStorage.getItem('stellarAddress');
  const walletId = localStorage.getItem('walletId');

  if (!savedAddress || !walletId) {
    updateUi(null);
    return;
  }  

  updateUi(savedAddress);
  if (messageToSign && walletId) {
    kit.setWallet(walletId);
    await signMessage(kit, messageToSign);
  }
}


function updateUi(publicKey: string | null) {
  if (publicKey) {
    // Wallet is connected
    walletInfo.classList.remove('hidden');
    statusText.innerHTML = 'Connected.<br>Please go back to the game.';
    connectBtn.classList.add('hidden');
    disconnectBtn.classList.remove('hidden');

    // Display the public key (shortened for readability)
    const shortenedKey = `${publicKey.substring(0, 8)}...${publicKey.substring(publicKey.length - 8)}`;
    publicKeyEl.textContent = shortenedKey;
    publicKeyEl.title = publicKey; // Show full key on hover

  } else {
    // Wallet is disconnected
    console.log("Wallet disconnected, updating UI");
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

async function signTransaction(kit: StellarWalletsKit, message: string) {
  try {
    const publicKey = await kit.getAddress();
    const { signedTxXdr } = await kit.signTransaction(message, {
        address: publicKey.address,
        networkPassphrase: selectedNetwork()
    });
    await postSignature(publicKey.address, message, signedTxXdr);
    statusText.textContent = 'Message signed successfully';
    
  } catch (error) {
    console.error('Signing error:', error);
    statusText.textContent = 'Failed to sign message';
  }
}

async function signMessage(kit: StellarWalletsKit, message: string) {
  try {
    const publicKey = await kit.getAddress();
    const { signedMessage } = await kit.signMessage(message, {
        address: publicKey.address,
        networkPassphrase: selectedNetwork()
    });
    await postSignature(publicKey.address, message, signedMessage);
    statusText.innerHTML = 'Message signed successfully.<br>Please go back to the game.';
    
  } catch (error) {
    console.error('Signing error:', error);
    statusText.textContent = 'Failed to sign message';
  }
}

function setupEventListeners(kit: StellarWalletsKit) {
  connectBtn.addEventListener('click', async () => {
    try {
      statusText.textContent = 'Connecting...';
      connectBtn.disabled = true;

      await kit.openModal({
        onWalletSelected: async (option) => {
          kit.setWallet(option.id);
          const { address } = await kit.getAddress();
          updateUi(address);

          localStorage.setItem('stellarAddress', address);
          localStorage.setItem('walletId', option.id);

          
          if (address) await postAddress(address);
          if (messageToSign) await signMessage(kit, messageToSign);
        },
        onClosed: () => updateUi(null)
      });
    } catch (err) {
      console.error(err);
      statusText.textContent = 'Connection failed.';
      connectBtn.disabled = false;
    }
  });

  disconnectBtn.addEventListener('click', async () => {
    await kit.disconnect();
    updateUi(null);
    localStorage.removeItem('stellarAddress');
    localStorage.removeItem('walletId');
  });
}

async function postAddress(address: string) {
  try {
    await fetch(`https://api.beamable.com/basic/${cid}.${pid}.micro_StellarFederation/ExternalAddress`, {
        method: "POST",
        headers: {
        "Content-Type": "application/json",
        "x-de-scope": `${cid}.${pid}`
        },
        body: JSON.stringify({ address: address, gamerTag: gamerTag })
  });
    
  } catch (error) {
    console.error('postSignature error:', error);
  }
}

async function postSignature(address: string, message: string, signature: string) {
  try {
    await fetch(`https://api.beamable.com/basic/${cid}.${pid}.micro_StellarFederation/ExternalSignature`, {
        method: "POST",
        headers: {
        "Content-Type": "application/json",
        "x-de-scope": `${cid}.${pid}`
        },
        body: JSON.stringify({ address: address, message: message, signature: signature }),
  });
    
  } catch (error) {
    console.error('postSignature error:', error);
  }
}
