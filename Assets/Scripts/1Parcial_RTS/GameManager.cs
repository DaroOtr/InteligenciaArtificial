using Pathfinder.Grapf;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
        
        public void InitGame()
        {
            int _width = 0;
            int _height = 0;
            int _separation = 0;
            int _mineCount = 0;
            int _minerCount = 0;
            int _caravanCount = 0;

            if (int.Parse(grapfWidth.text) <= 0 || grapfWidth.text == null)
                _width = 1;
            else
                _width = int.Parse(grapfWidth.text);
            
            if (int.Parse(grapfHeight.text) <= 0 || grapfHeight.text == null)
                _height = 1;
            else
                _height = int.Parse(grapfHeight.text);
            
            if (int.Parse(nodeSeparation.text) <= 0 || nodeSeparation.text == null)
                _separation = 1;
            else
                _separation = int.Parse(nodeSeparation.text);
            
            if (int.Parse(mineCount.text) <= 0 || mineCount.text == null)
                _mineCount = 1;
            else
                _mineCount = int.Parse(mineCount.text);
            
            if (int.Parse(minerCount.text) <= 0 || minerCount.text == null)
                _minerCount = 1;
            else
                _minerCount = int.Parse(minerCount.text);
            
            if (int.Parse(caravanCount.text) <= 0 || caravanCount.text == null)
                _caravanCount = 1;
            else
                _caravanCount = int.Parse(caravanCount.text);
            
            
            grapfView.SetGrapfCreationParams(_width,_height,_separation,_mineCount);
            grapfView.InitGrapf();
        }
    }
}
