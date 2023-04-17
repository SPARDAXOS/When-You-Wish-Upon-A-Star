using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum CellType
    {
        EMPTY,
        BLOCKED
    }

    public CellType Type = CellType.EMPTY;

    private int HCost = 0;
    private int GCost = 0;
    private int FCost = 0;
    public GameObject ParentRef = null;

    public int CellIndex = -1;

    private int CellWidth;
    private int CellHeight;

    private int OrthogonalMovementCost;
    private int DiagonalMovementCost;

    private SpriteRenderer SpriteRenderer_;

    public void SetCellIndex(int index)
    {
        CellIndex = index;
    }
    public void SetType(CellType type)
    {
        Type = type;
        UpdateTypeVisuals();
    }
    public void SetColor(Color color)
    {
        SpriteRenderer_.color = color;
    }
    public void SetParentCell(GameObject cell)
    {
        if (cell != null)
            ParentRef = cell;
        else
            Debug.LogError("SetParentCell was sent null!");
    }
    public void SetAsParent()
    {
        GCost = OrthogonalMovementCost;
    }

    public int GetCellIndex()
    {
        return CellIndex;
    }
    public CellType GetCellType()
    {
        return Type;
    }
    public int GetGCost()
    {
        return GCost;
    }
    public int GetFCost()
    {
        return FCost;
    }
    public GameObject GetParent()
    {
        return ParentRef;
    }

    public void CalculateHCost(Vector3 target)
    {
        int XVec = Mathf.RoundToInt(Mathf.Abs(target.x - transform.position.x));
        int ZVec = Mathf.RoundToInt(Mathf.Abs(target.z - transform.position.z));

        int HorizontalSteps = XVec / CellWidth;
        int VerticalSteps = ZVec / CellHeight;
        int PossibleDigonalSteps = 0;

        if (HorizontalSteps < VerticalSteps)
        {
            PossibleDigonalSteps = HorizontalSteps;
            VerticalSteps -= HorizontalSteps;
            HorizontalSteps = 0;
        }
        else if(HorizontalSteps > VerticalSteps)
        {
            PossibleDigonalSteps = VerticalSteps;
            HorizontalSteps -= VerticalSteps;
            VerticalSteps = 0;
        }
        else if (HorizontalSteps == VerticalSteps)
        {
            PossibleDigonalSteps = HorizontalSteps;
            HorizontalSteps = 0;
            VerticalSteps = 0;
        }

        int TotalCost = 0;
        TotalCost += (HorizontalSteps + VerticalSteps) * OrthogonalMovementCost;
        TotalCost += PossibleDigonalSteps * DiagonalMovementCost;

        HCost = TotalCost;
    }
    public void CalculateGCost()
    {
        if(ParentRef == null)
        {
            Debug.LogError("Cant calculate GCost due to parent being null!");
            return;
        }

        Cell ParentScript = ParentRef.GetComponent<Cell>();
        int TotalCost = 0;
        TotalCost += ParentScript.GetGCost();

        //Calculate if parent is ortho or diagonal // Calc both x and z. If one of them is 0 then it not diagonal?
        int XVec = Mathf.RoundToInt(Mathf.Abs(ParentRef.transform.position.x - transform.position.x));
        int ZVec = Mathf.RoundToInt(Mathf.Abs(ParentRef.transform.position.z - transform.position.z));
        //Debug.LogError("XVec : " + XVec);
        //Debug.LogError("ZVec : " + ZVec);

        // Alot of assumptions here! idk
        if(XVec == 0 && ZVec == 0)
        {
            Debug.LogError("XVec and ZVec are both zero!");
            return;
        }
        else if (XVec == 0 || ZVec == 0)
        {
            TotalCost += OrthogonalMovementCost;
        }
        else
            TotalCost += DiagonalMovementCost;

        GCost = TotalCost;
        //Debug.LogError("ParentIndex is : " + ParentScript.CellIndex);
        //Debug.LogError("CellIndex is : " + CellIndex);
        //Debug.LogError("GCost is : " + GCost + "At cell : " + CellIndex);
        //SetColor(Color.cyan);
    }
    public void CalculateGCost(Vector3 target)
    {
        int XVec = Mathf.RoundToInt(Mathf.Abs(target.x - transform.position.x));
        int ZVec = Mathf.RoundToInt(Mathf.Abs(target.z - transform.position.z));

        int HorizontalSteps = XVec / CellWidth;
        int VerticalSteps = ZVec / CellHeight;
        int PossibleDigonalSteps = 0;

        if (HorizontalSteps < VerticalSteps)
        {
            PossibleDigonalSteps = HorizontalSteps;
            VerticalSteps -= HorizontalSteps;
            HorizontalSteps = 0;
        }
        else if (HorizontalSteps > VerticalSteps)
        {
            PossibleDigonalSteps = VerticalSteps;
            HorizontalSteps -= VerticalSteps;
            VerticalSteps = 0;
        }
        else if (HorizontalSteps == VerticalSteps)
        {
            PossibleDigonalSteps = HorizontalSteps;
            HorizontalSteps = 0;
            VerticalSteps = 0;
        }

        int TotalCost = 0;
        TotalCost += (HorizontalSteps + VerticalSteps) * OrthogonalMovementCost;
        TotalCost += PossibleDigonalSteps * DiagonalMovementCost;

        //Test
        //if (ParentRef != null)
        //{
        //    Cell ParentScript = ParentRef.GetComponent<Cell>();
        //    int Difference = 0;
        //    Difference = Mathf.Abs(ParentScript.GetGCost() - TotalCost);
        //    TotalCost += Difference;
        //}


        GCost = TotalCost;
    }
    public void CalculateFCost()
    {
        FCost = HCost + GCost;
    }
    public void AddGCost(int cost)
    {
        GCost += cost;
    }

    public void CleanUp()
    {
        HCost = 0;
        GCost = 0;
        FCost = 0;
        ParentRef = null;
        UpdateTypeVisuals();
    }
    private void UpdateTypeVisuals()
    {
        switch (Type)
        {
            case CellType.EMPTY:
                {
                    SpriteRenderer_.color = Color.white;
                }break;
            case CellType.BLOCKED:
                {
                    SpriteRenderer_.color = Color.black;
                }
                break;
        }
    }
    public void SwitchType()
    {
        if(Type == CellType.EMPTY)
        {
            Type = CellType.BLOCKED;
            UpdateTypeVisuals();
        }
        else
        {
            Type = CellType.EMPTY;
            UpdateTypeVisuals();
        }
    }

    private void Awake()
    {
        SpriteRenderer_ = GetComponent<SpriteRenderer>();

        CellWidth = Mathf.RoundToInt(SpriteRenderer_.size.x);
        CellHeight = Mathf.RoundToInt(SpriteRenderer_.size.y);

        OrthogonalMovementCost = CellWidth;

        int DiagonalCost = (CellWidth * CellWidth) + (CellHeight * CellHeight);
        float DiagonalCostF = DiagonalCost;
        DiagonalCost = Mathf.RoundToInt(Mathf.Sqrt(DiagonalCostF));

        DiagonalMovementCost = DiagonalCost;
    }
}
