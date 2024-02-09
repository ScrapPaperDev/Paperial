using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Paperial;
using UnityEngine.TestTools;
using Scrappie;
using UnityEngine;
using System;

[TestFixture]
public class Tests
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestsSimplePasses()
    {
        // Arrange

        // Act

        // Assert
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }


    [Test]
    public void Math_UniqueIndexerFormula_ReturnsOrderedList()
    {
        // Arrange
        int size = 12;
        List<int> indicis = new List<int>();

        // Act
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    indicis.Add(GridUtils.CoordinatesToIndex(x, y, z, size));
                }
            }
        }

        indicis.Sort();

        //Assert
        for (int i = 0; i < indicis.Count; i++)
            Assert.AreEqual(i, indicis[i]);
    }

    [Test]
    public void Utils_LongFlags_ArbitraryIndiciesSet()
    {
        // Arrange
        long[] flags = new long[8];

        // Act
        flags.SetFlag(0);
        flags.SetFlag(221);
        flags.SetFlag(511);
        flags.SetFlag(149);

        // Assert
        Assert.IsTrue(flags.IsFlagged(0));
        Assert.IsTrue(flags.IsFlagged(221));
        Assert.IsTrue(flags.IsFlagged(511));
        Assert.IsTrue(flags.IsFlagged(149));
    }

    [Test]
    public void Utils_LongFlags_CorrectNumberGenerated()
    {
        // Arrange
        long[] flags = GridUtils.GenerateFlagsArray(GridUtils.GetGridFlagCount(12));
        int expectedSizeOfFlags = 12 * 12 * 12;
        int expectedSizeOfArray = 27;
        // Act


        // Assert
        Assert.AreEqual(expectedSizeOfFlags, flags.Length * 64);
        Assert.AreEqual(expectedSizeOfArray, flags.Length);
    }

    [Test]
    /// Asserts that when filled given 64 or more flags, each flag will be true
    public void Utils_LongFlags_FullFlagsUtilized()
    {
        // Arrange
        long[] flags = new long[2];
        long[] flags2 = new long[2];

        // Act
        for (int i = 0; i < 100; i++)
        {
            flags.SetFlag(i);
            //Debug.Log(i + ": " + Convert.ToString(flags[0], 2));
        }

        for (int i = 0; i < 64; i++)
            flags2.SetFlag(i);

        // Assert
        Assert.AreEqual(-1, flags[0]);

    }

    [Test]
    // Tests that the input names are mapped properly.
    // If an input is not found in the manager it wont detect any input(or throws), thus leaving the values as being viwed as pressed.
    // Because if input is found, it will set the values as not pushing any input.
    public void ValidateInputManager()
    {
        float LHori = 1.0f;
        LHori = Input.GetAxisRaw(PaperialInput.axis_LHori);
        Assert.AreEqual(0, LHori);

        float LVerti = 1.0f;
        LVerti = Input.GetAxisRaw(PaperialInput.axis_LVerti);
        Assert.AreEqual(0, LVerti);

        float RHori = 1.0f;
        RHori = Input.GetAxisRaw(PaperialInput.axis_RHori);
        Assert.AreEqual(0, RHori);

        float RVerti = 1.0f;
        RVerti = Input.GetAxisRaw(PaperialInput.axis_RVerti);
        Assert.AreEqual(0, RVerti);

        bool Select = true;
        Select = Input.GetButtonDown(PaperialInput.button_Select);
        Assert.AreEqual(false, Select);

        bool Cancel = true;
        Cancel = Input.GetButtonDown(PaperialInput.button_Cancel);
        Assert.AreEqual(false, Cancel);

        bool Loop = true;
        Loop = Input.GetButtonDown(PaperialInput.button_Loop);
        Assert.AreEqual(false, Loop);

        bool Pause = true;
        Pause = Input.GetButtonDown(PaperialInput.button_Pause);
        Assert.AreEqual(false, Pause);

        // Confirm that an input was not found
        bool wegi = true;
        Assert.Throws<ArgumentException>(() => wegi = Input.GetButtonDown("wegi"));

    }
}
