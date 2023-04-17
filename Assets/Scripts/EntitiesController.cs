using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesController : MonoBehaviour
{
    public enum MovableEntities
    {
        SPACESHIP,
        TRADINGPOST,
        FALLENSTAR
    }

    private float EntitiesYOffset = 3.0f;


    private GameObject StarChaserSample = null;
    private GameObject StarChaserObject = null;
    private StarChaser StarChaserScript = null;

    private GameObject SpaceShipSample = null;
    private GameObject SpaceShipObject = null;
    private SpaceShip SpaceShipScript = null;

    private GameObject TradingPostSample = null;
    private GameObject TradingPostObject = null;
    private TradingPost TradingPostScript = null;

    private GameObject FallenStarSample = null;
    private GameObject FallenStarObject = null;
    private FallenStar FallenStarScript = null;

    private GameObject GridManagerObject = null;
    private GridManager GridManagerScript = null;

    private GameObject PathFinderObject = null;
    private PathFinder PathFinderScript = null;


    public void LoadResources()
    {
        StarChaserSample = Resources.Load<GameObject>("StarChaser");
        if (StarChaserSample == null)
            Debug.LogError("Loading resources at EntitiesController has failed! - StarChaser");

        SpaceShipSample = Resources.Load<GameObject>("SpaceShip");
        if (SpaceShipSample == null)
            Debug.LogError("Loading resources at EntitiesController has failed! - SpaceShip");

        TradingPostSample = Resources.Load<GameObject>("TradingPost");
        if (TradingPostSample == null)
            Debug.LogError("Loading resources at EntitiesController has failed! - TradingPost");

        FallenStarSample = Resources.Load<GameObject>("FallenStar");
        if (FallenStarSample == null)
            Debug.LogError("Loading resources at EntitiesController has failed! - FallenStar");
    }
    public void RegisterGridManager(GameObject gridManager)
    {
        GridManagerObject = gridManager;
        GridManagerScript = GridManagerObject.GetComponent<GridManager>();
    }
    public void RegisterPathFinder(GameObject pathFinder)
    {
        PathFinderObject = pathFinder;
        PathFinderScript = PathFinderObject.GetComponent<PathFinder>();
    }

    private void CreateStarChaser()
    {
        if (StarChaserSample == null)
        {
            Debug.LogError("StarChaserSample is Null - StarChaser");
            return;
        }
        StarChaserObject = Instantiate(StarChaserSample);
        if (StarChaserObject == null)
        {
            Debug.LogError("StarChaserObject is Null - CreateStarChaser");
            return;
        }

        StarChaserScript = StarChaserObject.GetComponent<StarChaser>();
        if (StarChaserScript == null)
            Debug.LogError("Loading Script at EntitiesController has failed! - StarChaserScript");
        StarChaserScript.SetStatus(false);
    }
    private void CreateSpaceShip()
    {
        if (SpaceShipSample == null)
        {
            Debug.LogError("SpaceShipSample is Null - SpaceShip");
            return;
        }
        SpaceShipObject = Instantiate(SpaceShipSample);
        if (SpaceShipObject == null)
        {
            Debug.LogError("SpaceShipObject is Null - CreateSpaceShip");
            return;
        }

        SpaceShipScript = SpaceShipObject.GetComponent<SpaceShip>();
        if (SpaceShipScript == null)
            Debug.LogError("Loading Script at EntitiesController has failed! - SpaceShipScript");
        SpaceShipScript.SetStatus(false);
    }
    private void CreateTradingPost()
    {
        if (TradingPostSample == null)
        {
            Debug.LogError("TradingPostSample is Null - TradingPost");
            return;
        }
        TradingPostObject = Instantiate(TradingPostSample);
        if (TradingPostObject == null)
        {
            Debug.LogError("TradingPostObject is Null - CreateTradingPost");
            return;
        }

        TradingPostScript = TradingPostObject.GetComponent<TradingPost>();
        if (TradingPostScript == null)
            Debug.LogError("Loading Script at EntitiesController has failed! - TradingPostScript");
        TradingPostScript.SetStatus(false);
    }
    private void CreateFallenStar()
    {
        if (FallenStarSample == null)
        {
            Debug.LogError("FallenStarSample is Null - FallenStar");
            return;
        }
        FallenStarObject = Instantiate(FallenStarSample);
        if (FallenStarObject == null)
        {
            Debug.LogError("FallenStarObject is Null - CreateFallenStar");
            return;
        }

        FallenStarScript = FallenStarObject.GetComponent<FallenStar>();
        if (FallenStarScript == null)
            Debug.LogError("Loading Script at EntitiesController has failed! - FallenStarScript");
        FallenStarScript.SetStatus(false);
    }

    public void CreateEntities()
    {
        CreateStarChaser();
        CreateSpaceShip();
        CreateTradingPost();
        CreateFallenStar();
    }
    public void SetupEntities()
    {
        GameObject StarChaserCell = null;
        GameObject SpaceShipCell = null;
        GameObject TradingPostCell = null;
        GameObject FallenStarCell = null;

        Vector3 CellPosition = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 EntityPosition = new Vector3(0.0f, 0.0f, 0.0f);

        StarChaserCell = GridManagerScript.GetRandomEmptyCell();
        if (!StarChaserCell)
        {
            Debug.LogError("Invalid Cell - StarChaser");
            return;
        }

        SpaceShipCell = GridManagerScript.GetRandomEmptyCell();
        if (!SpaceShipCell)
        {
            Debug.LogError("Invalid Cell - SpaceShip");
            return;
        }

        TradingPostCell = GridManagerScript.GetRandomEmptyCell();
        if (!TradingPostCell)
        {
            Debug.LogError("Invalid Cell - TradingPost");
            return;
        }

        FallenStarCell = GridManagerScript.GetRandomEmptyCell();
        if (!FallenStarCell)
        {
            Debug.LogError("Invalid Cell - FallenStar");
            return;
        }

        CellPosition = StarChaserCell.transform.position;
        EntityPosition = new Vector3(CellPosition.x, CellPosition.y + EntitiesYOffset, CellPosition.z);
        StarChaserObject.transform.position = EntityPosition;

        CellPosition = SpaceShipCell.transform.position;
        EntityPosition = new Vector3(CellPosition.x, CellPosition.y + EntitiesYOffset, CellPosition.z);
        SpaceShipObject.transform.position = EntityPosition;

        CellPosition = TradingPostCell.transform.position;
        EntityPosition = new Vector3(CellPosition.x, CellPosition.y + EntitiesYOffset, CellPosition.z);
        TradingPostObject.transform.position = EntityPosition;

        CellPosition = FallenStarCell.transform.position;
        EntityPosition = new Vector3(CellPosition.x, CellPosition.y + EntitiesYOffset, CellPosition.z);
        FallenStarObject.transform.position = EntityPosition;

        StarChaserScript.RegisterEntitiesController(gameObject);

        StarChaserScript.SetStatus(true);
        SpaceShipScript.SetStatus(true);
        TradingPostScript.SetStatus(true);
        FallenStarScript.SetStatus(true);
    }

    public List<GameObject> FindPath(GameObject start, GameObject target)
    {
        List<GameObject> Path = PathFinderScript.FindPath(start, target);
        return Path;
    }

    public GameObject GetFallenStarObject()
    {
        return FallenStarObject;
    }
    public GameObject GetSpaceShipObject()
    {
        return SpaceShipObject;
    }
    public GameObject GetTradingPostObject()
    {
        return TradingPostObject;
    }
    public GameObject GetCellAtWorldPos(Vector3 pos)
    {
        return GridManagerScript.GetCellAtWorldPos(pos);
    }

    public void RespawnFallenStar()
    {
        GameObject FallenStarCell = null;
        Vector3 CellPosition = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 EntityPosition = new Vector3(0.0f, 0.0f, 0.0f);

        FallenStarCell = GridManagerScript.GetRandomEmptyCell();

        CellPosition = FallenStarCell.transform.position;
        EntityPosition = new Vector3(CellPosition.x, CellPosition.y + EntitiesYOffset, CellPosition.z);
        FallenStarObject.transform.position = EntityPosition;

        FallenStarScript.SetStatus(true);
    }

    public void SetEntityPosition(MovableEntities entity)
    {
        Vector3 MousePos = Input.mousePosition;
        MousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 WorldPos = Camera.main.ScreenToWorldPoint(MousePos);
        GameObject TargetCell = GridManagerScript.GetCellAtWorldPos(WorldPos);

        if (!TargetCell)
        {
            return;
        }
        if (TargetCell.GetComponent<Cell>().GetCellType() == Cell.CellType.BLOCKED)
        {
            return;
        }

        Vector3 TargetCellPosition = TargetCell.transform.position;
        if (IsThereEntityAt(TargetCellPosition))
        {
            return;
        }

        Vector3 EntityPosition = new Vector3(TargetCellPosition.x, TargetCellPosition.y + EntitiesYOffset, TargetCellPosition.z);

        switch (entity)
        {
            case MovableEntities.SPACESHIP:
                {
                    SpaceShipObject.transform.position = EntityPosition;
                }
                break;
            case MovableEntities.TRADINGPOST:
                {
                    TradingPostObject.transform.position = EntityPosition;
                }
                break;
            case MovableEntities.FALLENSTAR:
                {
                    FallenStarObject.transform.position = EntityPosition;
                }
                break;
        }
    }

    private bool IsThereEntityAt(Vector3 position)
    {
        position.y = EntitiesYOffset;
        if (position == StarChaserObject.transform.position)
        {
            return true;
        }
        if (position == SpaceShipObject.transform.position)
        {
            return true;
        }
        if (position == TradingPostObject.transform.position)
        {
            return true;
        }
        if (position == FallenStarObject.transform.position)
        {
            return true;
        }

        return false;
    }
}
