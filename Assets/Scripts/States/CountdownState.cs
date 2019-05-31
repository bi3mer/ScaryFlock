using UnityEngine;
using System;
using TMPro;

using BGC.StateMachine;

public class CountdownState : TriggeringState<Trigger>
{
    private float time = 0;
    private float countdown = 3;

    public TextMeshProUGUI Text;

    protected override void OnStateEnter()
    {
        time = countdown;
        Text.enabled = true;
    }

    public override void Update()
    {
        time -= Time.deltaTime;
        Text.text = Mathf.RoundToInt(time).ToString();

        if (Text.text.Equals("0", StringComparison.Ordinal))
        {
            Text.text = "GO!";
        }

        if (time <= 0)
        {
            ActivateTrigger(Trigger.NextState);
            Text.enabled = false;
        }
    }
}
