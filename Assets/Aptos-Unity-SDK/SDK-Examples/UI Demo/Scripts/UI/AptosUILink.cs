using System;
using System.Collections;
using System.Collections.Generic;
using Aptos.HdWallet;
using NBitcoin;
using UnityEngine;
using Aptos.Unity.Rest;
using Aptos.Unity.Rest.Model;
using UnityEngine.UI;
using Aptos.Accounts;
using Newtonsoft.Json;

namespace Aptos.Unity.Sample.UI
{
    public class AptosUILink : MonoBehaviour
    {
        static public AptosUILink Instance { get; set; }

        [HideInInspector]
        public string mnemonicsKey = "MnemonicsKey";
        [HideInInspector]
        public string privateKey = "PrivateKey";
        [HideInInspector]
        public string currentAddressIndexKey = "CurrentAddressIndexKey";

        [SerializeField] private int accountNumLimit = 10;
        public List<string> addressList;

        public event Action<float> onGetBalance;

        private Wallet wallet;
        private string faucetEndpoint = "https://faucet.devnet.aptoslabs.com";

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {

        }

        void Update()
        {

        }

        public void InitWalletFromCache()
        {
            wallet = new Wallet(PlayerPrefs.GetString(mnemonicsKey));
            GetWalletAddress();
            LoadCurrentWalletBalance();
        }

        public bool CreateNewWallet()
        {
            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            wallet = new Wallet(mnemo);

            PlayerPrefs.SetString(mnemonicsKey, mnemo.ToString());
            PlayerPrefs.SetInt(currentAddressIndexKey, 0);

            GetWalletAddress();
            LoadCurrentWalletBalance();

            if (mnemo.ToString() != string.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RestoreWallet(string _mnemo)
        {
            try
            {
                wallet = new Wallet(_mnemo);
                PlayerPrefs.SetString(mnemonicsKey, _mnemo);
                PlayerPrefs.SetInt(currentAddressIndexKey, 0);

                GetWalletAddress();
                LoadCurrentWalletBalance();

                return true;
            }
            catch
            {

            }

            return false;
        }

        public List<string> GetWalletAddress()
        {
            addressList = new List<string>();

            for (int i = 0; i < accountNumLimit; i++)
            {
                var account = wallet.GetAccount(i);
                var addr = account.AccountAddress.ToString();

                addressList.Add(addr);
            }

            return addressList;
        }

        public string GetCurrentWalletAddress()
        {
            return addressList[PlayerPrefs.GetInt(currentAddressIndexKey)];
        }

        public string GetPrivateKey()
        {
            return wallet.Account.PrivateKey;
        }

        public void LoadCurrentWalletBalance()
        {
            AccountResourceCoin.Coin coin = new AccountResourceCoin.Coin();
            ResponseInfo responseInfo = new ResponseInfo();

            StartCoroutine(RestClient.Instance.GetAccountBalance((_coin, _responseInfo) =>
            {
                coin = _coin;
                responseInfo = _responseInfo;

                if(responseInfo.status != ResponseInfo.Status.Success)
                {
                    onGetBalance?.Invoke(0.0f);
                }
                else
                {
                    onGetBalance?.Invoke(float.Parse(coin.Value));
                }

            }, wallet.GetAccount(PlayerPrefs.GetInt(currentAddressIndexKey)).AccountAddress));
        }

        public IEnumerator AirDrop(int _amount)
        {
            Coroutine cor = StartCoroutine(FaucetClient.Instance.FundAccount((success, returnResult) =>
            {
            }, wallet.GetAccount(PlayerPrefs.GetInt(currentAddressIndexKey)).AccountAddress.ToString()
                , _amount
                , faucetEndpoint));

            yield return cor;

            yield return new WaitForSeconds(1f);
            LoadCurrentWalletBalance();
            UIController.Instance.ToggleNotification(ResponseInfo.Status.Success, "Successfully Get Airdrop of " + AptosTokenToFloat((float)_amount) + " APT");
        }

        public IEnumerator SendToken(string _targetAddress, long _amount)
        {
            Rest.Model.Transaction transferTxn = new Rest.Model.Transaction();
            ResponseInfo responseInfo = new ResponseInfo();
            Coroutine transferCor = StartCoroutine(RestClient.Instance.Transfer((_transferTxn, _responseInfo) =>
            {
                transferTxn = _transferTxn;
                responseInfo = _responseInfo;
            }, wallet.GetAccount(PlayerPrefs.GetInt(currentAddressIndexKey)), _targetAddress, _amount));

            yield return transferCor;

            if (responseInfo.status == ResponseInfo.Status.Success)
            {
                string transactionHash = transferTxn.Hash;
                bool waitForTxnSuccess = false;
                Coroutine waitForTransactionCor = StartCoroutine(
                    RestClient.Instance.WaitForTransaction((_pending, _responseInfo) =>
                    {
                        waitForTxnSuccess = _pending;
                        responseInfo = _responseInfo;
                    }, transactionHash)
                );
                yield return waitForTransactionCor;

                if (waitForTxnSuccess)
                {
                    UIController.Instance.ToggleNotification(ResponseInfo.Status.Success, "Successfully send " + AptosTokenToFloat((float)_amount) + " APT to " + UIController.Instance.ShortenString(_targetAddress, 4));
                }
                else
                {
                    UIController.Instance.ToggleNotification(ResponseInfo.Status.Failed, "Send Token Transaction Failed");
                }
                
            }
            else
            {
                UIController.Instance.ToggleNotification(ResponseInfo.Status.Failed, responseInfo.message);
            }

            yield return new WaitForSeconds(1f);
            LoadCurrentWalletBalance();
        }

        public IEnumerator CreateCollection(string _collectionName, string _collectionDescription, string _collectionUri)
        {
            Rest.Model.Transaction createCollectionTxn = new Rest.Model.Transaction();
            ResponseInfo responseInfo = new ResponseInfo();
            Coroutine createCollectionCor = StartCoroutine(RestClient.Instance.CreateCollection((_createCollectionTxn, _responseInfo) =>
            {
                createCollectionTxn = _createCollectionTxn;
                responseInfo = _responseInfo;
            }, wallet.GetAccount(PlayerPrefs.GetInt(currentAddressIndexKey)),
            _collectionName, _collectionDescription, _collectionUri));
            yield return createCollectionCor;

            if (responseInfo.status == ResponseInfo.Status.Success)
            {
                UIController.Instance.ToggleNotification(ResponseInfo.Status.Success, "Successfully Create Collection: " + _collectionName);
            }
            else
            {
                UIController.Instance.ToggleNotification(ResponseInfo.Status.Failed, responseInfo.message);
            }

            yield return new WaitForSeconds(1f);
            LoadCurrentWalletBalance();

            string transactionHash = createCollectionTxn.Hash;
        }

        public IEnumerator CreateNFT(string _collectionName, string _tokenName, string _tokenDescription, int _supply, int _max, string _uri, int _royaltyPointsPerMillion)
        {
            Rest.Model.Transaction createTokenTxn = new Rest.Model.Transaction();
            ResponseInfo responseInfo = new ResponseInfo();
            Coroutine createTokenCor = StartCoroutine(
                RestClient.Instance.CreateToken((_createTokenTxn, _responseInfo) =>
                {
                    createTokenTxn = _createTokenTxn;
                    responseInfo = _responseInfo;
                }, wallet.GetAccount(PlayerPrefs.GetInt(currentAddressIndexKey)),
                _collectionName,
                _tokenName,
                _tokenDescription,
                _supply,
                _max,
                _uri,
                _royaltyPointsPerMillion)
            );
            yield return createTokenCor;

            if (responseInfo.status == ResponseInfo.Status.Success)
            {
                UIController.Instance.ToggleNotification(ResponseInfo.Status.Success, "Successfully Create NFT: " + _tokenName);
            }
            else
            {
                UIController.Instance.ToggleNotification(ResponseInfo.Status.Failed, responseInfo.message);
            }

            yield return new WaitForSeconds(1f);
            LoadCurrentWalletBalance();

            string createTokenTxnHash = createTokenTxn.Hash;
        }

        string mnemo = "voyage chalk social search pair zone husband mix lonely gather cherry beach";
        Mnemonic mnemo1 = new Mnemonic(Wordlist.English, WordCount.Twelve);

        public Button addAccountTab;
        public Button accountTab;
        public Button sendTransactionTab;
        public Button gameShopTab;
        public Button playGameTab;

        public Button buy200Button;
        public Button buy500Button;
        public Button buy1500Button;
        public Button buy5000Button;

        public TMPro.TextMeshProUGUI buyingStatusText;
        public TMPro.TextMeshProUGUI totalGoldBoughtText;

        public IEnumerator BuyGameMoney(int index)
        {
            float AccountBalance = UIController.Instance.aptosBalance;
            if (AccountBalance < 0.5f)
            {
                Debug.Log("Not enough Aptos");
                buyingStatusText.text = "Get more Aptos to claim";
                buyingStatusText.gameObject.SetActive(true);
                yield break;
            }

            buyingStatusText.text = "Buying Coin!";
            buyingStatusText.gameObject.SetActive(true);

            buy200Button.interactable = false;
            buy500Button.interactable = false;
            buy1500Button.interactable = false;
            buy5000Button.interactable = false;

            addAccountTab.interactable = false;
            accountTab.interactable = false;
            sendTransactionTab.interactable = false;
            playGameTab.interactable = false;
            gameShopTab.interactable = false;

            #region REST Client Setup
            Debug.Log("<color=cyan>=== =========================== ===</color>");
            Debug.Log("<color=cyan>=== Set Up REST Client ===</color>");
            Debug.Log("<color=cyan>=== =========================== ===</color>");

            RestClient restClient = RestClient.Instance.SetEndPoint(Constants.TESTNET_BASE_URL);
            Coroutine restClientSetupCor = StartCoroutine(RestClient.Instance.SetUp());
            yield return restClientSetupCor;
            #endregion

            Wallet wallet = new Wallet(PlayerPrefs.GetString(AptosUILink.Instance.mnemonicsKey));

            #region Alice Account
            Debug.Log("<color=cyan>=== ========= ===</color>");
            Debug.Log("<color=cyan>=== Addresses ===</color>");
            Debug.Log("<color=cyan>=== ========= ===</color>");
            Account alice = wallet.GetAccount(0);
            string authKey = alice.AuthKey();
            //Debug.Log("Alice Auth Key: " + authKey);

            AccountAddress aliceAddress = alice.AccountAddress;
            Debug.Log("Alice's Account Address: " + aliceAddress.ToString());

            PrivateKey privateKey = alice.PrivateKey;
            //Debug.Log("Aice Private Key: " + privateKey);
            #endregion

            Wallet wallet1 = new Wallet(mnemo);

            #region Bob Account
            Account bob = wallet1.GetAccount(0);
            AccountAddress bobAddress = bob.AccountAddress;
            Debug.Log("Bob's Account Address: " + bobAddress.ToString());

            Debug.Log("Wallet: Account 0: Alice: " + aliceAddress.ToString());
            Debug.Log("Wallet: Account 1: Bob: " + bobAddress.ToString());
            #endregion

            Debug.Log("<color=cyan>=== ================ ===</color>");
            Debug.Log("<color=cyan>=== Initial Balances ===</color>");
            Debug.Log("<color=cyan>=== ================ ===</color>");

            #region Get Alice Account Balance
            ResponseInfo responseInfo = new ResponseInfo();
            AccountResourceCoin.Coin coin = new AccountResourceCoin.Coin();
            responseInfo = new ResponseInfo();
            Coroutine getAliceBalanceCor1 = StartCoroutine(RestClient.Instance.GetAccountBalance((_coin, _responseInfo) =>
            {
                coin = _coin;
                responseInfo = _responseInfo;

            }, aliceAddress));
            yield return getAliceBalanceCor1;

            if (responseInfo.status == ResponseInfo.Status.Failed)
            {
                Debug.LogError(responseInfo.message);
                yield break;
            }

            Debug.Log("Alice's Balance Before Funding: " + coin.Value);
            #endregion

            #region Have Alice give Bob APT coins - Submit Transfer Transaction
            int APTValue = 1000000;

            if (index == 1) {
                APTValue = 1000000;
            }
            else if (index == 2) {
                APTValue = 2000000;
            }
            else if (index == 3)
            {
                APTValue = 4000000;
            }
            else if (index == 4)
            {
                APTValue = 8000000;
            }

            Rest.Model.Transaction transferTxn = new Rest.Model.Transaction();
            Coroutine transferCor = StartCoroutine(RestClient.Instance.Transfer((_transaction, _responseInfo) =>
            {
                transferTxn = _transaction;
                responseInfo = _responseInfo;
            }, alice, bob.AccountAddress.ToString(), APTValue));

            yield return transferCor;

            if (responseInfo.status != ResponseInfo.Status.Success)
            {
                Debug.LogWarning("Transfer failed: " + responseInfo.message);
                yield break;
            }

            Debug.Log("Transfer Response: " + responseInfo.message);
            string transactionHash = transferTxn.Hash;
            Debug.Log("Transfer Response Hash: " + transferTxn.Hash);
            #endregion

            #region Wait For Transaction
            bool waitForTxnSuccess = false;
            Coroutine waitForTransactionCor = StartCoroutine(
                RestClient.Instance.WaitForTransaction((_pending, _responseInfo) =>
                {
                    waitForTxnSuccess = _pending;
                    responseInfo = _responseInfo;
                }, transactionHash)
            );
            yield return waitForTransactionCor;

            if (!waitForTxnSuccess)
            {
                Debug.LogWarning("Transaction was not found. Breaking out of example", gameObject);
                yield break;
            }

            #endregion

            Debug.Log("<color=cyan>=== ===================== ===</color>");
            Debug.Log("<color=cyan>=== Intermediate Balances ===</color>");
            Debug.Log("<color=cyan>=== ===================== ===</color>");

            #region Get Alice Account Balance After Transfer
            Coroutine getAliceAccountBalance3 = StartCoroutine(RestClient.Instance.GetAccountBalance((_coin, _responseInfo) =>
            {
                coin = _coin;
                responseInfo = _responseInfo;
            }, aliceAddress));
            yield return getAliceAccountBalance3;

            if (responseInfo.status == ResponseInfo.Status.Failed)
            {
                Debug.LogError(responseInfo.message);
                yield break;
            }

            Debug.Log("Alice Balance After Transfer: " + coin.Value);
            #endregion

            #region Get Bob Account Balance After Transfer
            Coroutine getBobAccountBalance2 = StartCoroutine(RestClient.Instance.GetAccountBalance((_coin, _responseInfo) =>
            {
                coin = _coin;
                responseInfo = _responseInfo;
            }, bobAddress));
            yield return getBobAccountBalance2;

            if (responseInfo.status == ResponseInfo.Status.Failed)
            {
                Debug.LogError(responseInfo.message);
                yield break;
            }

            Debug.Log("Bob Balance After Transfer: " + coin.Value);

            #endregion

            LoadCurrentWalletBalance();

            buy200Button.interactable = true;
            buy500Button.interactable = true;
            buy1500Button.interactable = true;
            buy5000Button.interactable = true;

            addAccountTab.interactable = true;
            accountTab.interactable = true;
            sendTransactionTab.interactable = true;
            playGameTab.interactable = true;
            gameShopTab.interactable = true;

            if (index == 1)
            {
                ResourceBoost.Instance.golds += 200;
            }
            else if (index == 2)
            {
                ResourceBoost.Instance.golds += 500;
            }
            else if (index == 3)
            {
                ResourceBoost.Instance.golds += 1500;
            }
            else if (index == 4)
            {
                ResourceBoost.Instance.golds += 5000;
            }

            totalGoldBoughtText.text = "Total Money Bought: " + ResourceBoost.Instance.golds;

            buyingStatusText.text = "Purchase Completed";
            buyingStatusText.gameObject.SetActive(true);
            yield return null;
        }

        IEnumerator WaitForTransaction(string txnHash)
        {
            Coroutine waitForTransactionCor = StartCoroutine(
                RestClient.Instance.WaitForTransaction((pending, _responseInfo) =>
                {
                    Debug.Log(_responseInfo.message);
                }, txnHash)
            );
            yield return waitForTransactionCor;
        }

        public float AptosTokenToFloat(float _token)
        {
            return _token / 100000000f;
        }

        public long AptosFloatToToken(float _amount)
        {
            return Convert.ToInt64(_amount * 100000000);
        }
    }
}