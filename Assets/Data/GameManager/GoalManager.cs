
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BlankGoal
{
   public int NumberNeeded;
   public int NumberCollected;
   public Sprite GoalsSprite;
   public string MatchValue;
}
public class GoalManager : NghiaMono
{
    [SerializeField] protected GameManagerCtr gameManagerCtr;
    public BlankGoal[] Goal;
    public List<GoalPanel> CurrenGoal = new List<GoalPanel>();
    public GameObject GoalPrefab;
    public GameObject GoalIntroParent;
    public GameObject GoalGameParent;
    protected override void Start()
    {
        base.Start();
        this.ResetGoals();
        this.SetupGoal();
    }
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGameManagerCtr();
    }
    protected virtual void LoadGameManagerCtr()
    {
        if (this.gameManagerCtr != null) return;
        this.gameManagerCtr = GetComponentInParent<GameManagerCtr>();
        Debug.Log(transform.name + " :LoadGameManagerCtr", gameObject);
    }
    protected virtual void SetupGoal()
    {
        for (int i = 0; i < Goal.Length; i++)
        {
            GameObject goal = Instantiate(GoalPrefab);
            goal.transform.SetParent(GoalIntroParent.transform, false);

            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = Goal[i].GoalsSprite;
            panel.thisString = "0/" + Goal[i].NumberNeeded;
         
            GameObject Gamegoal = Instantiate(GoalPrefab);
            Gamegoal.transform.SetParent(GoalGameParent.transform, false);
            panel = Gamegoal.GetComponent<GoalPanel>();
            CurrenGoal.Add(panel);
            panel.thisSprite = Goal[i].GoalsSprite;
            panel.thisString = "0/" + Goal[i].NumberNeeded;
        }
    }
    public virtual void ResetGoals()
    {
        for (int i = 0; i < Goal.Length; i++)
        {
            Goal[i].NumberCollected = 0;
        }
    }
    public virtual void updateGoal()
    {
        int GoalComplate = 0;
        for (int i = 0; i < Goal.Length; i++)
        {
            CurrenGoal[i].thisText.text = "" + Goal[i].NumberCollected + "/" + Goal[i].NumberNeeded;
            if (Goal[i].NumberCollected >= Goal[i].NumberNeeded)
            {
                GoalComplate++;
                CurrenGoal[i].thisText.text = "" + Goal[i].NumberNeeded + "/" + Goal[i].NumberNeeded;
            }
        }
        if (GoalComplate >= Goal.Length && gameManagerCtr.GemBoardCtr.CurrentState == GemBoardCtr.GameState.Move)
        {
            gameManagerCtr.GameManager.WinGame();
            Debug.LogWarning("YouWin");
        }
    }
    public virtual void CompareGoal(String GoalToCompare)
    {
        for (int i = 0; i < Goal.Length; i++)
        {
            if (GoalToCompare == Goal[i].MatchValue)
            {
                Goal[i].NumberCollected ++;
            }
        }

    }
}
