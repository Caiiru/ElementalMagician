using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    // Define the direction of the connection point
    public enum Direction
    {
        Up,
        Down,
        LeftUp,
        LeftDown,
        RightUp,

        RightDown
    }

    // Direction of this connection point
    public Direction direction;

    // Reference to the connected room (if any)
    public GameObject connectedRoom;

    // Method to connect this point to another room
    public void ConnectToRoom(GameObject room)
    {
        connectedRoom = room;
    }

    // Method to check if this point is connected to another room
    public bool IsConnected()
    {
        return connectedRoom != null;
    }
}
