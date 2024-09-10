using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Skyrim_Interpreter;

public class PushYourCode : MonoBehaviour
{
    public Button CreadCard;
    public Button ProcessText;
    public TMP_InputField entrada;
    public GameObject Father;

    public void Start()
    {
        Father.SetActive(false);
        ProcessText.gameObject.SetActive(false);    
    }

    public void StarGame() 
    {
        SceneManager.LoadScene(1);
    }

    public void CreateCardButtom() 
    {
        Father.SetActive(true);
        ProcessText.gameObject.SetActive(true); 
    }
    public void CompileBottom() 
    {
        Father.SetActive(false);
        ProcessText.gameObject.SetActive(false);
        //recibir entrada y tokenizar
        string source = entrada.text;
        List<Token> mylist = new List<Token>();
        Lexer mylexer = new Lexer(source);
        mylist = mylexer.Tokenizer();
        //parsear listade token 
        Parser par = new Parser(mylist);
        par.Parse();
    }

}
