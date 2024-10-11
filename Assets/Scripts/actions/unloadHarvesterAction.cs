namespace Dune2 {
    public class unloadHarvesterAction : action {
        private harvester mHarvester = null;
        private const int UNLOAD_SPEED = 50;
        private refinery build;
        private float ostatok = 0.0f;
        public void Init(refinery r) {
            build = r;
        }
        public override bool Update(actionBase u, float dt) {
            if (mHarvester == null) {
                mHarvester = (u as harvester);
                if (mHarvester == null) {
                    return false;
                }
                if(build!=null) build.TurnOffLights();
            }
            
            int spices = mHarvester.GetSpiceCount();
            if (spices > 0) {
                float d = UNLOAD_SPEED * dt + ostatok;
                int delta = (int)d;
                if (delta == 0) {
                    ostatok += d;
                    return true;
                }
                ostatok = 0.0f;
                mHarvester.AddSpice(-delta);
                gameManager.GetInstance().AddCredits(delta);
            }
            else {
                actionManager.Harvest(mHarvester);
                return false;
            }
            return true;
        }
    }
}