using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Game Objects")]
    /*
        light-
            -pawn
            -rook
            -knight
            -bishop
            -queen
            -king
        dark-
            -pawn
            -rook
            -knight
            -bishop
            -queen
            -king
    */
    public GameObject[] prefabs; //<!> REQUIRED TO BE IN ORDER <!>
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
        GameObject piece = null;

        //set 'piece'to the active selection
        for (int i = 0; i < activePieces.ToArray().Length; i++)
        {
            if (selection.x == activePieces[i].transform.position.x && selection.y == activePieces[i].transform.position.z)
            {
                piece = activePieces[i];
                break;
            }
        }

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
                DestroySelectionTiles();
                Debug.Log("> deselected");
            }
            else if (Input.GetKeyDown(movePiece))
            {
                if (piece != null) //null-check
                {
                    for (int i = 0; i < possibleMoves.Length; i++) //for every possible move,
                    {
                        if (currentHover.x == possibleMoves[i].x && currentHover.y == possibleMoves[i].y && !IsOccupied(currentHover)) //if the hovered space is not occupied
                        {
                            if (piece.GetComponent<ChessPiece>().colorId == 0) //if the current piece is light
                            {
                                Vector3 current = new Vector3(currentHover.x - piece.transform.position.x, 0, currentHover.y - piece.transform.position.z);
                                piece.transform.Translate(current);
                            }
                            else //the current piece is dark
                            {
                                Vector3 current = new Vector3(piece.transform.position.x - currentHover.x, 0, piece.transform.position.z - currentHover.y);
                                piece.transform.Translate(current);
                            }

                            //deselect piece
                            hasSelection = false;
                            DestroySelectionTiles();
                            Debug.Log("> deselected");
                            break;
                        }
                    }
                }
            }
            else
            {
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
                if ((x + y) % 2 == 0) //if 'x + y' is evenly divisible by 2 (an even number)
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

        //if the 'index' (prefab id) is greater than six then the piece is guaranteed to be dark (based on manual input)
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
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity)) //if the mouse is hovering over any tile:
        {
            Vector3 hitpoint = new Vector3((int)hit.point.x, (int)hit.point.y, (int)hit.point.z);
            currentHover = new Vector2((int)hit.point.x + tileOffset, (int)hit.point.z + tileOffset);
        }
        else //the mouse is not, so set the currentHover variable to null, or in this case (-1, -1) because vectors can't contain 'null' values.
        {
            currentHover = new Vector2(-1, -1);
        }
    }

    public void ShowAvailableMoves(GameObject piece)
    {
        ConfigureAvailableMoves(piece); //find all possible moves, and add them to the 'possibleMoves' array.

        DestroySelectionTiles();
        selectTiles = new List<GameObject>(); //reset 'selectTiles' variable
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
                for (int i = 0; i < 7; i++)
                {
                    l.Add(new Vector2(x + i, y + i));
                    l.Add(new Vector2(x + i, y - i));
                    l.Add(new Vector2(x - i, y + i));
                    l.Add(new Vector2(x - i, y - i));
                }
                l.Remove(new Vector2(x, y));
                break;
            case PieceType.Queen:
                break;
            case PieceType.King:
                break;
        }

        //remove 'out-of-bounds' moves
        for (int i = 0; i < l.ToArray().Length; i++)
        {
            if (!IsInBounds(l[i]))
            {
                l.Remove(l[i]);
            }
        }

        //remove occupied moves, unless the occupant can be killed by doing said move.
        for (int i = 0; i < l.ToArray().Length; i++)
        {
            GameObject occupant = GetOccupant(l[i]);
            if (occupant != null && piece.GetComponent<ChessPiece>().colorId == occupant.GetComponent<ChessPiece>().colorId)
            {
                l.Remove(l[i]);
            }
        }

        possibleMoves = l.ToArray();

        if (cps.m_PieceType == PieceType.Knight || cps.m_PieceType == PieceType.Bishop)
        {
            for (int i = 0; i < possibleMoves.Length; i++)
            {
                possibleMoves[i].x += tileOffset;
                possibleMoves[i].y += tileOffset;
            }
        }
    }

    public GameObject GetOccupant(Vector2 pos)
    {
        for (int i = 0; i < activePieces.ToArray().Length; i++)
        {
            if (activePieces[i].transform.position.x == pos.x && activePieces[i].transform.position.y == pos.y)
            {
                return activePieces[i];
            }
        }
        return null;
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

    public bool IsInBounds(Vector2 pos)
    {
        if (pos.x >= 0 || pos.x >= tileSize * 8 || pos.y <= 0 || pos.y >= tileSize * 8)
            return true;
        return false;
    }

    public void DestroySelectionTiles()
    {
        for (int j = 0; j < selectTiles.ToArray().Length; j++)
        {
            Destroy(selectTiles[j]);
        }
    }
}
