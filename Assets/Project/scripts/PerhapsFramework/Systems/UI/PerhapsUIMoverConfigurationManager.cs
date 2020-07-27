using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{

    [System.Serializable]
    public struct UIMoverConfiguration
    {
        public string configName;
        public UIMoverPerhaps[] movers;
        public UIMoverTransition[] states;
    }

    public class PerhapsUIMoverConfigurationManager : MonoBehaviour
    {
        public UIMoverConfiguration[] uiMoverConfigurations;
        public UIMoverConfiguration mainMenuConfig => uiMoverConfigurations[0];
        public UIMoverConfiguration gameConfig => uiMoverConfigurations[1];

        List<UIMoverPerhaps> GetMoversRecursive()
        {
            List<UIMoverPerhaps> movers = new List<UIMoverPerhaps>();
            GetMover(transform, movers);

            return movers;
        }

        void GetMover(Transform t, List<UIMoverPerhaps> movers)
        {
            bool HasConfigManager = t.GetComponent<PerhapsUIMoverConfigurationManager>() != null;

            //we do not add movers that belong to other config managers.
            if (HasConfigManager && t != transform)
            {
                return;
            }

            UIMoverPerhaps mover = t.GetComponent<UIMoverPerhaps>();
            if (mover != null)
            {
                movers.Add(mover);
            }

            for (int i = 0; i < t.transform.childCount; i++)
            {
                GetMover(t.GetChild(i), movers);
            }
        }

        private void OnValidate()
        {
            if (uiMoverConfigurations == null)
                return;

            for (int i = 0; i < uiMoverConfigurations.Length; i++)
            {
                if (uiMoverConfigurations[i].movers == null || uiMoverConfigurations[i].movers.Length == 0)
                {
                    List<UIMoverPerhaps> movers = GetMoversRecursive();

                    for (int j = 0; j < movers.Count; j++)
                    {
                        if (movers[j].ignoreLayoutConfig)
                        {
                            //this mover is some special window that doesnt need to be -
                            //affected by the UIMoverConfiguration setup

                            movers.RemoveAt(j);
                            j--;
                        }
                    }

                    uiMoverConfigurations[i].movers = movers.ToArray();
                    uiMoverConfigurations[i].states = new UIMoverTransition[movers.Count];

                    var moverList = uiMoverConfigurations[i].movers;
                    var statesList = uiMoverConfigurations[i].states;
                    for (int j = 0; j < moverList.Length; j++)
                    {
                        statesList[j] = moverList[j].currentTransition;
                    }
                }
            }
        }

        /// <summary>
        /// Usefull for clearing the ui... or for making a complete clusterfuck
        /// </summary>
        /// <param name="command"></param>
        public void ArrangeAllUI(UIMoverTransition command)
        {
            for (int i = 0; i < uiMoverConfigurations.Length; i++)
            {
                UIMoverConfiguration config = uiMoverConfigurations[i];
                foreach (var item in config.movers)
                {
                    item.Transition(command);
                }
            }
        }

        public void ArrangeUIConfiguration(UIMoverConfiguration config, bool instant = false)
        {
            
            //TransitionCommand cmd = new TransitionCommand();
            for (int i = 0; i < config.movers.Length; i++)
            {
                UIMoverPerhaps mover = config.movers[i];

                if (mover != null)
                {
                    mover.Transition(config.states[i], instant);
                    /*
                    cmd.destinationState = config.states[i];
                    cmd.instant = instant;
                    cmd.time = mover.GetDefaultTransitionTime();
                    */

                    //mover.Transition(cmd);
                }
            }
        }
    }

}