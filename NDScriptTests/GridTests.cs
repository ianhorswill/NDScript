using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NDScript.NDScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDScriptTests
{
    [TestClass()]
    public class GridTests
    {
        //[TestMethod]
        //public void 

        [TestMethod]
        public void MineBumbler()
        {
            var s = ProgramOutput(bumbler);
            Console.WriteLine(s);
            Assert.AreEqual("4 4", s);
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
var map = grid([
   [s, s, s, s, s],
   [s, s, s, s, s],
   [s, s, s, X, X],
   [s, s, s, s, s],
   [s, s, s, X, s]]);

var x = 0;
var y = 0;

function right() {
  map[x,y] = ""-"";
  x = x+1;
  if (x == size || map[x,y] != "" "") fail;
  map[x,y] = ""*"";
}

function down() {
  map[x,y] = ""|"";
  y = y+1;
  if (y == size || map[x,y] != "" "") fail;
  map[x,y] = ""*"";
}

function done()
{
    return x == size-1 && y == size-1;
}

solve();

print(x,"" "",y);
";
    }
}