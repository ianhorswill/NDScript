using static NDScript.NDScript;

namespace NDScriptTests
{
    [TestClass()]
    public class IntegrationTests
    {
        [TestMethod]
        public void PersonalityGeneratorTest()
        {
            for (var i=0; i < 20; i++)
                Console.WriteLine(ProgramOutput(@"var traits = [ ""bookworm"", ""outgoing"", ""shy"", ""goody two-shoes"" ];
var nogoods = [ [""shy"", ""outgoing""] ];

function violates(set, nogood) {
	foreach (x in nogood)
		if (!contains(x, set)) return false;
	return true;
}

function makePersonality() {
	var p = setOf(chooseElement(traits), chooseElement(traits), chooseElement(traits));
	if (sizeOf(p) != 3) fail;
	foreach (nogood in nogoods)
		if (violates(p, nogood)) fail;
	return p;
}
print(makePersonality());
"));
        }

        [TestMethod]
        public void MineBumbler()
        {
            var s = ProgramOutput(bumbler);
            Console.WriteLine(s);
            Assert.IsTrue(s.EndsWith("\n4 4"));
        }

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
        public void WFCTest()
        {
            var o = ProgramOutput(WFC);
            Console.WriteLine(o);
        }

        private static readonly string WFC = @"
var beach = ""beach"";
var grass = ""grass"";
var water = ""water"";
var g = makeGrid(4, 4, setOf(beach, grass, water));
var compatible = relation(
   [beach, grass], [beach, grass], [beach, beach],
   [water, water], [water, beach],
   [grass, grass], [grass, beach]);

var remaining = nonsingletonPositionsOf(g);
function solve() {
    while (!isEmpty(remaining)) {
      var p = chooseElement(remaining);
      var tile = chooseElement(g[p]);
      narrowTo(p, setOf(tile));
      remaining = nonsingletonPositionsOf(g);
    }
}

function narrowTo(position, set) {
  var old = g[position];
  var new = intersection(g[position], set);
  if (old != new) {
     g[position] = new;
     var neighborCompatible = leftImage(compatible, new);
     foreach (n in neighborsOf(position, g))
       narrowTo(n, neighborCompatible);
  }
}

solve();
printGrid(g);
";
    }
}