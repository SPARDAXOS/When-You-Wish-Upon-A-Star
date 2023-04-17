using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationDirector : MonoBehaviour
{
    private int WidthCellsCount = 24;
    private int HeightCellsCount = 12;

    private Vector3 GridOriginPoint =  new Vector3(0.0f, 0.0f, 0.0f);


    private GameObject GridManagerSample = null;
    private GameObject GridManagerObject = null;
    private GridManager GridManagerScript = null;

    private GameObject PathFinderSample = null;
    private GameObject PathFinderObject = null;
    private PathFinder PathFinderScript = null;

    private GameObject EntitiesControllerSample = null;
    private GameObject EntitiesControllerObject = null;
    private EntitiesController EntitiesControllerScript = null;

    private GameObject MainCameraSample = null;
    private GameObject MainCameraObject = null;
    private MainCamera MainCameraScript = null;

    private GameObject InfoOverlaySample = null;
    private GameObject InfoOverlayObject = null;
    private InfoOverlay InfoOverlayScript = null;


    private void LoadResources()
    {
        GridManagerSample = Resources.Load<GameObject>("Grid");
        if (GridManagerSample == null)
            Debug.LogError("Loading resources at SimulationDirector has failed! - GridManagerSample");

        PathFinderSample = Resources.Load<GameObject>("PathFinder");
        if (PathFinderSample == null)
            Debug.LogError("Loading resources at SimulationDirector has failed! - PathFinderSample");

        EntitiesControllerSample = Resources.Load<GameObject>("EntitiesController");
        if (EntitiesControllerSample == null)
            Debug.LogError("Loading resources at SimulationDirector has failed! - EntitiesControllerSample");

        MainCameraSample = Resources.Load<GameObject>("MainCamera");
        if (MainCameraSample == null)
            Debug.LogError("Loading resources at SimulationDirector has failed! - MainCameraSample");

        InfoOverlaySample = Resources.Load<GameObject>("InfoOverlay");
        if (InfoOverlaySample == null)
            Debug.LogError("Loading resources at InfoOverlay has failed! - InfoOverlaySample");
    }
    private void CreateGridManager()
    {
        if(GridManagerSample == null)
        {
            Debug.LogError("GridSample is Null - CreateGridManager");
            return;
        }
        GridManagerObject = Instantiate(GridManagerSample);
        if(GridManagerObject == null)
        {
            Debug.LogError("GridManagerObject is Null - CreateGridManager");
            return;
        }

        GridManagerScript = GridManagerObject.GetComponent<GridManager>();
        if (GridManagerScript == null)
            Debug.LogError("Loading Script at SimulationDirector has failed! - MainGridScript");
    }
    private void CreatePathFinder()
    {
        if (PathFinderSample == null)
        {
            Debug.LogError("PathFinderSample is Null - CreatePathFinder");
            return;
        }
        PathFinderObject = Instantiate(PathFinderSample);
        if (PathFinderObject == null)
        {
            Debug.LogError("PathFinderObject is Null - CreatePathFinder");
            return;
        }

        PathFinderScript = PathFinderObject.GetComponent<PathFinder>();
        if (PathFinderScript == null)
            Debug.LogError("Loading Script at SimulationDirector has failed! - PathFinderScript");
    }
    private void CreateEntitiesController()
    {
        if (EntitiesControllerSample == null)
        {
            Debug.LogError("EntitiesControllerSample is Null - CreateEntitiesController");
            return;
        }
        EntitiesControllerObject = Instantiate(EntitiesControllerSample);
        if (EntitiesControllerObject == null)
        {
            Debug.LogError("EntitiesControllerObject is Null - CreateEntitiesController");
            return;
        }

        EntitiesControllerScript = EntitiesControllerObject.GetComponent<EntitiesController>();
        if (EntitiesControllerScript == null)
            Debug.LogError("Loading Script at SimulationDirector has failed! - EntitiesControllerScript");
    }
    private void CreateMainCamera()
    {
        if (MainCameraSample == null)
        {
            Debug.LogError("MainCameraSample is Null - CreateMainCamera");
            return;
        }
        MainCameraObject = Instantiate(MainCameraSample);
        if (MainCameraObject == null)
        {
            Debug.LogError("MainCameraObject is Null - CreateMainCamera");
            return;
        }

        MainCameraScript = MainCameraObject.GetComponent<MainCamera>();
        if (MainCameraScript == null)
            Debug.LogError("Loading Script at SimulationDirector has failed! - MainCameraScript");
    }
    private void CreateInfoOverlay()
    {
        if (InfoOverlaySample == null)
        {
            Debug.LogError("InfoOverlaySample is Null - CreateInfoOverlay");
            return;
        }
        InfoOverlayObject = Instantiate(InfoOverlaySample);
        if (InfoOverlayObject == null)
        {
            Debug.LogError("InfoOverlayObject is Null - CreateInfoOverlay");
            return;
        }

        InfoOverlayScript = InfoOverlayObject.GetComponent<InfoOverlay>();
        if (InfoOverlayScript == null)
            Debug.LogError("Loading Script at InfoOverlay has failed! - InfoOverlayScript");
    }

    private void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            MainCameraScript.UpdateClickPos();

        if (Input.GetKeyDown(KeyCode.O))
        {
            InfoOverlayScript.ToggleOverlay();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            InfoOverlayScript.ToggleLegends();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            EntitiesControllerScript.SetEntityPosition(EntitiesController.MovableEntities.SPACESHIP); ;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            EntitiesControllerScript.SetEntityPosition(EntitiesController.MovableEntities.TRADINGPOST);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            EntitiesControllerScript.SetEntityPosition(EntitiesController.MovableEntities.FALLENSTAR);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PathFinderScript.TogglePathfindingMode();
            var Mode = PathFinderScript.GetCurrentPathfindingMode();
            if (Mode == PathFinder.PathfindingMode.ASTAR)
                InfoOverlayScript.TogglePopUpText("A*");
            else
                InfoOverlayScript.TogglePopUpText("JPS");
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            PathFinderScript.ToggleVisualizePath();
        }


        //UI with instructions? 
        //UI with legends? tile types and such
        //StartChaser info? stamina and such
        //Ability to adjust ai update data and such?

    }

    //private void TestPathFinder(PathFinder.Mode mode)
    //{
    //    switch (mode)
    //    {
    //        case PathFinder.Mode.ASTAR:
    //            {
    //                GameObject StartCell = GridManagerScript.GetRandomEmptyCell();
    //                GameObject EndCell = GridManagerScript.GetRandomEmptyCell();
    //                PathFinderScript.FindPath(StartCell, EndCell, PathFinder.Mode.ASTAR, true);
    //            }
    //            break;
    //        case PathFinder.Mode.JPS:
    //            {
    //                //GameObject StartCell = GridManagerScript.GetCellAt(84);
    //                //77 //72 //95 //90
    //                //GameObject EndCell = GridManagerScript.GetCellAt(77);
    //                GameObject StartCell = GridManagerScript.GetRandomEmptyCell();
    //                GameObject EndCell = GridManagerScript.GetRandomEmptyCell();
    //                PathFinderScript.FindPath(StartCell, EndCell, PathFinder.Mode.JPS, true);
    //            }
    //            break;
    //    }
    //}


    void Start()
    {
        LoadResources();
        CreateGridManager();
        CreatePathFinder();
        CreateEntitiesController();
        CreateMainCamera();
        CreateInfoOverlay();

        GridManagerScript.LoadResources();
        GridManagerScript.SetOriginPoint(GridOriginPoint);
        GridManagerScript.CreateGrid(WidthCellsCount, HeightCellsCount);

        PathFinderScript.RegisterGridManager(GridManagerObject);

        EntitiesControllerScript.LoadResources();
        EntitiesControllerScript.RegisterGridManager(GridManagerObject);
        EntitiesControllerScript.RegisterPathFinder(PathFinderObject);
        EntitiesControllerScript.CreateEntities();
        EntitiesControllerScript.SetupEntities();

        MainCameraScript.RegisterGridManager(GridManagerObject);

    }
    void Update()
    {
        UpdateInput();
    }
}
