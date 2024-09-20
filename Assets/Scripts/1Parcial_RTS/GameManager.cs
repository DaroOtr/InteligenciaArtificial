using System;
using System.Collections.Generic;
using _1Parcial_RTS.RTS_Entities.MIner;
using Pathfinder.Grapf;
using Pathfinder.Node;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _1Parcial_RTS
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputField grapfWidth;
        [SerializeField] private InputField grapfHeight;
        [SerializeField] private InputField nodeSeparation;
        [SerializeField] private InputField mineCount;
        [SerializeField] private InputField minerCount;
        [SerializeField] private InputField caravanCount;
        [SerializeField] private GrapfView grapfView;
        [SerializeField] private List<Miner> _miners;
        private Dictionary<Node<Vector2Int>, int> _mines = new Dictionary<Node<Vector2Int>, int>();
        private (Node<Vector2Int> urbanCenterNode,int urbanCenterGold) _urbanCenter = new ValueTuple<Node<Vector2Int>, int>();
        private int _width = 0;
        private int _height = 0;
        private int _separation = 0;
        private int _mineCount = 0;
        private int _minerCount = 0;
        private int _caravanCount = 0;
        private bool _isGameInitialized = false;

        public void InitGame()
        {
            SetIngameParameters();
            InitGrapf();
            InitMines();
            InitMiners();
            _isGameInitialized = true;
        }

        private void InitMiners()
        {
            Func<int, int> mineFuc = MinegoldFromMine;
            Func<int, int> getGoldFunc = GetGoldFromMine;
            Action<int> depositAct = DepositGold;
            Func<Vector3,Node<Vector2Int>> recalculatePathFunc = GetClosestMine;
            foreach (Miner miner in _miners)
            {
                miner.InitMiner(grapfView.Grapf, grapfView._nodeSeparation, mineFuc,depositAct,getGoldFunc);
            }
        }

        private void InitGrapf()
        {
            grapfView.SetGrapfCreationParams(_width, _height, _separation, _mineCount);
            grapfView.InitGrapf();
            _urbanCenter.urbanCenterNode = grapfView.Grapf.GetNode(RtsNodeType.UrbanCenter);
            _urbanCenter.urbanCenterGold = 0;
        }

        private void InitMines()
        {
            ICollection<Node<Vector2Int>> ingameMines = grapfView.Grapf.GetNodesOfType(RtsNodeType.Mine);
            foreach (Node<Vector2Int> mine in ingameMines)
            {
                //_mines.Add(mine, Random.Range(0, 30));
                _mines.Add(mine,5);
                if (_mines[mine] == 0)
                    mine.SetBlock(true);
            }
        }


        private void Update()
        {
            if (_isGameInitialized)
                CheckForEmptyMines();
        }

        private void CheckForEmptyMines()
        {
            ICollection<Node<Vector2Int>> ingameMines = grapfView.Grapf.GetNodesOfType(RtsNodeType.Mine);
            foreach (Node<Vector2Int> mine in ingameMines)
            {
                if (_mines[mine] == 0)
                    mine.SetBlock(true);
            }
        }

        private Node<Vector2Int> GetClosestMine(Vector3 minerPos)
        {
            float distance = float.MaxValue;
            Node<Vector2Int> closestMine = new Node<Vector2Int>();
            foreach (KeyValuePair<Node<Vector2Int>,int> mine in _mines)
            {
                Vector3 minePos = new Vector3(mine.Key.GetCoordinate().x,mine.Key.GetCoordinate().y);
                if (Vector3.Distance(minerPos, minePos) < distance)
                {
                    distance = Vector3.Distance(minerPos, minePos);
                    closestMine = mine.Key;
                }
            }

            return closestMine;
        }

        private int MinegoldFromMine(int mineIndex)
        {
            Node<Vector2Int> mine = grapfView.Grapf.GetNode(mineIndex);
            if (_mines.ContainsKey(mine))
            {
                if (_mines[mine] > 0)
                {
                    _mines[mine]--;
                    Debug.Log("Mine Index : " + mineIndex);
                    Debug.Log("Current Mine Gold : " + _mines[mine]);
                    return 1;
                }
                else
                {
                    Debug.Log("Mine Index : " + mineIndex + " NO MORE GOLD");
                    return 0;
                }
                
            }

            Debug.Log("Error searching for the specified mine");
            return -1;
        }

        private int GetGoldFromMine(int mineIndex)
        {
            Node<Vector2Int> mine = grapfView.Grapf.GetNode(mineIndex);
            if (_mines.ContainsKey(mine))
            {
                return _mines[mine];
            }


            Debug.Log("Error searching for the specified mine");
            return -1;
        }

        private void DepositGold(int value)
        {
            _urbanCenter.urbanCenterGold += value;
        }

        private void SetIngameParameters()
        {
            if (grapfWidth.text == "" || int.Parse(grapfWidth.text) <= 0)
                _width = 11;
            else
                _width = int.Parse(grapfWidth.text);

            if (grapfHeight.text == "" || int.Parse(grapfHeight.text) <= 0)
                _height = 11;
            else
                _height = int.Parse(grapfHeight.text);

            if (nodeSeparation.text == "" || int.Parse(nodeSeparation.text) <= 0)
                _separation = 1;
            else
                _separation = int.Parse(nodeSeparation.text);

            if (mineCount.text == "" || int.Parse(mineCount.text) <= 0)
                _mineCount = 5;
            else
                _mineCount = int.Parse(mineCount.text);

            if (minerCount.text == "" || int.Parse(minerCount.text) <= 0)
                _minerCount = 1;
            else
                _minerCount = int.Parse(minerCount.text);

            if (caravanCount.text == "" || int.Parse(caravanCount.text) <= 0)
                _caravanCount = 1;
            else
                _caravanCount = int.Parse(caravanCount.text);
        }
    }
}