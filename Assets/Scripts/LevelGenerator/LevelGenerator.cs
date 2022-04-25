using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public class Cell
    {
        public int roomId;
        public Vector2Int id;
        public Vector3 position;
        public bool open;


        public Cell(Vector2Int id, Vector3 position)
        {
            this.id = id;
            this.position = position;
            open = false;
        }
    }
    public enum RoomDoorEnum
    {
        close = 0,
        right = 1,
        up = 2,
        rightUp = 3,
        left = 4,
        rightLeft = 5,
        upLeft = 6,
        rightUpLeft = 7,
        down = 8,
        rightDown = 9,
        upDown = 10,
        rightUpDown = 11,
        leftDown = 12,
        rightLeftDown = 13,
        upLeftDown = 14,
        rightUpLeftDown = 15,

    }
    [SerializeField]
    protected Vector2Int gridSize = Vector2Int.one * 5;
    [SerializeField]
    protected RoomActor roomPrefab;
    [SerializeField]
    protected StartRoomActor startRoomPrefab, endRoomPrefab;
    [SerializeField]
    protected DoorActor doorPrefab;//, startDoorPrefab, endDoorPrefab;
    [SerializeField]
    protected ItemWeapon[] weapons;
    protected Cell[] cells;
    protected List<RoomActor> rooms = new List<RoomActor>();
    protected List<DoorActor> doors = new List<DoorActor>();
    protected bool generated;
    protected const int RIGHT = 0, UP = 1, LEFT = 2, DOWN = 3;
    [SerializeField]
    protected SpawnListScriptableObject spawnList;
    [SerializeField]
    protected NavMeshSurface surface;
    [SerializeField]
    protected float timeToCompleteLevel = 180.0f;
    public string nextLevel;

    public RoomActor GetStart() => rooms[rooms.Count - 2];
    public RoomActor GetRoom(int index) => rooms[index];
    public float TimeToCompleteLevel => timeToCompleteLevel;

    [ContextMenu("Generate")]
    public void Generate()
    {
        int length = gridSize.x * gridSize.y;
        cells = new Cell[length];
        Vector3 origin = transform.position;
        for (int i = 0; i < gridSize.x; i++)
            for (int j = 0; j < gridSize.y; j++)
                cells[i + j * gridSize.x] = new Cell(new Vector2Int(i, j), origin + new Vector3(i, 0, j) * roomPrefab.Size);

        Vector2Int current = Vector2Int.zero;
        current.x = gridSize.x / 2;
        cells[current.x].open = true;
        while (!generated)
            Move(ref current, Random.Range(0, 4));
        for (int i = 0; i < length; i++)
        {
            if (cells[i].open)
            {
                RoomActor room = Instantiate(roomPrefab, transform);
                room.SetPosition(cells[i].position);
                room.Id = cells[i].id;              
                //room.OpenDoors(OpenRoom(cells[i].id));
                room.SetSpawnList(spawnList);
                rooms.Add(room);
                cells[i].roomId = rooms.Count - 1;
            }
        }

        RoomActor startRoom;
        RoomDoorEnum open = OpenRoom(rooms[0].Id);
        open = open | RoomDoorEnum.down;
        rooms[0].OpenDoors(open);
        startRoom = Instantiate(startRoomPrefab, transform);
        startRoom.SetPosition(rooms[0].transform.position + Vector3.back * (roomPrefab.Size + startRoomPrefab.Size) * 0.5f);
        

        RoomActor endRoom;
        open = OpenRoom(rooms[rooms.Count - 1].Id);
        open = open | RoomDoorEnum.up;
        rooms[rooms.Count - 1].OpenDoors(open);
        endRoom = Instantiate(endRoomPrefab, transform);
        endRoom.SetRotation(new Quaternion(0.0f, 1.0f, 0.0f, 0.0f));
        endRoom.SetPosition(rooms[rooms.Count - 1].transform.position + Vector3.forward * (roomPrefab.Size+ endRoomPrefab.Size )* 0.5f);
        StartRoomActor sra = endRoom as StartRoomActor;
        sra.trigger.nextLevel = nextLevel;


        for (int i = 1; i < rooms.Count - 1; i++)
        {
            rooms[i].OpenDoors(OpenRoom(rooms[i].Id));
        }

        rooms.Add(startRoom);
        rooms.Add(endRoom);


        for (int i = 0; i < gridSize.x; i++)
            for (int j = 0; j < gridSize.y-1; j++)
            {
                Cell from = cells[i + j * gridSize.x], to = cells[i + (j + 1) * gridSize.x];
                if (from.open && to.open)
                {
                    DoorActor door = Instantiate(doorPrefab, transform);
                    door.SetPosition(cells[i + j * gridSize.x].position + Vector3.forward * roomPrefab.Size * 0.5f);
                    door.SetRotation(Quaternion.identity);
                    door.SetRooms(rooms[from.roomId], rooms[to.roomId]);
                }
            }
        for (int j = 0; j < gridSize.y; j++)
            for (int i = 0; i < gridSize.x - 1; i++)      
            {
                Cell from = cells[i + j * gridSize.x], to = cells[(i + 1) + j * gridSize.x];
                if (from.open && to.open)
                {
                    DoorActor door = Instantiate(doorPrefab, transform);
                    door.SetPosition(cells[i + j * gridSize.x].position + Vector3.right * roomPrefab.Size * 0.5f);
                    door.SetRotation(new Quaternion(0.0f, 0.7071068f, 0.0f, 0.7071068f));
                    door.SetRooms(rooms[from.roomId], rooms[to.roomId]);
                }
            }

        int randWeapon = 0;
        int randRoom = Random.Range(1, rooms.Count - 3);
        if (randRoom >= rooms.Count)
            randRoom = 0;
        if (weapons.Length > 1)
            randWeapon = Random.Range(0, weapons.Length);
        IPoolObject ipo = GameInstance.Instance.PoolManager.Pop(weapons[randWeapon]);
        ipo.SetParent(transform);
        ipo.SetPosition(rooms[randRoom].transform.position);
        surface.BuildNavMesh();
    }
    protected void Move(ref Vector2Int current, int direction)
    {
        switch (direction)
        {
            case RIGHT:
                current.x += 1;
                break;
            case UP:
                current.y += 1;
                break;
            case LEFT:
                current.x += -1;
                break;
            case DOWN:
                current.y += -1;
                break;
            default:
                break;
        }
        if (current.x >= gridSize.x)
        {
            current.x = gridSize.x - 1;
            Move(ref current, UP);
            return;
        }
        else if (current.x < 0)
        {
            current.x = 0;
            Move(ref current, UP);
            return;
        }
        if (current.y < 0)
        {
            current.y = 0;
            Move(ref current, UP);
            return;
        }
        else if (current.y >= gridSize.y)
        {
            current.y = gridSize.y - 1;
            generated = true;
            return;
        }
        Cell cell = cells[current.x + current.y * gridSize.x];
        if (cell.open)
        {
            Move(ref current, UP);
            return;
        }
        else
        {
            cell.open = true;
            return;
        }

    }

    protected RoomDoorEnum OpenRoom(Vector2Int current)
    {
        RoomDoorEnum door = RoomDoorEnum.close;
        int x, y;
        x = current.x + 1;
        if (x < gridSize.x && cells[x + current.y * gridSize.x].open)
            door = door | RoomDoorEnum.right;
        y = current.y + 1;
        if (y < gridSize.y && cells[current.x + y * gridSize.x].open)
            door = door | RoomDoorEnum.up;
        x = current.x - 1;
        if (x >= 0 && cells[x + current.y * gridSize.x].open)
            door = door | RoomDoorEnum.left;
        y = current.y - 1;
        if (y >= 0 && cells[current.x + y * gridSize.x].open)
            door = door | RoomDoorEnum.down;
        return door;
    }
    private void OnDrawGizmosSelected()
    {
        if (roomPrefab != null)
        {
            if (!Application.isPlaying || !generated)
            {
                Gizmos.color = Color.magenta;
                for (int i = 0; i < gridSize.x; i++)
                    for (int j = 0; j < gridSize.y; j++)
                    {
                        Gizmos.DrawWireCube(transform.position + new Vector3(i, 0, j) * roomPrefab.Size, Vector3.one * roomPrefab.Size);
                    }
            }
            else
            {
                for (int i = 0; i < gridSize.x * gridSize.y; i++)
                {
                    if (!cells[i].open)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireCube(cells[i].position, Vector3.one * roomPrefab.Size);
                    }
                }
                for (int i = 0; i < gridSize.x * gridSize.y; i++)
                {
                    if (cells[i].open)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(cells[i].position, Vector3.one * roomPrefab.Size);
                    }
                }
            }
        }
    }
}
