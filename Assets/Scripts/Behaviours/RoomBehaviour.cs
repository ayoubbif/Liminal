using UnityEngine;

namespace Behaviours
{
    public class RoomBehaviour : MonoBehaviour
    {
        public GameObject[] doors;
        public GameObject[] walls; // 0 = north, 1 = south, 2 = east, 3 = west

        public void UpdateRoom(bool[] status)
        {
            for (var i = 0; i < status.Length; i++)
            {
                doors[i].SetActive(status[i]);
                walls[i].SetActive(!status[i]);
            }
        }
    }
}

