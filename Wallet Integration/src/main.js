import { Buffer } from 'buffer';
if (typeof window !== 'undefined') {
    window.Buffer = Buffer;
}
import { StellarWalletsKit, WalletNetwork, FreighterModule } from '@creit.tech/stellar-wallets-kit';
import { WalletConnectAllowedMethods, WalletConnectModule } from '@creit.tech/stellar-wallets-kit/modules/walletconnect.module';
// DOM elements
const connectBtn = document.getElementById('connect-btn');
const disconnectBtn = document.getElementById('disconnect-btn');
const statusText = document.getElementById('status-text');
const walletInfo = document.getElementById('wallet-info');
const publicKeyEl = document.getElementById('public-key');
const urlParams = new URLSearchParams(window.location.search);
const messageToSign = urlParams.get('message');
const network = urlParams.get('network');
const projectId = urlParams.get('projectId');
const cid = urlParams.get('cid');
const pid = urlParams.get('pid');
const gamerTag = urlParams.get('gamerTag');
const requiredParamsMissing = !network || !cid || !pid || !projectId || !gamerTag;
(async function main() {
    if (requiredParamsMissing) {
        disableAll();
        return;
    }
    const walletConnect = new WalletConnectModule({
        url: 'www.beamable.com',
        projectId,
        method: WalletConnectAllowedMethods.SIGN,
        description: `Stellar Wallet Connect Example Dapp`,
        name: 'Stellar Wallet Connect',
        icons: ['A LOGO/ICON TO SHOW TO YOUR USERS'],
        network: selectedNetwork()
    });
    const kit = new StellarWalletsKit({
        network: selectedNetwork(),
        modules: [new FreighterModule(), walletConnect]
    });
    await restoreWalletSession(kit);
    setupEventListeners(kit);
})();
function disableAll() {
    connectBtn.disabled = true;
    connectBtn.classList.add('hidden');
    statusText.textContent = 'Missing required URL parameters';
}
function selectedNetwork() {
    switch (network) {
        case 'public':
            return WalletNetwork.PUBLIC;
        case 'testnet':
            return WalletNetwork.TESTNET;
        default:
            return WalletNetwork.TESTNET;
    }
}
async function restoreWalletSession(kit) {
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
        await signTransaction(kit, messageToSign);
    }
}
function updateUi(publicKey) {
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
    }
    else {
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
async function signTransaction(kit, message) {
    try {
        const publicKey = await kit.getAddress();
        const { signedTxXdr } = await kit.signTransaction(message, {
            address: publicKey.address,
            networkPassphrase: selectedNetwork()
        });
        await postSignature(publicKey.address, message, signedTxXdr);
        statusText.textContent = 'Message signed successfully';
    }
    catch (error) {
        console.error('Signing error:', error);
        statusText.textContent = 'Failed to sign message';
    }
}
function setupEventListeners(kit) {
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
                    if (address)
                        await postAddress(address);
                    if (messageToSign)
                        await signTransaction(kit, messageToSign);
                },
                onClosed: () => updateUi(null)
            });
        }
        catch (err) {
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
async function postAddress(address) {
    try {
        await fetch(`https://api.beamable.com/basic/${cid}.${pid}.micro_StellarFederation/ExternalAddress`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "x-de-scope": `${cid}.${pid}`
            },
            body: JSON.stringify({ address: address, gamerTag: gamerTag })
        });
    }
    catch (error) {
        console.error('postSignature error:', error);
    }
}
async function postSignature(address, message, signature) {
    try {
        await fetch(`https://api.beamable.com/basic/${cid}.${pid}.micro_StellarFederation/ExternalSignature`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "x-de-scope": `${cid}.${pid}`
            },
            body: JSON.stringify({ address: address, message: message, signature: signature }),
        });
    }
    catch (error) {
        console.error('postSignature error:', error);
    }
}
