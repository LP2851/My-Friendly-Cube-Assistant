using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityCore {
    namespace Menu {
        
        public class Page : MonoBehaviour
        {
            public static readonly string FLAG_ON = "On";
            public static readonly string FLAG_OFF = "Off";
            public static readonly string FLAG_NONE = "None";

            public bool debug;

            public PageType type;
            public bool useAnimation;
            public string targetState {get;private set;}

            private Animator m_Animator;

#region Unity Functions
            private void OnEnable() {
                CheckAnimatorIntegrity();
            }
#endregion

#region Public Functions
            public void Animate(bool on) {
                if(useAnimation) {
                    m_Animator.SetBool("on", on);

                    StopCoroutine("AwaitAnimation");
                    StartCoroutine("AwaitAnimation", on);
                } else {
                    if(!on) {
                        gameObject.SetActive(false);
                    }
                }
            }

#endregion

#region Private Functions
            private IEnumerator AwaitAnimation(bool on) {
                targetState = on ? FLAG_ON : FLAG_OFF;

                // wait for the animator to reach the target state
                while(!m_Animator.GetCurrentAnimatorStateInfo(0).IsName(targetState)) {
                    yield return null;
                }
                // wait for the animator to finish animating
                while(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) {
                    yield return null;
                }

                targetState = FLAG_NONE;

                Log("Page ("+type+") finished transitioning to "+ (on ? "on" : "off"));


                if (!on) {
                    gameObject.SetActive(false);
                }
            }   

            private void CheckAnimatorIntegrity() {
                if (useAnimation) {
                    m_Animator = GetComponent<Animator>();
                    if(!m_Animator) {
                        LogWarning("You opted to animate a page ("+type+") but no animator component exists on the object");
                    }
                }
            }

            private void Log(string msg) {
                if(!debug) return;
                Debug.Log("[Page]: " + msg);
            }

            private void LogWarning(string msg) {
                if(!debug) return;
                Debug.LogWarning("[Page]: " + msg);
            }
#endregion
        }
    }
}
