public class StartRounds
{
    public static void UpdateRounds()
    {
        int n = 0; // para jugador 1
        int m = 0; // para jugador 2
        for(int x=0 ; x<GameManager.Instancia.Cementery1.transform.childCount ; x++)
        {
            if(GameManager.Instancia.Cementery1.transform.GetChild(x).transform.GetComponent<VisualCard>().card.power > 0)
            n += GameManager.Instancia.Cementery1.transform.GetChild(x).transform.GetComponent<VisualCard>().card.power;
        }
        for(int x=0 ; x<GameManager.Instancia.Cementery2.transform.childCount ; x++)
        {
            if(GameManager.Instancia.Cementery2.transform.GetChild(x).transform.GetComponent<VisualCard>().card.power > 0)
            m += GameManager.Instancia.Cementery2.transform.GetChild(x).transform.GetComponent<VisualCard>().card.power;
        }
        if(n > m)
        {
            CounterPoints.totalRound_P1 +=1;
        }
        else if(m > n)
        {
            CounterPoints.totalRound_P2 +=1;
        }
        else
        {
            CounterPoints.totalRound_P1 +=1;
            CounterPoints.totalRound_P2 +=1;
        }
    }
}