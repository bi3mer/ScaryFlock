using UnityEngine.Assertions;
using UnityEngine;
using TMPro;

using BGC.StateMachine;

public class GameStateMachine : MonoBehaviour
{
    private StateMachine<Bool, Trigger> sm = new StateMachine<Bool, Trigger>();

    [SerializeField]
    private TextMeshProUGUI scoreText = null;

    private void Awake()
    {
        Assert.IsNotNull(scoreText);
    }

    private void Start()
    {
        CountdownState countDownState = new CountdownState();
        State game = new TriggeringLambdaState<Trigger>(
            name: "game",
            onStateEnter: () =>
            {
                GameManager.Instance.RestartGame();
                return null;
            });
        State scoreScreen = new LambdaState(
            name: "score screen",
            onStateEnter: () =>
            {
                Debug.Log("Show a score!");
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
}
