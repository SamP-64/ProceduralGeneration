using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2 position;
    public Vector2 size;

    public Room(Vector2 _position, Vector2 _size) // Room Construcyot
    {
        position = _position;
        size = _size;
    }
}
