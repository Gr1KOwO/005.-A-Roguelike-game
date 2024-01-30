using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomsGeneratorVariant : MonoBehaviour
{
    [SerializeField]
    private RoomVariant startRoomPrefab;

    [SerializeField]
    private RoomVariant bossRoomPrefab;

    [SerializeField]
    private RoomVariant treasureRoomPrefab;


    [SerializeField]
    private RoomVariant[] roomPrefab;

    private IEnumerator Generate()
    {
        GraphVariant graph = new GraphVariant();
        var infos = graph.Generate1(Random.Range(8, 10));

        // ��������� ������� ������
        var startRoomPos = infos.Keys.First();
        var startRoom = Instantiate(startRoomPrefab, new Vector3(startRoomPos.x, 0, startRoomPos.y) * 11f, Quaternion.identity);
        startRoom.Setup(infos[startRoomPos]);

        yield return 0;

        // ���� ��� ������������, ���� �� ��� ������� ������� � �����������
        bool treasureRoomSpawned = false;

        // ��������� ������ �������� ����
        foreach (var pos in infos.Keys.Skip(1).SkipLast(1))
        {
            if (Random.Range(0, 100) < 10 && !treasureRoomSpawned)
            {
                // ��������� ������� � ����������� � �������� ������� ������
                var treasureRoomPos = pos;
                var treasureRoom = Instantiate(treasureRoomPrefab, new Vector3(treasureRoomPos.x, 0, treasureRoomPos.y) * 11f, Quaternion.identity);
                treasureRoom.Setup(infos[treasureRoomPos]);

                yield return 0;

                // ��������� ���� � true ����� ����, ��� ������� � ����������� ���� �������
                treasureRoomSpawned = true;
            }
            else
            {
                var room = Instantiate(roomPrefab[Random.Range(0, roomPrefab.Length)], new Vector3(pos.x, 0, pos.y) * 11f, Quaternion.identity);
                room.Setup(infos[pos]);

                yield return 0;
            }
        }

        // ��������� ������� �����
        var bossRoomPos = infos.Keys.Last();
        var bossRoom = Instantiate(bossRoomPrefab, new Vector3(bossRoomPos.x, 0, bossRoomPos.y) * 11f, Quaternion.identity);
        bossRoom.Setup(infos[bossRoomPos]);

        yield return 0;
    }

    private void Start()
    {
        StartCoroutine(Generate());
    }
}
