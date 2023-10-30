using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dune2;
using UnityEngine;

namespace Dune2
{
    class windtrap : building {
        private progressBar healthBar;
        private progressBar energyBar;

        ~windtrap() {
            gameManager.GetInstance().RemoveWindtrap();
        }
        public override void Init(int x, int y, int pPlayer, float pHealthPart)
        {
            rect.width = 2;
            rect.height = 2;
            health = 300;
            type = eBuildingType.kWindTrap;
            base.Init(x, y, pPlayer,pHealthPart);
        }
        // Start is called before the first frame update
        void Start()
        {
            base.Start();
            gameManager.GetInstance().AddEnergy(100);
            gameManager.GetInstance().AddWindtrap();
        }

        // Update is called once per frame
        void Update() {
            base.Update();
        }

        protected override void Activated() {
            //throw new NotImplementedException();
        }


        public override void Select() {
            gameManager.GetInstance().GetGui().ShowBuildIcon(eBuildingType.kWindTrap);
            healthBar = GameObject.Find("GUI/Icons/windtrap/health").GetComponent<progressBar>();
            healthBar.SetProgress((float)health/(float)fullHealth);
            energyBar = GameObject.Find("GUI/Icons/windtrap/energy").GetComponent<progressBar>();
            int e = gameManager.GetInstance().GetEnergy();
            int w = gameManager.GetInstance().GetWindtrapCount();
            float p = e/100.0f*w;
            energyBar.SetProgress(p);

            base.Select();
        }
        public override void Unselect()
        {
            gameManager.GetInstance().GetGui().HideBuildIcon();
            base.Unselect();
        }
    }
}
