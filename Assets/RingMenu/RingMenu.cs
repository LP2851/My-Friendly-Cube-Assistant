using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingMenu : MonoBehaviour
{
    public Ring data;
    public RingCakePiece ringCakePiecePrefab;
    public float gapWidthDegree = 1f;
    public Action<string> callback;
    protected RingCakePiece[] pieces;
    public RingMenu parent;
    public string path;

    void Start()
    {
        var stepLength = 360f / data.elements.Length;
        var iconDist = Vector3.Distance(ringCakePiecePrefab.icon.transform.position, ringCakePiecePrefab.CakePiece.transform.position) / 3;

        //Position it
        pieces = new RingCakePiece[data.elements.Length];

        for (int i = 0; i < data.elements.Length; i++)
        {
            pieces[i] = Instantiate(ringCakePiecePrefab, transform);
            //set root element
            pieces[i].transform.localPosition = Vector3.zero;
            pieces[i].transform.localRotation = Quaternion.identity;

            //set cake piece
            pieces[i].CakePiece.fillAmount = 1f / data.elements.Length -  gapWidthDegree / 360f;
            pieces[i].CakePiece.transform.localPosition = Vector3.zero;
            pieces[i].CakePiece.transform.localRotation = Quaternion.Euler(0, 0, -stepLength / 2f +  gapWidthDegree/ 2f + i * stepLength);
            pieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.5f);

            //set icon
            //pieces[i].icon.transform.localPosition = pieces[i].CakePiece.transform.localPosition + Quaternion.AngleAxis(i * stepLength, Vector3.forward) * Vector3.up * iconDist;

            pieces[i].icon.transform.localPosition = pieces[i].CakePiece.transform.localPosition + Quaternion.Euler(0, 0, -stepLength / 2f +  gapWidthDegree/ 2f + i * stepLength + -stepLength /2f) * Vector3.up * iconDist;
            pieces[i].icon.sprite = data.elements[i].icon;

        }
    }

    private void Update()
    {
        var stepLength = 360f / data.elements.Length;
        var mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - transform.position, Vector3.forward) + stepLength / 2f);
        var activeElement = (int)(mouseAngle / stepLength);
        for (int i = 0; i < data.elements.Length; i++)
        {
            if(i == activeElement)
                pieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.75f);
            else
                pieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.5f);
        }


        if (Input.GetMouseButtonDown(0))
        {
            //var path = path + "/" + data.elements[activeElement].name;
            if (data.elements[activeElement].nextRing != null)
            {
                var newSubRing = Instantiate(gameObject, transform.parent).GetComponent<RingMenu>();
                newSubRing.parent = this;
                for (var j = 0; j < newSubRing.transform.childCount; j++)
                    Destroy(newSubRing.transform.GetChild(j).gameObject);
                //newSubRing.data = data.elements[activeElement].nextRing;
                //newSubRing.path = path;
                //newSubRing.callback = callback;
            }
            else
            {
                //callback?.Invoke(path);
            }
            gameObject.SetActive(false);
        }
    }

    private float NormalizeAngle(float a) => (a + 360f) % 360f;
}