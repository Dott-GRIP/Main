using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject[] prefabs;
    public GameObject tilePrefab;
    public GameObject tileParent;
    public GameObject lightParent;
    public GameObject darkParent;
    public GameObject selectionPrefab;

    public List<GameObject> activePieces = new List<GameObject>();
    List<GameObject> selectTiles = new List<GameObject>();

    [Space]
    [Header("Input")]
    public KeyCode select = KeyCode.Mouse0;
    public KeyCode deselect = KeyCode.Mouse1;
    public KeyCode movePiece = KeyCode.Mouse0;

    [Space]
    [Header("Misc")]
    public float tileSize = 1;
    public float tileOffset = 0.5f;

    [Space]
    public Material lightMat;
    public Material darkMat;

    bool hasSelection;

    [Space]
    public Quaternion orientation = Quaternion.Euler(0, 180, 0);

    [Space]
    [Header("Vectors")]
    Vector2 currentHover = new Vector2(-1, -1);
    Vector2 selection = new Vector2(-1, -1);
    Vector2[] possibleMoves = new Vector2[] { new Vector2(-1, -1) };

    private void Start()
    {
        CreateBoard();
        CreatePieces();
    }

    private void Update()
    {
        UpdateSelection();
        if (!hasSelection)
        {
            if (Input.GetKeyDown(select))
            {
                if (currentHover.x >= 0 && currentHover.y >= 0)
                {
                    selection = currentHover;
                    hasSelection = true;
                    Debug.Log("> selected");
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(deselect))
            {
                selection = new Vector2(-1, -1);
                hasSelection = false;
                for (int i = 0; i < selectTiles.ToArray().Length; i++)
                {
                    Destroy(selectTiles[i]);
                }
                Debug.Log("> deselected");
            }
            else if (Input.GetKeyDown(movePiece))
            {

            }
            else
            {
                GameObject[] pcs = activePieces.ToArray();
                GameObject piece = null;
                for (int i = 0; i < pcs.Length; i++)
                {
                    if (selection.x == pcs[i].transform.position.x && selection.y == pcs[i].transform.position.z)
                    {
                        piece = pcs[i];
                        break;
                    }
                }
                if (piece != null)
                {
                    ShowAvailableMoves(piece);
                }
                else
                {
                    Debug.Log("> null");
                }
            }
        }
    }

    public void CreateBoard()
    {
        for (int x = 0; x < 8 * tileSize; x++)
        {
            for (int y = 0; y < 8 * tileSize; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, new Vector3(x + tileOffset, 0, y + tileOffset), Quaternion.identity, tileParent.transform);
                if ((x + y) % 2 == 0)
                {
                    newTile.GetComponent<MeshRenderer>().material = darkMat;
                }
                else
                {
                    newTile.GetComponent<MeshRenderer>().material = lightMat;
                }
            }
        }
    }

    public void CreatePieces()
    {
        //light
        //pawns
        for (int i = 0; i < 8; i++)
        {
            CreatePiece(0, i, 1);
        }
        //rooks
        CreatePiece(1, 0, 0);
        CreatePiece(1, 7, 0);
        //knights
        CreatePiece(2, 1, 0);
        CreatePiece(2, 6, 0);
        //bishops
        CreatePiece(3, 2, 0);
        CreatePiece(3, 5, 0);
        //queen
        CreatePiece(4, 3, 0);
        CreatePiece(5, 4, 0);
        //king

        //dark
        //pawns
        for (int i = 0; i < 8; i++)
        {
            CreatePiece(6, i, 6);
        }
        //rooks
        CreatePiece(7, 0, 7);
        CreatePiece(7, 7, 7);
        //knights
        CreatePiece(8, 1, 7);
        CreatePiece(8, 6, 7);
        //bishops
        CreatePiece(9, 2, 7);
        CreatePiece(9, 5, 7);
        //queen
        CreatePiece(10, 3, 7);
        //king
        CreatePiece(11, 4, 7);
    }

    public void CreatePiece(int index, int x, int y)
    {
        GameObject temp = Instantiate(prefabs[index]);
        activePieces.Add(temp);
        temp.transform.position = new Vector3(x + tileOffset, 0.1f, y + tileOffset);
        if (index >= 6)
        {
            temp.transform.rotation = orientation;
            temp.transform.SetParent(darkParent.transform);
        }
        else
        {
            temp.transform.rotation = Quaternion.identity;
            temp.transform.SetParent(lightParent.transform);
        }
    }

    public void UpdateSelection()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
        {
            Vector3 hitpoint = new Vector3((int)hit.point.x, (int)hit.point.y, (int)hit.point.z);
            currentHover = new Vector2((int)hit.point.x + tileOffset, (int)hit.point.z + tileOffset);
        }
        else
        {
            currentHover = new Vector2(-1, -1);
        }
    }

    public void ShowAvailableMoves(GameObject piece)
    {
        ConfigureAvailableMoves(piece);

        for (int i = 0; i < selectTiles.ToArray().Length; i++)
        {
            Destroy(selectTiles[i]);
        }
        selectTiles = new List<GameObject>();
        for (int i = 0; i < possibleMoves.Length; i++)
        {
            GameObject tmp = Instantiate(selectionPrefab);
            tmp.transform.position = new Vector3(possibleMoves[i].x, 0.15f, possibleMoves[i].y);
            tmp.transform.rotation = Quaternion.identity;
            selectTiles.Add(tmp);
        }
    }

    public void ConfigureAvailableMoves(GameObject piece)
    {
        ChessPiece cps = piece.GetComponent<ChessPiece>(); //Chess Piece Script

        int x = (int)piece.transform.position.x;
        int y = (int)piece.transform.position.z;

        List<Vector2> l = new List<Vector2>();

        switch (cps.m_PieceType)
        {
            case PieceType.Pawn:
                if (cps.colorId == 0)
                {
                    l.Add(new Vector2(x + tileOffset, y + 1 + tileOffset));
                    if (cps.hasMoved)
                    {
                        l.Add(new Vector2(x, 3));
                    }
                }
                else
                {
                    l.Add(new Vector2(x + tileOffset, y - 1 + tileOffset));
                    if (cps.hasMoved)
                    {
                        l.Add(new Vector2(x, 4));
                    }
                }
                break;
            case PieceType.Rook:
                List<float> xVals = new List<float>();
                List<float> yVals = new List<float>();

                for (int a = 0; a < 7; a++)
                {
                    if (a + tileOffset != x)
                    {
                        if (x > 4)
                        {
                            xVals.Add(a + tileOffset);
                        }
                        else
                        {
                            xVals.Add(a + 1 + tileOffset);
                        }
                    }
                }
                for (int b = 0; b < 7; b++)
                {
                    if (b + tileOffset != y)
                    {
                        if (y > 4)
                        {
                            yVals.Add(b + tileOffset);
                        }
                        else
                        {
                            yVals.Add(b + 1 + tileOffset);
                        }
                    }
                }
                for (int i = 0; i < xVals.ToArray().Length; i++)
                {
                    l.Add(new Vector2(xVals[i], y + tileOffset));
                }
                for (int i = 0; i < yVals.ToArray().Length; i++)
                {
                    l.Add(new Vector2(x + tileOffset, yVals[i]));
                }
                break;
            case PieceType.Knight:
                l.Add(new Vector2(x - 2, y - 1));
                l.Add(new Vector2(x - 2, y + 1));
                l.Add(new Vector2(x - 1, y - 2));
                l.Add(new Vector2(x - 1, y + 2));
                l.Add(new Vector2(x + 1, y - 2));
                l.Add(new Vector2(x + 1, y + 2));
                l.Add(new Vector2(x + 2, y - 1));
                l.Add(new Vector2(x + 2, y + 1));
                break;
            case PieceType.Bishop:
                break;
            case PieceType.Queen:
                break;
            case PieceType.King:
                break;
        }
        possibleMoves = l.ToArray();

        if (cps.m_PieceType == PieceType.Knight)
        {
            for (int i = 0; i < possibleMoves.Length; i++)
            {
                possibleMoves[i].x += tileOffset;
                possibleMoves[i].y += tileOffset;
            }
        }
    }

    public bool IsOccupied(Vector2 pos)
    {
        for (int i = 0; i < activePieces.ToArray().Length; i++)
        {
            if (pos.x == activePieces[i].transform.position.x && pos.y == activePieces[i].transform.position.z)
            {
                return true;
            }
        }
        return false;
    }
}
