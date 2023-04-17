using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarChaser : MonoBehaviour
{
    public enum StarChaserState
    {
        CHASING,
        RETURNING,
        TRADING,
        RESTING
    }
    public StarChaserState CurrentState = StarChaserState.CHASING;

    private float StaminaCap = 5.0f;
    private float StaminaIncreaseRate = 2.0f;
    private float StaminaDecreaseRate = 5.0f;
    private float StaminaDecreaseFrequency = 0.1f;

    private float Speed = 17.0f;

    private float TargetSnapDistance = 0.05f;

    private float PathFindingUpdateRate = 2.0f;




    public float CurrentStamina = 0.0f;
    private float StaminaDecreaseTimer = 0.0f;
    private float PathFindingUpdateTimer = 0.0f;

    private GameObject EntitiesControllerObject = null;
    private EntitiesController EntitiesControllerScript = null;

    private GameObject SpaceShipObject = null;
    private GameObject TradingPostObject = null;

    private GameObject FallenStarObject = null;
    private FallenStar FallenStarScript = null;

    private Vector3 CurrentTarget = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 Velocity = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 Direction = new Vector3(0.0f, 0.0f, 0.0f);

    public List<GameObject> CurrentPath = new List<GameObject>();

    public void RegisterEntitiesController(GameObject entitiesController)
    {
        EntitiesControllerObject = entitiesController;
        EntitiesControllerScript = EntitiesControllerObject.GetComponent<EntitiesController>();
    }
    public void SetStatus(bool state)
    {
        gameObject.SetActive(state);
    }
    public bool GetStatus()
    {
        return gameObject.activeInHierarchy;
    }

    private void UpdatePath()
    {
        PathFindingUpdateTimer -= Time.deltaTime;
        if (PathFindingUpdateTimer <= 0.0f)
        {
            PathFindingUpdateTimer = PathFindingUpdateRate;
            switch (CurrentState)
            {
                case StarChaserState.CHASING:
                    {
                        Vector3 Pos = FallenStarObject.transform.position;
                        GameObject Cell = EntitiesControllerScript.GetCellAtWorldPos(Pos);
                        FindPathToTarget(Cell);
                    }
                    break;
                case StarChaserState.TRADING:
                    {
                        Vector3 Pos = TradingPostObject.transform.position;
                        GameObject Cell = EntitiesControllerScript.GetCellAtWorldPos(Pos);
                        FindPathToTarget(Cell);
                    }
                    break;
                case StarChaserState.RETURNING:
                    {
                        Vector3 Pos = SpaceShipObject.transform.position;
                        GameObject Cell = EntitiesControllerScript.GetCellAtWorldPos(Pos);
                        FindPathToTarget(Cell);
                    }
                    break;
            }
        }
    }
    private void FindPathToTarget(GameObject target)
    {
        Vector3 Pos = transform.position;
        GameObject Cell = EntitiesControllerScript.GetCellAtWorldPos(Pos);
        List<GameObject> FullPath = EntitiesControllerScript.FindPath(Cell, target);
        if (FullPath == null)
            return;

        FullPath.RemoveAt(FullPath.Count - 1);

        CurrentPath = FullPath;
    }

    private void CalculateDirection()
    {
        Vector3 TargetPosition;
        Vector3 StarChaserPosition = transform.position;

        if (CurrentPath.Count > 0)
        {
            TargetPosition = CurrentPath[CurrentPath.Count - 1].transform.position;
            TargetPosition.y = transform.position.y;
            CurrentTarget = TargetPosition;
        }
        else
        {
            PathFindingUpdateTimer = 0.0f; // For immediate PathFinding
            Direction = new Vector3(0.0f, 0.0f, 0.0f);
            return;
        }

        Vector3 DirectionTowardsTarget = CurrentTarget - StarChaserPosition;
        Direction = Vector3.Normalize(DirectionTowardsTarget);
    }
    private void CalculateVelocity()
    {
        Velocity = Speed * Time.deltaTime * Direction;
    }

    private void UpdateMovement()
    {
        CalculateDirection();
        CalculateVelocity();
        transform.position += Velocity;

        if (HasReachedTarget())
        {
            if(CurrentPath.Count > 0)
                CurrentPath.RemoveAt(CurrentPath.Count - 1);
            if (CurrentPath.Count != 0)
                return;

            TransitionToNextState();
        }
    }
    private bool HasReachedTarget()
    {
        Vector3 VecTowardsTarget = (CurrentTarget - transform.position);
        float len = Vector3.Magnitude(VecTowardsTarget);
        if (len <= TargetSnapDistance)
        {
            transform.position = CurrentTarget;
            return true;
        }
        return false;
    }

    private void DecreaseStamina()
    {
        StaminaDecreaseTimer -= Time.deltaTime;
        if (StaminaDecreaseTimer <= 0.0f)
        {
            StaminaDecreaseTimer = StaminaDecreaseFrequency;
            CurrentStamina -= StaminaDecreaseRate * Time.deltaTime;
            if (CurrentStamina <= 0.0f)
            {
                CurrentStamina = 0.0f;
                TransitionToNextState();
            }
        }
    }
    private void IncreaseStamina()
    {
        CurrentStamina += StaminaIncreaseRate * Time.deltaTime;
        if (CurrentStamina >= StaminaCap)
        {
            CurrentStamina = StaminaCap;
            TransitionToNextState();
        }
    }

    private void AcquireFallenStar()
    {
        FallenStarScript.SetStatus(false);
    }
    private void DropAcquiredFallenStar()
    {
        FallenStarScript.SetStatus(true);
        FallenStarObject.transform.position = transform.position;
    }
    private void TradeAcquiredFallenStar()
    {
        EntitiesControllerScript.RespawnFallenStar();
    }

    private void TransitionToNextState()
    {
        switch (CurrentState)
        {
            case StarChaserState.CHASING:
                {
                    AcquireFallenStar();
                    PathFindingUpdateTimer = 0.0f; //For immediate pathfinding at next attempt.
                    CurrentState = StarChaserState.TRADING;
                }
                break;
            case StarChaserState.RETURNING:
                {
                    CurrentState = StarChaserState.RESTING;
                }
                break;
            case StarChaserState.TRADING:
                {
                    if (CurrentStamina <= 0.0f)
                    {
                        DropAcquiredFallenStar();
                        PathFindingUpdateTimer = 0.0f; //For immediate pathfinding at next attempt.
                        CurrentState = StarChaserState.RETURNING;
                        break;
                    }

                    TradeAcquiredFallenStar();
                    CurrentState = StarChaserState.CHASING;
                }
                break;
            case StarChaserState.RESTING:
                {
                    PathFindingUpdateTimer = 0.0f; //For immediate pathfinding at next attempt.
                    CurrentState = StarChaserState.CHASING;
                }
                break;
        }
    }
    private void UpdateState()
    {
        switch (CurrentState)
        {
            case StarChaserState.RESTING:
                {
                    IncreaseStamina();
                }
                break;
            case StarChaserState.TRADING:
                {
                    UpdatePath();
                    UpdateMovement();
                    DecreaseStamina();
                }
                break;
            case StarChaserState.CHASING:
                {
                    UpdatePath();
                    UpdateMovement();
                }
                break;
            case StarChaserState.RETURNING:
                {
                    UpdatePath();
                    UpdateMovement();
                }
                break;
        }
    }

    void Start()
    {
        CurrentStamina = StaminaCap;
        StaminaDecreaseTimer = StaminaDecreaseFrequency;
        PathFindingUpdateTimer = PathFindingUpdateRate;
        FallenStarObject = EntitiesControllerScript.GetFallenStarObject();
        FallenStarScript = FallenStarObject.GetComponent<FallenStar>();
        SpaceShipObject = EntitiesControllerScript.GetSpaceShipObject();
        TradingPostObject = EntitiesControllerScript.GetTradingPostObject();
    }
    void Update()
    {
        UpdateState();
    }
}
