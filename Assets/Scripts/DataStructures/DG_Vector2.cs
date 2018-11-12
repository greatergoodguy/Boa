using System;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

#pragma warning disable CS0660,CS0661

[TypeConverter(typeof(DG_Vector2Converter))]
public struct DG_Vector2 {
    [JsonIgnore]
    public static DG_Vector2 zero = new DG_Vector2(0, 0);
    public static DG_Vector2 up = new DG_Vector2(0, 1);
    public static DG_Vector2 right = new DG_Vector2(1, 0);
    public static DG_Vector2 down = new DG_Vector2(0, -1);
    public static DG_Vector2 left = new DG_Vector2(-1, 0);
    public DG_Vector2 normalized {
        get {
            return FromUnityVector2(this.ToUnityVector2().normalized);
        }
    }

    public static DG_Vector2 FromUnityVector2(Vector2 vector) {
        return new DG_Vector2(vector);
    }

    public readonly int x;
    public readonly int y;

    public DG_Vector2(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public DG_Vector2(Vector2 vector2) {
        this.x = (int) vector2.x;
        this.y = (int) vector2.y;
    }

    public DG_Vector2(Vector3 vector3) {
        this.x = (int) vector3.x;
        this.y = (int) vector3.z;
    }

    public override string ToString() {
        return x.ToString() + "," + y.ToString();
    }

    public static DG_Vector2 operator +(DG_Vector2 a, DG_Vector2 b) {
        return new DG_Vector2(a.x + b.x, a.y + b.y);
    }

    public static DG_Vector2 operator -(DG_Vector2 a, DG_Vector2 b) {
        return new DG_Vector2(a.x - b.x, a.y - b.y);
    }

    public static bool operator ==(DG_Vector2 a, DG_Vector2 b) {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(DG_Vector2 a, DG_Vector2 b) {
        return a.x != b.x || a.y != b.y;
    }

    public Vector2 ToUnityVector2() => new Vector2(x, y);
}

public class DG_Vector2Converter : TypeConverter {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type source) {
        return source == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context,
        CultureInfo culture, object value) {

        var positionString = (string) value;

        var split = positionString.Split(',');

        return new DG_Vector2(int.Parse(split[0]), int.Parse(split[1]));
    }

    public override object ConvertTo(ITypeDescriptorContext context,
        CultureInfo culture,
        object value, Type destinationType) {

        var position = (DG_Vector2) value;

        return position.x.ToString() + "," + position.y.ToString();
    }
}
