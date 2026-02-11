using UnityEngine;

public class PlayerStateManager: MonoBehaviour
{
    public Enums.PlayerState CurrentPlayerState;
    public ObservationStateBehaviour ObservationStateBehaviour;
    public FocusStateBehaviour FocusStateBehaviour;

    public void Init(PlayerController playerController)
    {
        ObservationStateBehaviour.Init(playerController);
        FocusStateBehaviour.Init(playerController);

        CurrentPlayerState = Enums.PlayerState.Observation;
        EnterState(Enums.PlayerState.Observation);
    }

    public void SetState(Enums.PlayerState state)
    {
        ExitState(CurrentPlayerState);
        CurrentPlayerState = state;
        EnterState(CurrentPlayerState);
    }

    private void EnterState(Enums.PlayerState state)
    {
        switch (state)
        {
            case Enums.PlayerState.Observation:
                ObservationStateBehaviour.Enter();
                break;
            case Enums.PlayerState.Focus:
                FocusStateBehaviour.Enter();
                break;
        }
    }

    private void ExitState(Enums.PlayerState state)
    {
        switch (state)
        {
            case Enums.PlayerState.Observation:
                ObservationStateBehaviour.Exit();
                break;
            case Enums.PlayerState.Focus:
                FocusStateBehaviour.Exit();
                break;
        }
    }

    #region Verif current state

    public bool IsCurrentStateEqualsFocus()
    {
        return CurrentPlayerState == Enums.PlayerState.Focus;
    }

    public bool IsCurrentStateEqualsObservation()
    {
        return CurrentPlayerState == Enums.PlayerState.Observation;
    }

    #endregion
}