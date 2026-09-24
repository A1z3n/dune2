using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dune2;
using UnityEngine;

namespace Dune2
{
    public class radar : building {

        public override void Init(int x, int y, int pPlayer, float pHealthPart)
        {
            rect.width = 2;
            rect.height = 2;
            health = 1000;
            type = eBuildingType.kRadar;
            base.Init(x, y, pPlayer,pHealthPart);
        }
        // Start is called before the first frame update

        void Start()
        {
            base.Start();
            gameManager.GetInstance().ActivateRadar();
        }

        // Update is called once per frame
        void Update()
        {
            base.Update();
        }

        protected override void Activated() {
            //throw new NotImplementedException();
        }

        protected override void OnDestroy()
        {
            gameManager.GetInstance().DeactivateRadar();
        }

        public override void Select() {
            gameManager.GetInstance().GetGui().ShowBuildIcon(eBuildingType.kRadar);
            base.Select();
        }
        public override void Unselect()
        {
            gameManager.GetInstance().GetGui().HideBuildIcon();
            base.Unselect();
        }
        public override eBuildingType GetBuildingType()
        {
            return type;
        }
    }
}
