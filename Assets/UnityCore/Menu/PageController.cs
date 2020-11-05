using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityCore {
    namespace Menu {
        
        public class PageController : MonoBehaviour
        {

            public static PageController instance;

            public bool debug = true;
            public PageType entryPage;
            public Page[] pages;

            private Hashtable m_Pages;

            public PageType onPage;

#region Unity Functions
            private void Awake() {
                if(!instance) {
                    instance = this;
                    m_Pages = new Hashtable();
                    RegisterAllPages();
                    
                    if(entryPage != PageType.None) {
                        TurnPageOn(entryPage);
                    }
                }
            }
#endregion

#region Public Functions
            public void TurnPageOn(PageType type) {
                if(type == PageType.None) return;
                if(!PageExists(type)) {
                    LogWarning("You are trying to turn a page on ("+type+") that has not been registered.");
                    return;
                }

                Page page = GetPage(type);
                page.gameObject.SetActive(true);
                page.Animate(true);
                onPage = type;

            }
            public void TurnPageOff(PageType off, PageType on=PageType.None, bool waitForExit=false) {
                if(off == PageType.None) return;
                if(!PageExists(off)) {
                    LogWarning("You are trying to turn a page off ("+off+") that has not been registered.");
                    return;
                }

                Page offPage = GetPage(off);
                if(offPage.gameObject.activeSelf) {
                    offPage.Animate(false);
                }
                
                if(on != PageType.None) {
                    Page onPage = GetPage(on);
                    if(waitForExit) {
                        
                        StopCoroutine("WaitForPageExit");
                        StartCoroutine(WaitForPageExit(onPage, offPage));
                    } else { 
                        TurnPageOn(on);
                    }
                }
                onPage = on;
                
            }
#endregion

#region Private Functions
            private IEnumerator WaitForPageExit(Page on, Page off) {
                while(off.targetState != Page.FLAG_NONE) {
                    yield return null;
                }
                TurnPageOn(on.type);
            }
            private void RegisterAllPages() {
                foreach(Page page in pages) {
                    RegisterPage(page);
                }
            }
            private void RegisterPage(Page page) {
                if(PageExists(page.type)) {
                    LogWarning("You are trying to register a page ("+ page.type+") that has already been registered: "+page.gameObject.name);
                    return;
                }
                
                m_Pages.Add(page.type, page);
                Log("Registered new page ("+page.type+")");
            }

            private bool PageExists(PageType type) {
                return m_Pages.ContainsKey(type);
            }
            private Page GetPage(PageType type) {
                if(!PageExists(type)) {
                    LogWarning("You are trying to get a page ("+type+") that has not been registered");
                    return null;
                }
                return (Page)m_Pages[type];
            }

            private void Log(string msg) {
                if(!debug) return;
                Debug.Log("[PageController]: " + msg);
            }

            private void LogWarning(string msg) {
                if(!debug) return;
                Debug.LogWarning("[PageController]: " + msg);
            }
#endregion
        }
    }
}
