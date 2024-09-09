using Pathfinder.Grapf;
using UnityEngine;
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


            grapfView.SetGrapfCreationParams(_width, _height, _separation, _mineCount);
            grapfView.InitGrapf();
        }
    }
}