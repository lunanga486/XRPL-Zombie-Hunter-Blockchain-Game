//using Solana.Unity.Metaplex.NFT.Library;
//using Solana.Unity.Metaplex.Utilities;
//using Solana.Unity.Programs;
//using Solana.Unity.Rpc.Builders;
//using Solana.Unity.Rpc.Core.Http;
//using Solana.Unity.Rpc.Models;
//using Solana.Unity.Rpc.Types;
//using Solana.Unity.SDK;
//using Solana.Unity.SDK.Nft;
//using Solana.Unity.Wallet;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class SolanaMainMenuManager : MonoBehaviour
//{
//    //Game Shop
//    public Button buy200Button;
//    public Button buy500Button;
//    public Button buy1500Button;
//    public Button buy5000Button;
//    public Button backBtn;


//    public TextMeshProUGUI totalGoldBoughtText;
//    public TextMeshProUGUI buyingStatusText;

//    private const long SolLamports = 1000000000;

//    private void HandleResponse(RequestResult<string> result)
//    {
//        Debug.Log(result.Result == null ? result.Reason : "");
//    }

//    public async void SpendTokenToBuyCoins(int indexValue)
//    {

//        Double _ownedSolAmount = await Web3.Instance.WalletBase.GetBalance();

//        if (_ownedSolAmount <= 0)
//        {
//            buyingStatusText.text = "Not Enough SOL";
//            return;
//        }

//        TurnOffButtons();

//        float costValue = 0f;
//        buyingStatusText.text = "Buying...";
//        buyingStatusText.gameObject.SetActive(true);
//        if (indexValue == 0)
//        {
//            costValue = 0.005f;
//        }
//        else if (indexValue == 1)
//        {
//            costValue = 0.01f;
//        }
//        else if (indexValue == 2)
//        {
//            costValue = 0.02f;
//        }
//        else if (indexValue == 3)
//        {
//            costValue = 0.04f;
//        }
//        try
//        {
//            RequestResult<string> result = await Web3.Instance.WalletBase.Transfer(
//               new PublicKey("Hw1VoYsnB7kX5h4nZiczEndj6mMF3i7DZR5Q2Ng1JiM4"),
//               Convert.ToUInt64(costValue * SolLamports));
//            HandleResponse(result);

//            TurnOnButtons();


//            if (indexValue == 0)
//            {
//                buyingStatusText.text = "+200 Golds";
//                ResourceBoost.Instance.golds += 200;
//            }
//            else if (indexValue == 1)
//            {
//                buyingStatusText.text = "+500 Golds";
//                ResourceBoost.Instance.golds += 500;
//            }
//            else if (indexValue == 2)
//            {
//                buyingStatusText.text = "+1,500 Golds";
//                ResourceBoost.Instance.golds += 1500;
//            }
//            else if (indexValue == 3)
//            {
//                buyingStatusText.text = "+5,000 Golds";
//                ResourceBoost.Instance.golds += 5000;
//            }

//            totalGoldBoughtText.text = "Gold Bought: " + ResourceBoost.Instance.golds.ToString();

//        }
//        catch (Exception ex)
//        {
//            Debug.LogError($"Lỗi khi thực hiện chuyển tiền: {ex.Message}");
//        }
//    }

//    private void TurnOffButtons()
//    {
//        buy200Button.interactable = false;
//        buy500Button.interactable = false;
//        buy1500Button.interactable = false;
//        buy5000Button.interactable = false;

//        backBtn.interactable = false;
//    }

//    private void TurnOnButtons()
//    {
//        buy200Button.interactable = true;
//        buy500Button.interactable = true;
//        buy1500Button.interactable = true;
//        buy5000Button.interactable = true;

//        backBtn.interactable = true;
//    }


//    public void PlayGame()
//    {
//        SceneManager.LoadScene("Logo");
//    }


//}
