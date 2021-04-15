﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice : MonoBehaviour
{
    public string question; //질문
    public string[] answers; //답변 (최대 1~4 허용)
}
