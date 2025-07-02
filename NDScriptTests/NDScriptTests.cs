namespace NDScript
{
    [TestClass()]
    public class NDScriptTests
    {
        private static string bumbler = @"
// Find a way from the top-left corner to the bottom-right
// using only down- and right-moves.  Moving off the board
// Or hitting a bomb (occupied square) is failure.
function solve()
{
    while (!done()) choose right(); or down();
}

// Design of the map
var s = "" "";
var X = ""X"";
var size = 5;
var map = [
   [s, s, s, s, s],
   [s, s, s, s, s],
   [s, s, s, X, X],
   [s, s, s, s, s],
   [s, s, s, X, s]];

var x = 0;
var y = 0;

function right() {
  map[y][x] = ""-"";
  x = x+1;
  if (x == size || map[y][x] != "" "") fail;
  map[y][x] = ""*"";
}

function down() {
  map[y][x] = ""|"";
  y = y+1;
  if (y == size || map[y][x] != "" "") fail;
  map[y][x] = ""*"";
}

function done()
{
    return x == size-1 && y == size-1;
}

function PrintMap()
{
   printLine(""▒▒▒▒▒▒▒"");
   foreach (row in map) {
     print(""▒"");
     foreach (tile in row) print(tile);
     printLine(""▒"");
   }
   printLine(""▒▒▒▒▒▒▒"");
}



solve();
PrintMap();

print(x,"" "",y);
";

        [TestMethod]
        public void MineBumbler()
        {
            var s = NDScript.ProgramOutput(bumbler);
            Console.WriteLine(s);
            Assert.IsTrue(s.EndsWith("\n4 4"));
        }
    }
}