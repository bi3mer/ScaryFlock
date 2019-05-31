using UnityEngine.Assertions;
using UnityEngine;
using TMPro;

using BGC.StateMachine;

public class GameStateMachine : Singleton<GameStateMachine>
{
    private StateMachine<Bool, Trigger> sm = new StateMachine<Bool, Trigger>();

    [SerializeField]
    private TextMeshProUGUI scoreText = null;

    [SerializeField]
    private ScoreScreen scoreScreen = null;

    private void Awake()
    {
        Assert.IsNotNull(scoreScreen);
        Assert.IsNotNull(scoreText);
    }

    private void Start()
    {
        CountdownState countDownState = new CountdownState();
        State game = new TriggeringLambdaState<Trigger>(
            name: "game",
            onStateEnter: () =>
            {
                GameManager.Instance.ShowScoreUI();
                GameManager.Instance.RestartGame();
                return null;
            });
        State scoreScreen = new LambdaState(
            name: "score screen",
            onStateEnter: () =>
            {
                int score = GameManager.Instance.Score;
                GameManager.Instance.HideScoreUI();
                GameManager.Instance.Reset();
                GameManager.Instance.gameObject.SetActive(false);
                this.scoreScreen.SetScore(score);
                this.scoreScreen.gameObject.SetActive(true);
            });

        countDownState.Text = scoreText;

        sm.AddEntryState(countDownState);
        sm.AddState(game);
        sm.AddState(scoreScreen);

        sm.AddTransition(countDownState, game, sm.CreateTriggerCondition(Trigger.NextState));
        sm.AddTransition(game, scoreScreen, sm.CreateTriggerCondition(Trigger.NextState));

        sm.Start();
    }

    private void Update()
    {
        sm.Update();
    }

    // @NOTE: you shouldn't do this, I just don't have a ton of time and I'm tired
    public void GotoNextState()
    {
        sm.ActivateTriggerDeferred(Trigger.NextState);
    }
}
