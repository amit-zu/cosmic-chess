# Cosmic Chess

**Cosmic Chess** is an innovative chess game that combines traditional gameplay with exciting new variants. Featuring themes like Future Chess, Pirate Chess, and Medieval Chess, the game introduces unique mechanics and unpredictable features to make each match more engaging and fun.

## Contributing

### Adding pieces

Here's a list of things that need to be done in order to add a piece to the game.

1. Add a new scene of type Piece.
2. Change it's name to the name of the piece you're adding.
3. Add a CollisionBody2D under the piece node, and give it a rectangle shape (make sure the rectangle size is equal to the other pieces).
4. Create a script that inherits from Piece and connect it to the piece.
5. Connect all the relevant signals to the piece (on mouse entered, on mouse exited, on body entered, on body exited).
6. Set the piece's sprite in the \_Ready() function, you can see how it's done in other pieces.
7. Implement the GetLegalSquares(), GetTakingSquares(), GetCheckingSquares() according to the piece's logic. In the end of GetLegalSquares() and GetTakingSquares(), use the function FilterLegalSquaresForCheck() to filter the moves so that only the moves that prevent check are applied.
8. If the piece is part of the starting position, add it to the game on the correct square. Make sure you add the piece under the node "Pieces".
9. Set the color and the initial file and rank for the piece. If the piece is white, set it to black and then set it to white again (weird Godot bug). Same with pieces that start on the A file or on the first rank.
10. Add the piece's symbol to BoardItemSymbols.cs.
11. Add the piece to GetLegalSquaresForType() and GetTakingSquaresForType() in Piece.cs.
12. You're done! Test your piece a little bit and commit your changes.
