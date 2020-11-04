using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGamePanel : MonoBehaviour
{
    public Button m_Btn1;
    public Button m_Btn2;
    public Button m_Btn3;
    public Text m_TextCoin;

    private int m_CoinValue = 10000;

    private void Start()
    {
        m_Btn1.onClick.AddListener(()=> {

        });

        m_Btn2.onClick.AddListener(() => {

        });

        m_Btn3.onClick.AddListener(() => {

        });

        SetCoinText();
    }

    private void SetCoinText()
    {
        m_TextCoin.text = m_CoinValue.ToString();
    }
}
