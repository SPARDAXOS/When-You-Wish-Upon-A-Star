using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class InfoOverlay : MonoBehaviour
{
    private bool OverlayState = false;
    private bool LegendsState = false;

    private float PopUpTextDuration = 2.0f;
    private float PopUpTextTimer = 0.0f;

    GameObject PopUpTextObject = null;
    Text PopUpTextScript = null;

    GameObject OverlayToggleInstruction = null;
    GameObject LegendsToggleInstruction = null;

    GameObject InstructionV = null;
    GameObject InstructionP = null;
    GameObject InstructionS = null;
    GameObject InstructionT = null;
    GameObject InstructionF = null;
    GameObject InstructionLMB = null;

    GameObject LegendNormal = null;
    GameObject LegendBlocked = null;
    GameObject LegendEvaluated = null;
    GameObject LegendExamined = null;
    GameObject LegendStart = null;
    GameObject LegendEnd = null;
    GameObject LegendPath = null;

    private void SaveChildrenReferences()
    {
        PopUpTextObject = transform.Find("PopUpText").gameObject;
        PopUpTextScript = PopUpTextObject.GetComponent<Text>();

        OverlayToggleInstruction = transform.Find("OverlayToggleInstruction").gameObject;
        LegendsToggleInstruction = transform.Find("LegendsToggleInstruction").gameObject;

        InstructionV = transform.Find("InstructionV").gameObject;
        InstructionP = transform.Find("InstructionP").gameObject;
        InstructionS = transform.Find("InstructionS").gameObject;
        InstructionT = transform.Find("InstructionT").gameObject;
        InstructionF = transform.Find("InstructionF").gameObject;
        InstructionLMB = transform.Find("InstructionLMB").gameObject;

        LegendNormal = transform.Find("LegendNormal").gameObject;
        LegendBlocked = transform.Find("LegendBlocked").gameObject;
        LegendEvaluated = transform.Find("LegendEvaluated").gameObject;
        LegendExamined = transform.Find("LegendExamined").gameObject;
        LegendStart = transform.Find("LegendStart").gameObject;
        LegendEnd = transform.Find("LegendEnd").gameObject;
        LegendPath = transform.Find("LegendPath").gameObject;

        SetOverlayElementsState(false);
        SetLegendsElementsState(false);
        PopUpTextObject.SetActive(false);
    }
    void Start()
    {
        SaveChildrenReferences();
    }
    void Update()
    {
        if (PopUpTextTimer > 0.0f)
        {
            UpdatePopUpTextDuration();
        }
    }
    public void ToggleOverlay()
    {
        if (OverlayState)
        {
            SetOverlayElementsState(false);
            OverlayState = false;
        }
        else
        {
            OverlayState = true;
            SetOverlayElementsState(true);
        }
    }
    public void ToggleLegends()
    {
        if (LegendsState)
        {
            SetLegendsElementsState(false);
            LegendsState = false;
        }
        else
        {
            LegendsState = true;
            SetLegendsElementsState(true);
        }
    }
    private void SetOverlayElementsState(bool state)
    {
        if (state)
            OverlayToggleInstruction.SetActive(false);
        else
            OverlayToggleInstruction.SetActive(true);

        InstructionV.SetActive(state);
        InstructionP.SetActive(state);
        InstructionS.SetActive(state);
        InstructionT.SetActive(state);
        InstructionF.SetActive(state);
        InstructionLMB.SetActive(state);
    }
    private void SetLegendsElementsState(bool state)
    {
        if (state)
            LegendsToggleInstruction.SetActive(false);
        else
            LegendsToggleInstruction.SetActive(true);

        LegendNormal.SetActive(state);
        LegendBlocked.SetActive(state);
        LegendEvaluated.SetActive(state);
        LegendExamined.SetActive(state);
        LegendStart.SetActive(state);
        LegendEnd.SetActive(state);
        LegendPath.SetActive(state);
    }

    public void TogglePopUpText(string text)
    {
        PopUpTextTimer = PopUpTextDuration;
        if (!PopUpTextObject.activeInHierarchy)
            PopUpTextObject.SetActive(true);
        PopUpTextScript.text = text;
    }
    private void UpdatePopUpTextDuration()
    {
        PopUpTextTimer -= Time.deltaTime;
        if(PopUpTextTimer <= 0.0f)
        {
            PopUpTextTimer = 0.0f;
            PopUpTextObject.SetActive(false);
        }
    }
}      
       
       
       