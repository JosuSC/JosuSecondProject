using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CounterPoints : MonoBehaviour
{
    public GameObject p1Row_M, p1Row_R, p1Row_S, p2Row_M, p2Row_R, p2Row_S;    
    public TextMeshProUGUI p1TotalCounter, p2TotalCounter; 
    public TextMeshProUGUI _p1TotalCounter, _p2TotalCounter; //Estos para las sombras

    public TextMeshProUGUI p1RoundCounter, _p1RoundCounter;
    public TextMeshProUGUI p2RoundCounter, _p2RoundCounter;


    private int pointsM_P1 = 0, pointsR_P1 = 0, pointsS_P1 = 0, pointsM_P2 = 0, pointsR_P2 = 0, pointsS_P2 = 0;

    public static int totalPoints_P1;
    public static int totalPoints_P2;

    public static int totalRound_P1;
    public static int totalRound_P2;


    public void ActualizePoints()
    {
         // Actualizar puntos para el jugador 1
        pointsM_P1 = CalculateRowPoints(p1Row_M);
        pointsR_P1 = CalculateRowPoints(p1Row_R);
        pointsS_P1 = CalculateRowPoints(p1Row_S);

        // Actualizar puntos para el jugador 2
        pointsM_P2 = CalculateRowPoints(p2Row_M);
        pointsR_P2 = CalculateRowPoints(p2Row_R);
        pointsS_P2 = CalculateRowPoints(p2Row_S);
    }

    public static int CalculateRowPoints(GameObject row)
    {
        int totalPoints = 0;
        // Obtener todos los objetos hijos de la fila que tengan el componente CardDisplay
        VisualCard[] cardDisplays = row.GetComponentsInChildren<VisualCard>();

        foreach (VisualCard cardDisplay in cardDisplays)
        {
            // Acceder a la tarjeta de cada CardDisplay
            Card card = cardDisplay.card; 
                totalPoints += card.power;
            
        }
        //UnityEngine.Debug.Log("Entro al calcular y es " + totalPoints); 
        return totalPoints;
    }

    public void ActualizeVisual()
    {
        //Actualizar la cantidad de rondas ganadas por  cada jugador 
        p1RoundCounter.text = "Rounds: " + totalRound_P1.ToString();
        _p1RoundCounter.text = "Rounds: " + totalRound_P1.ToString();

        p2RoundCounter.text = "Rounds: " + totalRound_P2.ToString();
        _p2RoundCounter.text = "Rounds: " + totalRound_P2.ToString();
    }

    public void CalculeTotalPoints()
    {
        totalPoints_P1 = CalculateRowPoints(p1Row_M) + CalculateRowPoints(p1Row_R) + CalculateRowPoints(p1Row_S);
        totalPoints_P2 = CalculateRowPoints(p2Row_M) + CalculateRowPoints(p2Row_R) + CalculateRowPoints(p2Row_S); 
        ActualizeTotalPoints();
    } 

    public void ActualizeTotalPoints()
    {
        p1TotalCounter.text = "Points: " + totalPoints_P1.ToString();
        _p1TotalCounter.text = "Points: " + totalPoints_P1.ToString();

        p2TotalCounter.text = "Points: " + totalPoints_P2.ToString();
        _p2TotalCounter.text = "Points: " + totalPoints_P2.ToString();

    }

    void Update()
    {
        ActualizePoints();
        ActualizeVisual();
        CalculeTotalPoints();
        ActualizeTotalPoints();
    }

}