using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject[] prefabs;
    public GameObject tilePrefab;
    public GameObject tileParent;
    public GameObject lightParent;
    public GameObject darkParent;

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

    [Space]
    public bool hasSelection;

    [Space]
    public Quaternion orientation = Quaternion.Euler(0, 180, 0);

    [Space]
    [Header("Vectors")]
    public Vector2 currentHover = new Vector2(-1, -1);
    public Vector2 selection = new Vector2(-1, -1);

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
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(deselect))
            {
                selection = new Vector2(-1, -1);
                hasSelection = false;
            }
            else if (Input.GetKeyDown(movePiece))
            {

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
            currentHover = new Vector2((int)hit.point.x, (int)hit.point.z);
        }
        else
        {
            currentHover = new Vector2(-1, -1);
        }
    }

    public void MovePiece(GameObject piece, Vector2 position)
    {

    }
}
