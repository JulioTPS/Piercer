using UnityEngine;
using System.Collections.Generic;

public class PieceBag
{
    private readonly List<Piece> pieceReferences;
    private List<Piece> bag;
    private Piece previousPiece;
    private const float PIECE_REPEAT_CHANCE = 0.25f;

    public PieceBag(List<Piece> pieceReferences)
    {
        this.pieceReferences = new List<Piece>(pieceReferences);
        FillBag();
    }

    private void FillBag()
    {
        bag = new List<Piece>(pieceReferences);
        do
        {
            for (int i = bag.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (bag[randomIndex], bag[i]) = (bag[i], bag[randomIndex]);
            }
        } while (bag[0] == previousPiece && Random.value > PIECE_REPEAT_CHANCE);

        previousPiece = bag[0];
    }

    public Piece GetNewPiece()
    {
        if (bag.Count == 0)
            FillBag();
        Piece piece = bag[0];
        bag.RemoveAt(0);
        return piece;
    }
}
