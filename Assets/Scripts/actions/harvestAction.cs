
using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Dune2 {
    public class harvestAction : action {

        private harvester mHarvester = null;
        private int spices = 0;
        private int prevFrame = 0;
        private float spicesF = 0;
        private int MAX_SPICES = 700;
        private const int HARVEST_SPEED = 50;
        private int prevSpices = 0;
        private Vector2Int tilePos;
        private Vector2Int destPos;
        private float ostatok = 0.0f;


        public override bool Update(actionBase u, float dt)
        {
            if (mHarvester == null) {
                mHarvester = (u as harvester);
                if (mHarvester == null) {
                    return false;
                }

                MAX_SPICES = mHarvester.GetMaxSpices();
                tilePos = mHarvester.GetTilePos();

            }
            base.Update(u, dt);

            spices = mHarvester.GetSpiceCount();
            bool isHarvestable = gameManager.GetInstance().GetMapManager().IsSpiceAtPoint(tilePos) ;
            bool isFull = spices >= MAX_SPICES;
            mHarvester.ChangeHarvestAnimation(isHarvestable);
            if (isHarvestable && !isFull) {


                float d = HARVEST_SPEED * Time.deltaTime + ostatok;
                int delta = (int)d;
                if (delta == 0)
                {
                    ostatok += d;
                    return true;
                }
                ostatok = 0.0f;

                int need = MAX_SPICES - spices;
                if ( delta > need) {
                    delta = need;
                }
                float left = gameManager.GetInstance().GetMapManager()
                    .AddSpiceCountAt(tilePos.x, tilePos.y, -delta);

                mHarvester.AddSpice(delta);
                spices = mHarvester.GetSpiceCount();
                if (spices >= MAX_SPICES)
                {
                    spices = MAX_SPICES;
                    Finish();
                    return false;
                }
                /*
                int need = MAX_SPICES - spices;
                float d = (HARVEST_SPEED * Time.deltaTime);
                if (d > need) {
                    d = need;
                }

                spicesF += d;
                spices = (int)spicesF;
                if (spices != prevSpices) {
                    int delta = prevSpices - spices;
                    int left = gameManager.GetInstance().GetMapManager()
                        .AddSpiceCountAt(tilePos.x, tilePos.y, delta);
                    prevSpices = spices;
                    if (left > 0) {
                        mHarvester.AddSpice(-delta);
                    }
                    if (left < 0)
                        d += left;
                    if (left <= 0) {
                        if (spices < MAX_SPICES) {

                        }
                    }

                    if (spices >= MAX_SPICES) {
                        spices = MAX_SPICES;
                        spicesF = MAX_SPICES;
                        prevSpices = MAX_SPICES;
                        isHarvest = false;
                        var dst = gameManager.GetInstance().GetMapManager().GetBuildingsManager()
                            .GetNearestBuilding(eBuildingType.kRefinery, mHarvester.GetTilePos().x,
                                mHarvester.GetTilePos().y);
                        (dst as refinery).TurnOnLights();
                        actionManager.MoveToTarget(mHarvester, dst);
                        return false;
                    }
                }
                */
            }
            //else if(u.GetActions().Count==0) {
            else if (!isFull && !isHarvestable) {
                destPos = mHarvester.SearchSpice();
                actionManager.moveToPoint(mHarvester, destPos.x, destPos.y);
                return false;
            }

            return true;
        }

        public override eActionType GetActionType() {
            return eActionType.kHarvestAction;
        }

        private void Finish() {
            var dst = gameManager.GetInstance().GetMapManager().GetBuildingsManager()
                .GetNearestBuilding(eBuildingType.kRefinery, mHarvester.GetTilePos().x,
                    mHarvester.GetTilePos().y);
            (dst as refinery)?.TurnOnLights();
            actionManager.MoveToTarget(mHarvester, dst);
        }

    }
}