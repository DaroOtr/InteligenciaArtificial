using Pathfinder.Grapf;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _1Parcial_RTS
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputField grapfWidth;
        [SerializeField] private InputField grapfHeight;
        [SerializeField] private InputField nodeSeparation;
        [SerializeField] private GrapfView _grapfView;
        
        public void InitGame()
        {
            int width = 0;
            int height = 0;
            int separation = 0;

            if (int.Parse(grapfWidth.text) <= 0 || grapfWidth.text == null)
                width = 1;
            else
                width = int.Parse(grapfWidth.text);
            
            if (int.Parse(grapfHeight.text) <= 0 || grapfHeight.text == null)
                height = 1;
            else
                height = int.Parse(grapfHeight.text);
            
            if (int.Parse(nodeSeparation.text) <= 0 || nodeSeparation.text == null)
                separation = 1;
            else
                separation = int.Parse(nodeSeparation.text);
            
            _grapfView.SetGrapfCreationParams(width,height,separation);
            _grapfView.InitGrapf();
        }
    }
}
