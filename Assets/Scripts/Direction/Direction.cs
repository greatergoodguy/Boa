using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionEnum {
    None = 0,
    Up = 1,
    Right = 2,
    Down = 3,
    Left = 4,
}

public struct Direction {
    public readonly DirectionEnum direction;

    public Direction(DirectionEnum direction) {
        this.direction = direction;
    }

    public Quaternion GetHeadRotation() => FOO.directions[direction].GetHeadRotation();

    public DG_Vector2 GetMoveVector() => FOO.directions[direction].GetMoveVector();

    public static DirectionEnum Random(int seed) {
        return (DirectionEnum)new System.Random(seed).Next(0, 4);
    }
}

static class FOO {
    public static readonly Directions directions = new Directions() { { DirectionEnum.Up, new Up() }, { DirectionEnum.Right, new Right() }, { DirectionEnum.Down, new Down() }, { DirectionEnum.Left, new Left() } };
}

class Directions : Dictionary<DirectionEnum, IDirection> { }

interface IDirection {
    DG_Vector2 GetMoveVector();
    Quaternion GetHeadRotation();
}

struct Up : IDirection {
    public Quaternion GetHeadRotation() => Quaternion.Euler(0, 0, 0);

    public DG_Vector2 GetMoveVector() => new DG_Vector2(0, 1);
}

struct Right : IDirection {
    public Quaternion GetHeadRotation() => Quaternion.Euler(0, 90, 0);

    public DG_Vector2 GetMoveVector() => new DG_Vector2(1, 0);
}

struct Down : IDirection {
    public Quaternion GetHeadRotation() => Quaternion.Euler(0, 180, 0);

    public DG_Vector2 GetMoveVector() => new DG_Vector2(0, -1);
}

struct Left : IDirection {
    public Quaternion GetHeadRotation() => Quaternion.Euler(0, 270, 0);

    public DG_Vector2 GetMoveVector() => new DG_Vector2(-1, 0);
}
