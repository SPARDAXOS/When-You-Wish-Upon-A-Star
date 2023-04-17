using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private float BlockedCellProbability = 0.20f;


    private int WidthCellsCount = 0;
    private int HeightCellsCount = 0;
    private float CellWidth = 0.0f;
    private float CellHeight = 0.0f;

    private float GridRightEdge = 0;
    private float GridLowerEdge = 0;
    private float GridUpperEdge = 0;
    private float GridLeftEdge = 0;

    private Vector3 OriginPoint = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 CenterPoint = new Vector3(0.0f, 0.0f, 0.0f);

    private GameObject CellSample = null;
    private List<GameObject> AllCells;

    public void LoadResources()
    {
        CellSample = Resources.Load<GameObject>("Cell");
        if (CellSample == null)
            Debug.LogError("Loading resources at Grid has failed! - Cell");
    }
    public void SetOriginPoint(Vector3 point)
    {
        OriginPoint = point;
    }

    public void CreateGrid(int width, int height)
    {
        AllCells = new List<GameObject>(width * height);

        WidthCellsCount = width;
        HeightCellsCount = height;

        SpriteRenderer CellSpriteRenderer_ = CellSample.GetComponent<SpriteRenderer>();
        CellWidth = CellSpriteRenderer_.size.x;
        CellHeight = CellSpriteRenderer_.size.y;

        GameObject NewCell;
        Cell NewCellScript;
        float Rand;
        for (int i = 0; i < HeightCellsCount; i++)
        {
            for (int j = 0; j < WidthCellsCount; j++)
            {
                NewCell = Instantiate(CellSample);
                NewCellScript = NewCell.GetComponent<Cell>();
                //Set Specifics!

               NewCellScript.SetCellIndex((i * WidthCellsCount) + j);

                Rand = Random.Range(0.0f, 1.0f);
                if (Rand <= BlockedCellProbability)
                    NewCellScript.SetType(Cell.CellType.BLOCKED);
                else
                    NewCellScript.SetType(Cell.CellType.EMPTY);


                NewCell.transform.position = OriginPoint + new Vector3(CellWidth * j, 0.0f, -CellHeight * i);
                AllCells.Add(NewCell);
            }
        }

        CalculateGridEdges();
        CalculateCenterPoint();
    }

    public Vector3 GetCenterPoint()
    {
        return CenterPoint;
    }

    public GameObject GetCellAt(int index)
    {
        if (!IsCellValid(index))
            return null;
        return AllCells[index];
    }

    public GameObject GetDiagonalCellAt(int index, int x, int y)
    {
        if (!IsCellValid(index))
        {
            Debug.LogWarning("Cell not valid at GetDiagonalCellAt : " + index + " " + x + " " + y);
            return null;
        }
        if(x > 1 || x < -1)
        {
            Debug.LogError("Invalid direction at GetDiagonalCellAt X : " + x);
            return null;
        }
        if (y > 1 || y < -1)
        {
            Debug.LogError("Invalid direction at GetDiagonalCellAt Y : " + y);
            return null;
        }

        int WantedCellIndex = index + ((y * WidthCellsCount) + x);
        GameObject WantedCell = GetCellAt(WantedCellIndex);
        if (!IsCellValid(WantedCellIndex))
            return null;

        if (x == 1)
        {
            if (WantedCellIndex % WidthCellsCount == 0)
                return null;
        }
        else if (x == -1) 
        {
            if(WantedCellIndex % WidthCellsCount == (WidthCellsCount - 1))
                return null;
        }


        return WantedCell;
    }

    public GameObject GetRandomCell()
    {
        // Test this later plz
        int index = -1;
        while (!IsCellValid(index))
            index = Random.Range(0, (WidthCellsCount * HeightCellsCount - 1));

        return AllCells[index];
    }
    public GameObject GetRandomEmptyCell()
    {
        // I dont like this! Change it


        int Index = -1;
        int MaximumTries = 52;
        int CurrentTries = 0;
        bool Done = false;
        Cell CellScript;
        while (!Done)
        {
            Index = Random.Range(0, (WidthCellsCount * HeightCellsCount - 1));
            if (IsCellValid(Index))
            {
                CellScript = AllCells[Index].GetComponent<Cell>();
                if(CellScript.GetCellType() == Cell.CellType.EMPTY)
                    return AllCells[Index];
            }
            CurrentTries++;
            if(CurrentTries > MaximumTries)
            {
                Debug.LogError("Maximum tries amount was reached and no cell was found!");
                return null;
            }
                
        }
            

        return null;
    }

    //UnblockedCells
    public List<GameObject> GetAdjacentCellsAt(int index)
    {
        if (!IsCellValid(index))
        {
            Debug.LogError("Invalid index at GetAdjacentCellsAt! : " + index);
            return null;
        }

        List<GameObject> AvailableCells = new List<GameObject>();
        int CurrentIndex = -1;
        Cell CellScript;

        // Right Left Up Down
        CurrentIndex = index + 1;
        if (IsCellValid(CurrentIndex))
        {
            CellScript = AllCells[CurrentIndex].GetComponent<Cell>();
            // The fact that im checking against not blocked has implications. Think about it later.
            if ((CurrentIndex + 1) % WidthCellsCount != 1 && CellScript.GetCellType() != Cell.CellType.BLOCKED)
                AvailableCells.Add(AllCells[CurrentIndex]);
        }
        // Weird as fuc way of checking if edge
        CurrentIndex = index - 1;
        if (IsCellValid(CurrentIndex))
        {
            CellScript = AllCells[CurrentIndex].GetComponent<Cell>();
            if ((CurrentIndex + 1) % WidthCellsCount != 0 && CellScript.GetCellType() != Cell.CellType.BLOCKED)
                AvailableCells.Add(AllCells[CurrentIndex]);
        }

        CurrentIndex = index - WidthCellsCount * 1;
        if (IsCellValid(CurrentIndex))
        {
            CellScript = AllCells[CurrentIndex].GetComponent<Cell>();
            if(CellScript.GetCellType() != Cell.CellType.BLOCKED)
                AvailableCells.Add(AllCells[CurrentIndex]);
        }

        CurrentIndex = index + WidthCellsCount * 1;
        if (IsCellValid(CurrentIndex))
        {
            CellScript = AllCells[CurrentIndex].GetComponent<Cell>();
            if (CellScript.GetCellType() != Cell.CellType.BLOCKED)
                AvailableCells.Add(AllCells[CurrentIndex]);
        }

        // TopRight DownRight TopLeft DownLeft
        CurrentIndex = (index - WidthCellsCount) + 1;
        if (IsCellValid(CurrentIndex))
        {
            CellScript = AllCells[CurrentIndex].GetComponent<Cell>();
            if ((CurrentIndex + 1) % WidthCellsCount != 1 && CellScript.GetCellType() != Cell.CellType.BLOCKED)
                AvailableCells.Add(AllCells[CurrentIndex]);
        }

        CurrentIndex = (index + WidthCellsCount) + 1;
        if (IsCellValid(CurrentIndex))
        {
            CellScript = AllCells[CurrentIndex].GetComponent<Cell>();
            if ((CurrentIndex + 1) % WidthCellsCount != 1 && CellScript.GetCellType() != Cell.CellType.BLOCKED)
                AvailableCells.Add(AllCells[CurrentIndex]);
        }

        CurrentIndex = (index - WidthCellsCount) - 1;
        if (IsCellValid(CurrentIndex))
        {
            CellScript = AllCells[CurrentIndex].GetComponent<Cell>();
            if ((CurrentIndex + 1) % WidthCellsCount != 0 && CellScript.GetCellType() != Cell.CellType.BLOCKED)
                AvailableCells.Add(AllCells[CurrentIndex]);
        }

        CurrentIndex = (index + WidthCellsCount) - 1;
        if (IsCellValid(CurrentIndex))
        {
            CellScript = AllCells[CurrentIndex].GetComponent<Cell>();
            if ((CurrentIndex + 1) % WidthCellsCount != 0 && CellScript.GetCellType() != Cell.CellType.BLOCKED)
                AvailableCells.Add(AllCells[CurrentIndex]);
        }

        if (AvailableCells.Count == 0)
            return null;

        return AvailableCells;
    }


    public List<GameObject> GetHorizontalCellsAt(int index, int direction)
    {
        if (!IsCellValid(index))
        {
            Debug.LogError("Invalid cell sent to GetHorizontalCellsAt");
            return null;
        }

        List<GameObject> Cells = new List<GameObject>();
        GameObject CurrentCell = GetCellAt(index); 
        Cell CurrentCellScript = CurrentCell.GetComponent<Cell>();


        //In case index was at the edge.
        if (direction == 1)
        {
            if (CurrentCellScript.GetCellIndex() % WidthCellsCount == WidthCellsCount - 1)
            {
                return Cells;
            }
        }
        else if (direction == -1)
        {
            if (CurrentCellScript.GetCellIndex() % WidthCellsCount == 0)
            {
                return Cells;
            }
        }



        while (CurrentCell)
        {
            CurrentCell = GetCellAt(CurrentCellScript.GetCellIndex() + direction);
            if (!CurrentCell)
                break;
            CurrentCellScript = CurrentCell.GetComponent<Cell>();

            if (direction == 1)
            {
                if (CurrentCellScript.GetCellIndex() % WidthCellsCount == WidthCellsCount - 1)
                {
                    Cells.Add(CurrentCell);
                    break;
                }
            }
            else if(direction == -1)
            {
                if (CurrentCellScript.GetCellIndex() % WidthCellsCount == 0)
                {
                    Cells.Add(CurrentCell);
                    break;
                }
            }

            if (CurrentCellScript.GetCellType() == Cell.CellType.BLOCKED)
                break;
           
            Cells.Add(CurrentCell);
        }
        return Cells;
    }
    public List<GameObject> GetVerticalCellsAt(int index, int direction)
    {
        if (!IsCellValid(index))
        {
            Debug.LogError("Invalid cell sent to GetVerticalCellsAt");
            return null;
        }

        List<GameObject> Cells = new List<GameObject>();
        GameObject CurrentCell = GetCellAt(index);
        Cell CurrentCellScript = CurrentCell.GetComponent<Cell>();

        while (CurrentCell)
        {
            CurrentCell = GetCellAt(CurrentCellScript.GetCellIndex() + (WidthCellsCount * direction));
            if (!CurrentCell)
                break;
            CurrentCellScript = CurrentCell.GetComponent<Cell>();

            if (CurrentCellScript.GetCellType() == Cell.CellType.BLOCKED)
                break;

            Cells.Add(CurrentCell);
        }
        return Cells;
    }


    public GameObject GetAdjacentHorizontalCellAt(int index, int dir)
    {
        if (!IsCellValid(index))
        {
            Debug.LogWarning("Invalid index at GetAdjacentHorizontalCellAt");
            return null;
        }

        
        GameObject WantedCell = GetCellAt(index + dir);
        if (!IsCellValid(index + dir))
        {
            Debug.LogWarning("Invalid index at GetAdjacentHorizontalCellAt " + index);
            return null;
        }

        int WantedCellIndex = WantedCell.GetComponent<Cell>().GetCellIndex();

        if (dir == 1)
        {
            if (WantedCellIndex % WidthCellsCount == 0)
                return null;
        }
        else if (dir == -1)
        {
            if (WantedCellIndex % WidthCellsCount == (WidthCellsCount - 1))
                return null;
        }

        return WantedCell;
    }
    public GameObject GetAdjacentVerticalCellAt(int index, int dir)
    {
        if (!IsCellValid(index))
        {
            Debug.LogWarning("Invalid index at GetAdjacentHorizontalCellAt");
            return null;
        }

        GameObject WantedCell = GetCellAt(index + (dir * WidthCellsCount));
        if (!IsCellValid(index + dir * WidthCellsCount))
            return null;

        int WantedCellIndex = WantedCell.GetComponent<Cell>().GetCellIndex();

        return WantedCell;
    }



    public List<GameObject> GetHorizontalLineNeighbours(int index, float dir)
    {
        if (!IsCellValid(index))
        {
            return null;
        }

        List<GameObject> Cells = new List<GameObject>();
        GameObject WantedCell;
        int WantedCellIndex;
        if (dir == 1 || dir == -1)
        {
            WantedCellIndex = index - WidthCellsCount;
            if (IsCellValid(WantedCellIndex))
            {
                WantedCell = GetCellAt(WantedCellIndex);
                Cells.Add(WantedCell);
            }

            WantedCellIndex = index + WidthCellsCount;
            if (IsCellValid(WantedCellIndex))
            {
                WantedCell = GetCellAt(WantedCellIndex);
                Cells.Add(WantedCell);
            }
        }

        return Cells;
    }
    public List<GameObject> GetVerticalLineNeighbours(int index)
    {
        if (!IsCellValid(index))
        {
            return null;
        }
        List<GameObject> Cells = new List<GameObject>();
        GameObject WantedCell;
        int WantedCellIndex;
        WantedCellIndex = index + 1;
        if (IsCellValid(WantedCellIndex) && WantedCellIndex % WidthCellsCount != 0)
        {
            WantedCell = GetCellAt(WantedCellIndex);
            Cells.Add(WantedCell);
        }
        WantedCellIndex = index - 1;
        if (IsCellValid(WantedCellIndex) && WantedCellIndex % WidthCellsCount != (WidthCellsCount - 1))
        {
            WantedCell = GetCellAt(WantedCellIndex);
            Cells.Add(WantedCell);
        }

        return Cells;
    }
    public List<GameObject> GetDiagonalNeighbours(int index, int dirX, int dirZ)
    {
        if (!IsCellValid(index))
        {
            return null;
        }

        List<GameObject> Cells = new List<GameObject>();
        GameObject WantedCell;
        int WantedCellIndex;

        if(dirX == 1)
        {
            WantedCellIndex = index + 1;
            if (IsCellValid(WantedCellIndex))
            {
                if (WantedCellIndex % WidthCellsCount != 0)
                {
                    WantedCell = GetCellAt(WantedCellIndex);
                    Cells.Add(WantedCell);
                }
            }
        }
        else if(dirX == -1)
        {
            WantedCellIndex = index - 1;
            if (IsCellValid(WantedCellIndex))
            {
                if (WantedCellIndex % WidthCellsCount != (WidthCellsCount - 1))
                {
                    WantedCell = GetCellAt(WantedCellIndex);
                    Cells.Add(WantedCell);
                }
            }
        }

        if (dirZ == 1)
        {
            WantedCellIndex = index + WidthCellsCount;
            if (IsCellValid(WantedCellIndex))
            {
                WantedCell = GetCellAt(WantedCellIndex);
                Cells.Add(WantedCell);
            }
        }
        else if (dirZ == -1)
        {
            WantedCellIndex = index - WidthCellsCount;
            if (IsCellValid(WantedCellIndex))
            {
                WantedCell = GetCellAt(WantedCellIndex);
                Cells.Add(WantedCell);
            }
        }

        return Cells;
    }


    private bool IsCellValid(int index)
    {
        if (AllCells.Count == 0)
            return false;

        if (index < 0)
            return false;
        else if (index >= AllCells.Count)
            return false;

        return true;
    }


    public int GetCellsCount()
    {
        return AllCells.Count;
    }

    public void AdjacencyTest()
    {
        GameObject RandomCell = GetRandomEmptyCell();
        Debug.LogError("The Random Cell is : " + RandomCell.GetComponent<Cell>().GetCellIndex());
        List<GameObject> Cel = GetAdjacentCellsAt(24);
        for (int i = 0; i < Cel.Count; i++)
        {
            Debug.LogError(Cel[i].GetComponent<Cell>().GetCellIndex());
            Cel[i].GetComponent<Cell>().SetColor(Color.green);
        }
    }
    public void HCostTest()
    {
        GameObject RandomCell1 = GetRandomCell();
        Debug.LogError("Random Cell 1 is : " + RandomCell1.GetComponent<Cell>().GetCellIndex());
        RandomCell1.GetComponent<Cell>().SetColor(Color.red);
        GameObject RandomCell2 = GetRandomCell();
        Debug.LogError("Random Cell 2 is : " + RandomCell2.GetComponent<Cell>().GetCellIndex());
        RandomCell2.GetComponent<Cell>().SetColor(Color.red);

        RandomCell1.GetComponent<Cell>().CalculateHCost(RandomCell2.transform.position);

    }

    private void CalculateGridEdges()
    {
        GridUpperEdge = OriginPoint.z + CellHeight / 2;
        GridLeftEdge = OriginPoint.x - CellWidth / 2;
        GridRightEdge = OriginPoint.x + (CellWidth * (WidthCellsCount - 1)) + CellWidth / 2;
        GridLowerEdge = OriginPoint.z - (CellHeight * (HeightCellsCount - 1)) - CellHeight / 2;
    }
    private void CalculateCenterPoint()
    {
        float X = (GridRightEdge - GridLeftEdge) * 0.5f;
        float Z = (GridLowerEdge - GridUpperEdge) * 0.5f;
        Vector3 GridCenter = new Vector3(X, OriginPoint.y, Z);
        CenterPoint = GridCenter;
    }

    public GameObject GetCellAtWorldPos(Vector3 pos)
    {
        GameObject WantedCell = null;
        Vector3 CellPos;
        for(int i = 0; i < AllCells.Count; i++)
        {
            CellPos = AllCells[i].transform.position;
            if (CellPos.x + CellWidth/2 > pos.x && CellPos.x - CellWidth/2 < pos.x)
            {
                if(CellPos.z + CellHeight/2 > pos.z && CellPos.z - CellHeight/2 < pos.z)
                    return AllCells[i];
            }
        }
        

        return WantedCell;
    }
    public void SwitchCellTypeAt(Vector3 pos)
    {
        GameObject WantedCell = GetCellAtWorldPos(pos);
        if (!WantedCell)
        {
            return;
        }

        Cell WantedCellScript = WantedCell.GetComponent<Cell>();
        WantedCellScript.SwitchType();
    }
    public void CleanUpAllCells()
    {
        for(int i = 0; i < AllCells.Count; i++)
        {
            AllCells[i].GetComponent<Cell>().CleanUp();
        }
    }
}
