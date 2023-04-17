using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public enum PathfindingMode
    {
        ASTAR,
        JPS
    }

    private PathfindingMode CurrentPathfindingMode = PathfindingMode.ASTAR;
    private bool VisualizePath = false;

    private GameObject GridManagerObject;
    private GridManager GridManagerScript;

    private List<GameObject> OpenList = new List<GameObject>();
    private List<GameObject> ClosedList = new List<GameObject>();

    public void RegisterGridManager(GameObject gridManager)
    {
        GridManagerObject = gridManager;
        GridManagerScript = GridManagerObject.GetComponent<GridManager>();
    }

    public List<GameObject> FindPath(GameObject start, GameObject target)
    {
        List<GameObject> Path = new List<GameObject>();

        switch (CurrentPathfindingMode)
        {
            case PathfindingMode.ASTAR:
                {
                    Path = FindAstarPath(start, target);
                }
                break;
            case PathfindingMode.JPS:
                {
                    Path = FindJPSPath(start, target);
                }
                break;
        }

        if (Path.Count <= 0)
            return null;

        return Path;
    }

    public void ToggleVisualizePath()
    {
        if (VisualizePath)
            VisualizePath = false;
        else
            VisualizePath = true;
    }
    public void TogglePathfindingMode()
    {
        if (CurrentPathfindingMode == PathfindingMode.ASTAR)
            CurrentPathfindingMode = PathfindingMode.JPS;
        else
            CurrentPathfindingMode = PathfindingMode.ASTAR;
    }
    public PathfindingMode GetCurrentPathfindingMode()
    {
        return CurrentPathfindingMode;
    }



    private List<GameObject> FindAstarPath(GameObject start, GameObject target)
    {
        List<GameObject> Path = new List<GameObject>();
        List<GameObject> AdjacentCells;

        GameObject CurrentProcessingCell = null;
        Cell CurrentProcessingCellScript = null;

        Cell AdjacentCellScript = null;
        int CurrentProcessingCellIndex = -1;
        int LowestFCostCellIndex = -1;

        bool PathFound = false;

        int TargetCellIndex = target.GetComponent<Cell>().GetCellIndex();

        //If the start is actually the target.
        if (start.GetComponent<Cell>().GetCellIndex() == TargetCellIndex)
            return Path;

        //Clean up the open and closed lists, and reset all cells.
        CleanLists();
        GridManagerScript.CleanUpAllCells();

        // Setting up the starting cell!
        start.GetComponent<Cell>().SetAsParent();
        start.GetComponent<Cell>().CalculateHCost(target.transform.position);
        start.GetComponent<Cell>().CalculateFCost();
        OpenList.Add(start);

        //Pathfinding loop starts!
        while (OpenList.Count > 0)
        {
            LowestFCostCellIndex = GetNextInOpenList();
            if (LowestFCostCellIndex < 0)
            {
                Debug.LogError("No Lowest FCost cell was returned!");
                break;
            }

            CurrentProcessingCell = OpenList[LowestFCostCellIndex];
            CurrentProcessingCellScript = CurrentProcessingCell.GetComponent<Cell>();
            CurrentProcessingCellIndex = CurrentProcessingCellScript.GetCellIndex();

            if (CurrentProcessingCellScript.GetCellIndex() == TargetCellIndex)
            {
                PathFound = true;
                break;
            }

            ClosedList.Add(CurrentProcessingCell);
            OpenList.Remove(CurrentProcessingCell);

            AdjacentCells = GridManagerScript.GetAdjacentCellsAt(CurrentProcessingCellIndex);
            for (int j = 0; j < AdjacentCells.Count; j++)
            {
                if (IsInClosedList(AdjacentCells[j]) || IsInOpenList(AdjacentCells[j]))
                    continue;
                else
                {
                    AdjacentCellScript = AdjacentCells[j].GetComponent<Cell>();
                    AdjacentCellScript.SetParentCell(CurrentProcessingCell);
                    AdjacentCellScript.CalculateHCost(target.transform.position);
                    AdjacentCellScript.CalculateGCost();
                    AdjacentCellScript.CalculateFCost();
                    OpenList.Add(AdjacentCells[j]);
                    if (VisualizePath)
                        AdjacentCellScript.SetColor(Color.blue);
                }
            }
        }

        //Path Clean up here!
        if (PathFound)
        {
            CurrentProcessingCell = target;
            CurrentProcessingCellScript = target.GetComponent<Cell>();
            Path.Add(CurrentProcessingCell);
            while (CurrentProcessingCellScript.GetParent() != null)
            {
                CurrentProcessingCell = CurrentProcessingCellScript.GetParent();
                CurrentProcessingCellScript = CurrentProcessingCell.GetComponent<Cell>();
                Path.Add(CurrentProcessingCell);
            }

            if (VisualizePath)
            {
                for (int i = 0; i < Path.Count; i++)
                {
                    if (i == 0)
                    {
                        Path[i].GetComponent<Cell>().SetColor(Color.red);
                        continue;
                    }
                    if (i == Path.Count - 1)
                    {
                        Path[i].GetComponent<Cell>().SetColor(Color.green);
                        continue;
                    }

                    Path[i].GetComponent<Cell>().SetColor(Color.magenta);
                }
            }
        }

        return Path;
    }
    private List<GameObject> FindJPSPath(GameObject start, GameObject target)
    {
        List<GameObject> Neighbours = new List<GameObject>();

        GameObject ProcessingNeighbour = null;
        Cell ProcessingNeighbourScript = null;
        int ProcessingNeighbourIndex = -1;

        GameObject PotentialForcedNeighbour = null;
        Cell PotentialForcedNeighbourScript = null;

        Vector3 NeighbourDirectionToParent;




        List<GameObject> Path = new List<GameObject>();

        GameObject CurrentRootCell; 
        Cell CurrentRootCellScript;
        int CurrentRootCellIndex = -1;

        GameObject CurrentProcessingCell;
        Cell CurrentProcessingCellScript;

        int LowestFCostCellIndex = -1;


        bool PathFound = false;

        Vector3 DirectionToParent;
        Vector3 CurrentRootPosition;
        Vector3 CurrentRootParentPosition;

        int XInt = 0;
        int ZInt = 0;

        int StartingCellIndex = start.GetComponent<Cell>().GetCellIndex();
        int TargetCellIndex = target.GetComponent<Cell>().GetCellIndex();



        //If the start is actually the target.
        if (StartingCellIndex == TargetCellIndex)
            return Path;

        //Clean up the open and closed lists, and reset all cells.
        CleanLists();
        GridManagerScript.CleanUpAllCells();

        //Calculate Its shit
        start.GetComponent<Cell>().SetAsParent();
        start.GetComponent<Cell>().CalculateHCost(target.transform.position);
        start.GetComponent<Cell>().CalculateFCost();
        OpenList.Add(start);

        //InitialOrthoExpansion
        PathFound = CheckOrthogonalPaths(start, target);

        //Diagonal+OrthoExpansion
        if (!PathFound)
            PathFound = CheckDiagonalPaths(start, target);

        ClosedList.Add(start);
        OpenList.Remove(start);


        if (!PathFound)
        {
            while (OpenList.Count > 0)
            {
                //Gets the index of the cell with the lowest if cost.
                LowestFCostCellIndex = GetNextInOpenList();
                if (LowestFCostCellIndex == -1)
                {
                    Debug.LogError("Failed at the very start to find the lowest f cost.");
                    return null;
                }

                CurrentRootCell = OpenList[LowestFCostCellIndex];
                CurrentRootCellScript = CurrentRootCell.GetComponent<Cell>();
                CurrentRootCellIndex = CurrentRootCellScript.GetCellIndex();

                //Check if its the target before you go in.
                if (CurrentRootCellIndex == TargetCellIndex)
                {
                    PathFound = true;
                    break;
                }

                CurrentRootPosition = CurrentRootCell.transform.position;
                CurrentRootParentPosition = CurrentRootCellScript.GetParent().transform.position;
                DirectionToParent = CalculateDirection(CurrentRootPosition, CurrentRootParentPosition);
                DirectionToParent *= -1;

                if (IsInClosedList(CurrentRootCell))
                {
                    OpenList.Remove(CurrentRootCell);
                    ClosedList.Add(CurrentRootCell);
                    continue;
                }


                //Check diagonaly for neighbours.
                //Forced Neighbours.
                if (!IsInClosedList(CurrentRootCell))
                {
                    if (DirectionToParent.x > 0)
                        XInt = -1;
                    else if(DirectionToParent.x < 0)
                        XInt = 1;

                    if (DirectionToParent.z > 0)
                        ZInt = 1;
                    else if (DirectionToParent.z < 0)
                        ZInt = -1;

                    Neighbours = GridManagerScript.GetDiagonalNeighbours(CurrentRootCellIndex, XInt, ZInt);
                    for (int i = 0; i < Neighbours.Count; i++)
                    {
                        ProcessingNeighbour = Neighbours[i];
                        ProcessingNeighbourScript = ProcessingNeighbour.GetComponent<Cell>();
                        ProcessingNeighbourIndex = ProcessingNeighbourScript.GetCellIndex();

                        if (ProcessingNeighbourScript.GetCellType() == Cell.CellType.BLOCKED)
                        {
                            //Calculates direction to parent.
                            NeighbourDirectionToParent =
                                CalculateDirection(ProcessingNeighbour.transform.position, CurrentRootCell.transform.position);

                            //Gets the correct potential forced neighbour.
                            if (NeighbourDirectionToParent.x == 1 || NeighbourDirectionToParent.x == -1)
                            {
                                PotentialForcedNeighbour =
                                    GridManagerScript.GetAdjacentVerticalCellAt(ProcessingNeighbourIndex, -ZInt);
                            }
                            else if (NeighbourDirectionToParent.z == 1 || NeighbourDirectionToParent.z == -1)
                            {
                                PotentialForcedNeighbour =
                                    GridManagerScript.GetAdjacentHorizontalCellAt(ProcessingNeighbourIndex, -XInt);
                            }

                            //If its not valid.
                            if (!PotentialForcedNeighbour)
                                continue;

                            //If its already in the open list.
                            if (IsInOpenList(PotentialForcedNeighbour))
                                continue;

                            if (IsInClosedList(PotentialForcedNeighbour))
                                continue;

                            PotentialForcedNeighbourScript = PotentialForcedNeighbour.GetComponent<Cell>();
                            if (PotentialForcedNeighbourScript.GetCellType() == Cell.CellType.EMPTY)
                            {
                                PotentialForcedNeighbourScript.SetParentCell(CurrentRootCell);
                                PotentialForcedNeighbourScript.CalculateHCost(target.transform.position);
                                PotentialForcedNeighbourScript.CalculateGCost();

                                PotentialForcedNeighbourScript.CalculateFCost();

                                OpenList.Add(PotentialForcedNeighbour);
                                ClosedList.Add(CurrentRootCell);

                                //Visualization.
                                if (VisualizePath)
                                    PotentialForcedNeighbourScript.SetColor(Color.blue);
                            }
                        }
                    }
                }


                //OrthoChecks
                if (DirectionToParent.x > 0)
                {
                    PathFound = CheckHorizontalLine(CurrentRootCell, target, 1);
                    if (PathFound)
                        break;
                }
                else if (DirectionToParent.x < 0)
                {
                    PathFound = CheckHorizontalLine(CurrentRootCell, target, -1);
                    if (PathFound)
                        break;
                }

                if (DirectionToParent.z > 0)
                {
                    PathFound = CheckVerticalLine(CurrentRootCell, target, -1);
                    if (PathFound)
                        break;

                }
                else if (DirectionToParent.z < 0)
                {
                    PathFound = CheckVerticalLine(CurrentRootCell, target, 1);
                    if (PathFound)
                        break;
                }


                //DiagonalChecks
                //TopRight
                if (DirectionToParent.x > 0 && DirectionToParent.z > 0)
                {
                    PathFound = CheckDiagonalLine(CurrentRootCell, target, 1, -1);
                    if (PathFound)
                        break;
                }
                //DownRight
                else if (DirectionToParent.x > 0 && DirectionToParent.z < 0)
                {
                    PathFound = CheckDiagonalLine(CurrentRootCell, target, 1, 1);
                    if (PathFound)
                        break;
                }
                //UpLeft
                else if (DirectionToParent.x < 0 && DirectionToParent.z > 0)
                {
                    PathFound = CheckDiagonalLine(CurrentRootCell, target, -1, -1);
                    if (PathFound)
                        break;
                }
                //DownLeft
                else if (DirectionToParent.x < 0 && DirectionToParent.z < 0)
                {
                    PathFound = CheckDiagonalLine(CurrentRootCell, target, -1, 1);
                    if (PathFound)
                        break;
                }

                ClosedList.Add(CurrentRootCell);
                OpenList.Remove(CurrentRootCell);
            }
        }



        //Path Clean up here!
        if (PathFound)
        {
            CurrentProcessingCell = target;
            CurrentProcessingCellScript = target.GetComponent<Cell>();
            Path.Add(CurrentProcessingCell);
            while (CurrentProcessingCellScript.GetParent() != null)
            {
                CurrentProcessingCell = CurrentProcessingCellScript.GetParent();
                CurrentProcessingCellScript = CurrentProcessingCell.GetComponent<Cell>();
                Path.Add(CurrentProcessingCell);

                if (Path.Count > GridManagerScript.GetCellsCount())
                {
                    Path.Clear();
                    return Path;
                }
            }

            if (VisualizePath)
            {
                for (int i = 0; i < Path.Count; i++)
                {
                    if (i == 0)
                    {
                        Path[i].GetComponent<Cell>().SetColor(Color.red);
                        continue;
                    }
                    if (i == Path.Count - 1)
                    {
                        Path[i].GetComponent<Cell>().SetColor(Color.green);
                        continue;
                    }

                    Path[i].GetComponent<Cell>().SetColor(Color.magenta);
                }
            }
        }

        return Path;
    }

    private void CleanLists()
    {
        if (OpenList.Count > 0)
            OpenList.Clear();
        if (ClosedList.Count > 0)
            ClosedList.Clear();
    }

    private bool IsInClosedList(GameObject cell)
    {
        if (ClosedList.Count <= 0)
            return false;

        int CellIndexA = cell.GetComponent<Cell>().GetCellIndex();
        int CellIndexB;

        for(int i = 0; i < ClosedList.Count; i++)
        {
            CellIndexB = ClosedList[i].GetComponent<Cell>().GetCellIndex();
            if (CellIndexA == CellIndexB)
                return true;
        }
        return false;
    }
    private bool IsInOpenList(GameObject cell)
    {
        if (OpenList.Count <= 0)
            return false;

        int CellIndexA = cell.GetComponent<Cell>().GetCellIndex();
        int CellIndexB;

        for (int i = 0; i < OpenList.Count; i++)
        {
            CellIndexB = OpenList[i].GetComponent<Cell>().GetCellIndex();
            if (CellIndexA == CellIndexB)
                return true;
        }
        return false;
    }
    private int GetNextInOpenList()
    {
        int Lowest = 0;
        int LowestIndex = -1;
        Cell CellScript = null;

        if (OpenList.Count <= 0)
            return LowestIndex;
        for (int i = 0; i < OpenList.Count; i++)
        {
            CellScript = OpenList[i].GetComponent<Cell>();
            if (i == 0)
            {
                Lowest = CellScript.GetFCost();
                LowestIndex = i;
                continue;
            }
            // Gets latest cell with lowest f cost!
            if (CellScript.GetFCost() < Lowest)
            {
                Lowest = CellScript.GetFCost();
                LowestIndex = i;
            }
        }

        return LowestIndex;
    }


    private bool CheckDiagonalPaths(GameObject root, GameObject target)
    {
        bool TargetFound = false;

        //TopRight
        TargetFound = CheckDiagonalLine(root, target, 1, -1);
        if (TargetFound)
            return TargetFound;

        //TopLeft
        TargetFound = CheckDiagonalLine(root, target, -1, -1);
        if (TargetFound)
            return TargetFound;

        //DownRight
        TargetFound = CheckDiagonalLine(root, target, 1, 1);
        if (TargetFound)
            return TargetFound;

        //DownLeft
        TargetFound = CheckDiagonalLine(root, target, -1, 1);
        if (TargetFound)
            return TargetFound;

        return TargetFound;
    }
    private bool CheckOrthogonalPaths(GameObject root, GameObject target)
    {
        bool TargetFound = false;

        //Right
        TargetFound = CheckHorizontalLine(root, target, 1);
        if (TargetFound)
            return TargetFound;

        //Left
        TargetFound = CheckHorizontalLine(root, target, -1);
        if (TargetFound)
            return TargetFound;

        //Up
        TargetFound = CheckVerticalLine(root, target, -1);
        if (TargetFound)
            return TargetFound;

        //Down
        TargetFound = CheckVerticalLine(root, target, 1);
        if (TargetFound)
            return TargetFound;

        return TargetFound;
    }


    private bool CheckHorizontalLine(GameObject root, GameObject target, int dir)
    {
        List<GameObject> FullLine;
        List<GameObject> Neighbours;

        GameObject ProcessingCell = null;
        Cell ProcessingCellScript = null;
        int ProcessingCellIndex = -1;

        GameObject ProcessingNeighbour = null;
        Cell ProcessingNeighbourScript = null;
        int ProcessingNeighbourIndex = -1;

        GameObject PotentialForcedNeighbour = null;
        Cell PotentialForcedNeighbourScript = null;

        int TargetCellIndex = -1;
        int RootCellIndex = -1;

        bool TargetFound = false;

        //Setup!
        TargetCellIndex = target.GetComponent<Cell>().GetCellIndex();
        RootCellIndex = root.GetComponent<Cell>().GetCellIndex();

        FullLine = GridManagerScript.GetHorizontalCellsAt(RootCellIndex, dir);
        if (FullLine.Count > 0) 
        {
            for (int i = 0; i < FullLine.Count; i++)
            {
                ProcessingCell = FullLine[i];
                ProcessingCellScript = ProcessingCell.GetComponent<Cell>();
                ProcessingCellIndex = ProcessingCellScript.GetCellIndex();

                //If its the target.
                if (ProcessingCellIndex == TargetCellIndex)
                {
                    ProcessingCellScript.SetParentCell(root);
                    return true;
                }

                //Visualization.
                if(VisualizePath)
                    ProcessingCellScript.SetColor(Color.gray);

                //If its already in the closed list.
                if (IsInClosedList(ProcessingCell))
                    continue;

                //Neighbours checking.
                Neighbours =
                    GridManagerScript.GetHorizontalLineNeighbours(ProcessingCellIndex, dir);

                for (int j = 0; j < Neighbours.Count; j++)
                {
                    ProcessingNeighbour = Neighbours[j];
                    ProcessingNeighbourScript = ProcessingNeighbour.GetComponent<Cell>();
                    ProcessingNeighbourIndex = ProcessingNeighbourScript.GetCellIndex();

                    if (ProcessingNeighbourScript.GetCellType() == Cell.CellType.BLOCKED)
                    {
                        PotentialForcedNeighbour =
                            GridManagerScript.GetAdjacentHorizontalCellAt(ProcessingNeighbourIndex, dir);
                        
                        //If its not valid.
                        if (!PotentialForcedNeighbour)
                            continue;

                        //If its already in the open list.
                        if (IsInOpenList(PotentialForcedNeighbour))
                            continue;
                        if (IsInClosedList(PotentialForcedNeighbour))
                            continue;


                        PotentialForcedNeighbourScript = PotentialForcedNeighbour.GetComponent<Cell>();


                        if (PotentialForcedNeighbour.GetComponent<Cell>().GetParent() != null)
                            continue;


                        if (PotentialForcedNeighbourScript.GetCellType() == Cell.CellType.EMPTY)
                        {
                            if(ProcessingCellScript.GetParent() != root)
                                ProcessingCellScript.SetParentCell(root);

                            PotentialForcedNeighbourScript.SetParentCell(ProcessingCell);
                            PotentialForcedNeighbourScript.CalculateHCost(target.transform.position);

                            PotentialForcedNeighbourScript.CalculateGCost(root.transform.position);
                            PotentialForcedNeighbourScript.AddGCost(root.GetComponent<Cell>().GetGCost());

                            PotentialForcedNeighbourScript.CalculateFCost();

                            OpenList.Add(PotentialForcedNeighbour);
                            ClosedList.Add(ProcessingCell);

                            //Visualization.
                            if (VisualizePath)
                                PotentialForcedNeighbourScript.SetColor(Color.blue);
                        }
                    }                  
                }
            }
        } 

        return TargetFound;
    }
    private bool CheckVerticalLine(GameObject root, GameObject target, int dir)
    {
        List<GameObject> Neighbours;
        List<GameObject> FullLine;

        GameObject ProcessingCell = null;
        Cell ProcessingCellScript = null;
        int ProcessingCellIndex = -1;

        GameObject ProcessingNeighbour = null;
        Cell ProcessingNeighbourScript = null;
        int ProcessingNeighbourIndex = -1;

        GameObject PotentialForcedNeighbour = null;
        Cell PotentialForcedNeighbourScript = null;

        int TargetCellIndex = -1;
        int RootCellIndex = -1;
        bool TargetFound = false;

        //Setup!
        TargetCellIndex = target.GetComponent<Cell>().GetCellIndex();
        RootCellIndex = root.GetComponent<Cell>().GetCellIndex();

        FullLine = GridManagerScript.GetVerticalCellsAt(RootCellIndex, dir);
        if (FullLine.Count > 0)
        {
            for (int i = 0; i < FullLine.Count; i++)
            {
                ProcessingCell = FullLine[i];
                ProcessingCellScript = ProcessingCell.GetComponent<Cell>();
                ProcessingCellIndex = ProcessingCellScript.GetCellIndex();

                //If its the target.
                if (ProcessingCellIndex == TargetCellIndex)
                {
                    ProcessingCellScript.SetParentCell(root);
                    return true;
                }

                //Visualization.
                if(VisualizePath)
                    ProcessingCellScript.SetColor(Color.gray);

                //If its in the open list.
                if (IsInClosedList(ProcessingCell))
                    continue;


                //Neighbours checking.
                Neighbours =
                    GridManagerScript.GetVerticalLineNeighbours(ProcessingCellIndex);

                for (int j = 0; j < Neighbours.Count; j++)
                {
                    ProcessingNeighbour = Neighbours[j];
                    ProcessingNeighbourScript = ProcessingNeighbour.GetComponent<Cell>();
                    ProcessingNeighbourIndex = ProcessingNeighbourScript.GetCellIndex();

                    if (ProcessingNeighbourScript.GetCellType() == Cell.CellType.BLOCKED)
                    {
                        PotentialForcedNeighbour =
                            GridManagerScript.GetAdjacentVerticalCellAt(ProcessingNeighbourIndex, dir);

                        //If its not valid.
                        if (!PotentialForcedNeighbour)
                            continue;

                        //If its already in the open list.
                        if (IsInOpenList(PotentialForcedNeighbour))
                            continue;
                        if (IsInClosedList(PotentialForcedNeighbour))
                            continue;

                        PotentialForcedNeighbourScript = PotentialForcedNeighbour.GetComponent<Cell>();

                        if (PotentialForcedNeighbourScript.GetCellType() == Cell.CellType.EMPTY)
                        {
                            if (ProcessingCellScript.GetParent() != root)
                                ProcessingCellScript.SetParentCell(root);

                            PotentialForcedNeighbourScript.SetParentCell(ProcessingCell);
                            PotentialForcedNeighbourScript.CalculateHCost(target.transform.position);

                            PotentialForcedNeighbourScript.CalculateGCost(root.transform.position);
                            PotentialForcedNeighbourScript.AddGCost(root.GetComponent<Cell>().GetGCost());
                            PotentialForcedNeighbourScript.CalculateFCost();

                            OpenList.Add(PotentialForcedNeighbour);
                            ClosedList.Add(ProcessingCell);

                            //Visualization
                            if (VisualizePath)
                                PotentialForcedNeighbourScript.SetColor(Color.blue);
                        }
                    }
                }
            }
        }
        return TargetFound;
    }
    private bool CheckDiagonalLine(GameObject root, GameObject target, int dirX, int dirY)
    {
        List<GameObject> Neighbours = new List<GameObject>();

        GameObject CurrentDiagonalCell = root;
        Cell CurrentDiagonalCellScript = root.GetComponent<Cell>();
        int CurrentDiagonalCellIndex = CurrentDiagonalCellScript.GetCellIndex();

        GameObject ProcessingNeighbour = null;
        Cell ProcessingNeighbourScript = null;
        int ProcessingNeighbourIndex = -1;

        GameObject PotentialForcedNeighbour = null;
        Cell PotentialForcedNeighbourScript = null;

        bool TargetFound = false;

        Vector3 DirectionToParent;

        //To keep track if this check added forced neighbours, if so then create path to root.
        int OpenListInitialSize = OpenList.Count;
        int TargetCellIndex = target.GetComponent<Cell>().GetCellIndex();

        while (CurrentDiagonalCell)
        {
            CurrentDiagonalCell = GridManagerScript.GetDiagonalCellAt(CurrentDiagonalCellIndex, dirX, dirY);
            if (!CurrentDiagonalCell)
                break;

            CurrentDiagonalCellScript = CurrentDiagonalCell.GetComponent<Cell>();
            CurrentDiagonalCellIndex = CurrentDiagonalCellScript.GetCellIndex();

            //If its the target.
            if (CurrentDiagonalCellIndex == TargetCellIndex)
            {
                CurrentDiagonalCellScript.SetParentCell(root);
                return true; 
            }

            //If its blocked.
            if (CurrentDiagonalCellScript.GetCellType() == Cell.CellType.BLOCKED)
                break;

            //Visualization.
            if(VisualizePath)
                CurrentDiagonalCellScript.SetColor(Color.gray);


            CurrentDiagonalCellScript.CalculateGCost(root.transform.position);
            CurrentDiagonalCellScript.AddGCost(root.GetComponent<Cell>().GetGCost());

            //Forced Neighbours.
            if (!IsInClosedList(CurrentDiagonalCell))
            {
                Neighbours = GridManagerScript.GetDiagonalNeighbours(CurrentDiagonalCellIndex, -dirX, -dirY);
                for (int i = 0; i < Neighbours.Count; i++)
                {
                    ProcessingNeighbour = Neighbours[i];
                    ProcessingNeighbourScript = ProcessingNeighbour.GetComponent<Cell>();
                    ProcessingNeighbourIndex = ProcessingNeighbourScript.GetCellIndex();

                    if (ProcessingNeighbourScript.GetCellType() == Cell.CellType.BLOCKED)
                    {
                        //Calculates direction to parent.
                        DirectionToParent =
                            CalculateDirection(ProcessingNeighbour.transform.position, CurrentDiagonalCell.transform.position);

                        //Gets the correct potential forced neighbour.
                        if (DirectionToParent.x == 1 || DirectionToParent.x == -1)
                        {
                            PotentialForcedNeighbour =
                                GridManagerScript.GetAdjacentVerticalCellAt(ProcessingNeighbourIndex, dirY);
                        }
                        else if (DirectionToParent.z == 1 || DirectionToParent.z == -1)
                        {
                            PotentialForcedNeighbour =
                                GridManagerScript.GetAdjacentHorizontalCellAt(ProcessingNeighbourIndex, dirX);
                        }

                        //If its not valid.
                        if (!PotentialForcedNeighbour)
                            continue;

                        //If its already in the open list.
                        if (IsInOpenList(PotentialForcedNeighbour))
                            continue;
                        if (IsInClosedList(PotentialForcedNeighbour))
                            continue;


                        PotentialForcedNeighbourScript = PotentialForcedNeighbour.GetComponent<Cell>();


                        if (PotentialForcedNeighbourScript.GetCellType() == Cell.CellType.EMPTY)
                        {

                            /// Wasnt it supposed to connect to root of it finds any forced??
                            if (CurrentDiagonalCellScript.GetParent() != root)
                                CurrentDiagonalCellScript.SetParentCell(root);

                            PotentialForcedNeighbourScript.SetParentCell(CurrentDiagonalCell);
                            PotentialForcedNeighbourScript.CalculateHCost(target.transform.position);
                            PotentialForcedNeighbourScript.CalculateGCost();

                            PotentialForcedNeighbourScript.CalculateFCost();

                            OpenList.Add(PotentialForcedNeighbour);
                            ClosedList.Add(CurrentDiagonalCell);

                            OpenListInitialSize = OpenList.Count;

                            //Visualization.
                            if (VisualizePath)
                                PotentialForcedNeighbourScript.SetColor(Color.blue);
                        }
                    }
                }
            }

            //Ortho - Horizontal line check.
            TargetFound = CheckHorizontalLine(CurrentDiagonalCell, target, dirX);
            if (TargetFound)
            {
                if (CurrentDiagonalCellScript.GetParent() != root && !CurrentDiagonalCellScript.GetParent())
                    CurrentDiagonalCellScript.SetParentCell(root);

                return true;
            }
            if (OpenList.Count > OpenListInitialSize)
            {
                if (CurrentDiagonalCellScript.GetParent() != root && !CurrentDiagonalCellScript.GetParent())
                    CurrentDiagonalCellScript.SetParentCell(root);

                OpenListInitialSize = OpenList.Count;
            }

            //Ortho - Vertical line check.
            TargetFound = CheckVerticalLine(CurrentDiagonalCell, target, dirY);
            if (TargetFound)
            {
                if (CurrentDiagonalCellScript.GetParent() != root && !CurrentDiagonalCellScript.GetParent())
                    CurrentDiagonalCellScript.SetParentCell(root);

                return true;
            }
            if (OpenList.Count > OpenListInitialSize)
            {
                if (CurrentDiagonalCellScript.GetParent() != root && !CurrentDiagonalCellScript.GetParent())
                    CurrentDiagonalCellScript.SetParentCell(root);
            }
        }

        return TargetFound;
    }


    private Vector3 CalculateDirection(Vector3 start, Vector3 end)
    {
        Vector3 Direction = end - start;
        Vector3 NormalizedDirection = Vector3.Normalize(Direction);
        return NormalizedDirection;
    }
}
