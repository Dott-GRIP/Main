using UnityEngine;

public enum PieceType
{
    Pawn = 0,
    Rook = 1,
    Knight = 2,
    Bishop = 3,
    Queen = 4,
    King = 5
}
public class ChessPiece : MonoBehaviour
{
    public PieceType m_PieceType = PieceType.Pawn;
    public bool hasMoved = false;
    [Range(0, 1)]
    public int colorId = 0;
}
