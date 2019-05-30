using MLAgents;

public class FlockingAcademy : Academy
{
    public override void AcademyReset()
    {
        GameManager.Instance.RestartGame();
    }

    public override void AcademyStep()
    {
        // pass
    }
}
