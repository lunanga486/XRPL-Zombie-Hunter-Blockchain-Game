using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thirdweb;
using UnityEngine.UI;
using TMPro;
using System;
using Amazon.Runtime;
using Org.BouncyCastle.Bcpg;
using UnityEngine.SceneManagement;

public class BlockchainManager : MonoBehaviour
{
    public string Address { get; private set; }
    public Button luckyNumberButton;

    private string smartContract = "0x4Fe1C926497540676ce0f42453EBDa9eF007a67a";
    private string abiString = "[{\"inputs\":[],\"name\":\"generateNumber\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_user\",\"type\":\"address\"}],\"name\":\"getGold\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_user\",\"type\":\"address\"}],\"name\":\"getPass\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_gold\",\"type\":\"uint256\"}],\"name\":\"updateGold\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_pass\",\"type\":\"string\"}],\"name\":\"updatePass\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI countdownStatusText;
    public TextMeshProUGUI randomNumberText;
    private float lastClickTime = 0f;
    private const float clickCooldown = 20f;
    private Coroutine countdownCoroutine;
    private bool isCountingDown = false; // Biến cờ để kiểm tra đếm ngược đã bắt đầu chưa

    public TextMeshProUGUI text1; // Tham chiếu tới UI Text thứ nhất
    public TextMeshProUGUI text2; // Tham chiếu tới UI Text thứ hai
    public TextMeshProUGUI text3; // Tham chiếu tới UI Text thứ ba

    public TextMeshProUGUI resultText; // Tham chiếu tới UI Text thứ ba

    public TextMeshProUGUI totalCoinsOwnedText;

    public Button number1Button;
    public Button number2Button;
    public Button number3Button;

    private int correctValue = 0;

    private string receiveAddress = "0xA24d7ECD79B25CE6C66f1Db9e06b66Bd11632E00";

    public Button gold100Button;
    public Button gold300Button;
    public Button gold1000Button;
    public Button gold3000Button;
    public Button backButton;

    public TextMeshProUGUI buyingStatusText;
    public TextMeshProUGUI totalCoinBoughtValueText;


    private void Start()
    {
        HideAllNumberButton();
        resultText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        //Dùng singleton hiển thị tổng coins đã mua
        //Singleton
        totalCoinsOwnedText.text = "Singleton";
        buyingStatusText.gameObject.SetActive(false);
        ResourceBoost.Instance.golds = 0;
    }

    private void Update()
    {
        totalCoinBoughtValueText.text = ResourceBoost.Instance.golds.ToString();
        totalCoinsOwnedText.text = ResourceBoost.Instance.golds.ToString();
    }

    private void HideAllNumberButton() {
        number1Button.interactable = false;
        number2Button.interactable = false;
        number3Button.interactable = false;
    }
    private void ShowAllNumberButton()
    {
        number1Button.interactable = true;
        number2Button.interactable = true;
        number3Button.interactable = true;
    }
    private void HideAllShopButton()
    {
        gold100Button.interactable = false;
        gold300Button.interactable = false;
        gold1000Button.interactable = false;
        gold3000Button.interactable = false;
        backButton.interactable = false;
    }
    private void ShowAllShopButton()
    {
        gold100Button.interactable = true;
        gold300Button.interactable = true;
        gold1000Button.interactable = true;
        gold3000Button.interactable = true;
        backButton.interactable = true;
    }

    public async void CreateRandomNumberOnOasis()
    {


        if (!isCountingDown) // Nếu đếm ngược chưa bắt đầu
        {
            isCountingDown = true; // Bắt đầu đếm ngược
            lastClickTime = Time.time; // Cập nhật thời gian nhấp đầu tiên

            //trừ token ở đây
            try
            {
                // Attempt to transfer tokens
                await ThirdwebManager.Instance.SDK.Wallet.Transfer(receiveAddress, "1");
                Debug.Log("Transfer successful.");

                Address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
                var contract = ThirdwebManager.Instance.SDK.GetContract(
                    smartContract,
                    abiString
                );
                try
                {
                    int generatedNumber = await contract.Read<int>("generateNumber");
                    correctValue = generatedNumber;
                    randomNumberText.text = generatedNumber.ToString();
                    Debug.Log("generatedNumber: " + generatedNumber);

                    GenerateAndDisplayNumbers(generatedNumber);
                    ShowAllNumberButton();

                    // Bắt đầu đếm ngược lại từ đầu
                    if (countdownCoroutine != null)
                    {
                        StopCoroutine(countdownCoroutine);
                        countdownCoroutine = null;
                    }
                    countdownCoroutine = StartCoroutine(StartCountdown());
                }
                catch (Exception ex)
                {
                    Debug.LogError("Đã xảy ra lỗi: " + ex.Message);
                    countdownStatusText.text = "Đã xảy ra lỗi: " + ex.Message;
                    isCountingDown = false; // Cho phép thử lại nếu có lỗi
                }

            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the transfer
                Debug.LogError($"Transfer failed: {ex.Message}");
                isCountingDown = false; // Cho phép thử lại nếu có lỗi
            }
        }
        else
        {
            // Hiển thị thông báo chờ
            float remainingTime = clickCooldown - (Time.time - lastClickTime);
            Debug.Log("Vui lòng chờ " + remainingTime.ToString("F2") + " giây nữa để click lại.");
            countdownStatusText.text = "Please wait before clicking again.";
            countdownStatusText.gameObject.SetActive(true);
        }     
    }

    void GenerateAndDisplayNumbers(int generatedNumber)
    {
        int number1, number2;

        // Tạo số ngẫu nhiên khác với generatedNumber cho number1
        do
        {
            number1 = UnityEngine.Random.Range(0, 10); // Thay đổi khoảng giá trị tùy ý
        } while (number1 == generatedNumber);

        // Tạo số ngẫu nhiên khác với generatedNumber và khác với number1 cho number2
        do
        {
            number2 = UnityEngine.Random.Range(0, 10);
        } while (number2 == generatedNumber || number2 == number1);

        // Tạo mảng chứa các Text UI
        TextMeshProUGUI[] texts = new TextMeshProUGUI[] { text1, text2, text3 };

        // Sắp xếp ngẫu nhiên các Text
        ShuffleArray(texts);

        // Gán số vào các Text ngẫu nhiên
        texts[0].text = number1.ToString(); // Hiển thị số ngẫu nhiên đầu tiên
        texts[1].text = number2.ToString(); // Hiển thị số ngẫu nhiên thứ hai
        texts[2].text = generatedNumber.ToString(); // Hiển thị số generatedNumber
    }

    public void CompareText1Value()
    {
        HideAllNumberButton();
        if (correctValue == 0) {
            return;
        }
        // Chuyển đổi giá trị text trong Text thành int
        if (int.TryParse(text1.text, out int textValue))
        {
            // So sánh và in kết quả
            if (textValue == correctValue)
            {
                Debug.Log("Bằng");
                resultText.text = "You get 5000 Coins!";
                resultText.gameObject.SetActive(true);

                //Cộng Coins và hiện thị tại đây bằng singleton
                ResourceBoost.Instance.golds += 5000;
            }
            else
            {
                resultText.text = "You lose!";
                resultText.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Giá trị không hợp lệ");
        }
    }

    public void CompareText2Value()
    {
        HideAllNumberButton();
        if (correctValue == 0)
        {
            return;
        }
        // Chuyển đổi giá trị text trong Text thành int
        if (int.TryParse(text2.text, out int textValue))
        {
            // So sánh và in kết quả
            if (textValue == correctValue)
            {
                Debug.Log("Bằng");
                resultText.text = "You get 5000 Coins!";
                resultText.gameObject.SetActive(true);

                //Cộng Coins và hiện thị tại đây bằng singleton
                ResourceBoost.Instance.golds += 5000;
            }
            else
            {
                Debug.Log("Không bằng");
                resultText.text = "You lose!";
                resultText.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Giá trị không hợp lệ");
        }
    }

    public void CompareText3Value()
    {
        HideAllNumberButton();
        if (correctValue == 0)
        {
            return;
        }
        // Chuyển đổi giá trị text trong Text thành int
        if (int.TryParse(text3.text, out int textValue))
        {
            // So sánh và in kết quả
            if (textValue == correctValue)
            {
                Debug.Log("Bằng");
                resultText.text = "You get 5000 Coins!";
                resultText.gameObject.SetActive(true);

                //Cộng Coins và hiện thị tại đây bằng singleton
                ResourceBoost.Instance.golds += 5000;
            }
            else
            {
                Debug.Log("Không bằng");
                resultText.text = "You lose!";
                resultText.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Giá trị không hợp lệ");
        }
    }

    void ShuffleArray(TextMeshProUGUI[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            // Chọn một chỉ số ngẫu nhiên từ 0 đến length - 1
            int randomIndex = UnityEngine.Random.Range(0, array.Length);
            // Hoán đổi phần tử hiện tại với phần tử tại chỉ số ngẫu nhiên
            TextMeshProUGUI temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    private IEnumerator StartCountdown()
    {
        float remainingTime = clickCooldown;
        while (remainingTime > 0)
        {
            countdownText.text = remainingTime.ToString("F2");
            countdownText.gameObject.SetActive(true);
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        // Khi hết thời gian chờ, xóa text và cho phép click lại
        countdownText.text = "";
        countdownText.gameObject.SetActive(false);
        countdownStatusText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
        isCountingDown = false; // Cho phép người dùng click lại
        HideAllNumberButton();
    }

    private static float ConvertStringToFloat(string numberStr)
    {
        // Convert the string to a float
        float number = float.Parse(numberStr);

        // Return the float value
        return number;
    }

    public async void SpendTokenToBuyGold(int indexValue)
    {
        Address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        HideAllShopButton();

        float costValue = 0.1f;

        if (indexValue == 1)
        {
            costValue = 0.1f;
        }
        else if (indexValue == 2) 
        {
            costValue = 0.2f;
        }
        else if (indexValue == 3)
        {
            costValue = 0.5f;
        }
        else if (indexValue == 4)
        {
            costValue = 1f;
        }

        buyingStatusText.text = "Buying...";
        buyingStatusText.gameObject.SetActive(true);

        var userBalance = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
        if (ConvertStringToFloat(userBalance.displayValue) < costValue)
        {
            buyingStatusText.text = "Not Enough EMC";
        }
        else
        {
            try
            {
                // Thực hiện chuyển tiền, nếu thành công thì tiếp tục xử lý giao diện
                await ThirdwebManager.Instance.SDK.Wallet.Transfer(receiveAddress, costValue.ToString());

                // Chỉ thực hiện các thay đổi giao diện nếu chuyển tiền thành công

                ShowAllShopButton();

                if (indexValue == 1)
                {
                    buyingStatusText.text = "+ $100";
                    ResourceBoost.Instance.golds += 100;
                }
                else if (indexValue == 2)
                {
                    buyingStatusText.text = "+ $300";
                    ResourceBoost.Instance.golds += 300;
                }
                else if (indexValue == 3)
                {
                    buyingStatusText.text = "+ $1000";
                    ResourceBoost.Instance.golds += 1000;
                }
                else if (indexValue == 4)
                {
                    buyingStatusText.text = "+ $3000";
                    ResourceBoost.Instance.golds += 3000;
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có lỗi xảy ra
                Debug.LogError($"Lỗi khi thực hiện chuyển tiền: {ex.Message}");
                buyingStatusText.text = "Error. Please try again";
                ShowAllShopButton();
            }
        }
    }

    public void PlayGame() {
        SceneManager.LoadScene("Logo");
    }
}
