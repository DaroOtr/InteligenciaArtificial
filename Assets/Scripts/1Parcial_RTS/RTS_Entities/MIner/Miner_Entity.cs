using System.Collections;
using System.Collections.Generic;
using Pathfinder.Algorithm;
using Pathfinder.Grapf;
using Pathfinder.Node;
using UnityEngine;

namespace _1Parcial_RTS.RTS_Entities.MIner
{
    public enum MinerBehaviours { Walk,Mine,Retreat,Eat }
    public enum MinerFlags { OnMineReach, OnMove , OnRetreat }
    public class Miner : MonoBehaviour
    {
        private AStarPathfinder<Node<Vector2Int>, Vector2Int> _pathfinder = new AStarPathfinder<Node<Vector2Int>, Vector2Int>() ;
        List<Node<Vector2Int>> _path = new List<Node<Vector2Int>>();
        public GrapfView grapfView;
        private Node<Vector2Int> _startNode;
        private Node<Vector2Int> _destinationNode;
        private FSM<MinerBehaviours, MinerFlags> _minerFsm = new FSM<MinerBehaviours, MinerFlags>();
        private uint _minerID;

        public void InitMiner()
        {
            SetStarterNode(grapfView.Grapf.GetNode(RtsNodeType.UrbanCenter));
            SetInitialPos();
            FindDestination();
            CalculatePath();
            //StartCoroutine(Move(path));
        }

        private void SetInitialPos()
        {
            Vector3 newPos = new Vector3(_startNode.GetCoordinate().x * grapfView._nodeSeparation,
                _startNode.GetCoordinate().y * grapfView._nodeSeparation);                        
            transform.position = newPos;                                                          
        }

        private void CalculatePath()
        {
            _path = _pathfinder.FindPath(_startNode, _destinationNode, grapfView.Grapf);
        }

        private void SetStarterNode(Node<Vector2Int> newStarterNode)
        {
            _startNode = newStarterNode;
        }

        private void SetDestinationNode(Node<Vector2Int> newDestinationNode)
        {
            _destinationNode = newDestinationNode;
        }

        private void FindDestination()
        {
            _destinationNode = grapfView.Grapf.GetNode(RtsNodeType.Mine);
        }

        public IEnumerator Move(List<Node<Vector2Int>> path)
        {
            //Vector3 lastPos = transform.position;
            foreach (Node<Vector2Int> node in path)
            {
                Vector3 newPos = new Vector3(node.GetCoordinate().x * grapfView._nodeSeparation,
                    node.GetCoordinate().y * grapfView._nodeSeparation);
                
                transform.position = newPos;
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}
