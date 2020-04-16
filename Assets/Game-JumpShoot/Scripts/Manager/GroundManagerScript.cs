﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManagerScript : MonoBehaviour
{
    [Header("Others")]
    public GameObject groundPrefab;

    public GameObject groundHolder;

    [Header("Difficulty Setting")]
    public int initOffSet = -4;

    public int distanceToNextGround = 8;
    public int initialGround = 5;
    public float groundWidth = 3;
    public float groundHeight = 0.6f;
    public float minVelocity = 1f;
    public float maxVelocity = 2f;
    public int difficultyGrowRate = 10;
    public float widthGrow = 0.05f;
    public float velocityGrow = 0.05f;
    public float widthGrowRateAdjust = 0.1f;
    public float velocityGrowRateAdjust = 0.1f;

    [Header("Debug")]
    [ReadOnly] public int difficultyLevel = 1;

    [ReadOnly] public int groundIndex = 0;

    private void Start()
    {
        InitGround();
    }

    private void InitGround()
    {
        for (int i = 0; i < initialGround; i++)
        {
            GenerateGround();
        }
    }

    public void GenerateGround()
    {
        DifficultyCalculation();
        Vector2 position = new Vector2(0, initOffSet + groundIndex * distanceToNextGround);
        GameObject newGroundObj = Instantiate(groundPrefab, position, Quaternion.identity);
        newGroundObj.transform.SetParent(groundHolder.transform);
        SetGroundAttribute(newGroundObj);
        groundIndex++;
    }

    public void GenerateGroundCheckPoint(Vector2 pos)
    {
        DifficultyCalculation();
        Vector2 position = pos;
        GameObject newGroundObj = Instantiate(groundPrefab, position, Quaternion.identity);
        newGroundObj.transform.SetParent(groundHolder.transform);
        SetGroundAttribute(newGroundObj);
        groundIndex++;
    }

    private void SetGroundAttribute(GameObject obj)
    {
        // Random Type
        GroundScript.GroundType groundType = (GroundScript.GroundType)Random.Range(0, 3);
        float velocity = 0;
        Vector2 newScale = new Vector2(groundWidth, groundHeight);

        if (groundType == GroundScript.GroundType.Normal)
        {
            velocity = Random.Range(minVelocity, maxVelocity);
            newScale = new Vector2(groundWidth, groundHeight);
        }
        else if (groundType == GroundScript.GroundType.JumpHigh)
        {
            velocity = Random.Range(minVelocity, maxVelocity);
            //	obj.GetComponent<GroundScript>().SetColor(10,10,10);
            newScale = new Vector2(groundWidth * 0.8f, groundHeight * 0.75f);
        }
        else if (groundType == GroundScript.GroundType.TimeBomb)
        {
            velocity = Random.Range(minVelocity * 1.25f, maxVelocity * 1.25f);
            newScale = new Vector2(groundWidth, groundHeight);
        }

        obj.GetComponent<GroundScript>().SetGround(newScale, 0, 0, velocity, groundType);
    }

    private void DifficultyCalculation()
    {
        if (groundIndex > difficultyLevel * difficultyGrowRate)
        {
            difficultyLevel = 1 + (groundIndex / difficultyGrowRate);

            float widthDifference = (1 - Mathf.Sin(Deg2Rad(difficultyLevel * widthGrowRateAdjust))) * widthGrow;
            float velocityDifference = (1 - Mathf.Sin(Deg2Rad(difficultyLevel * velocityGrowRateAdjust))) * velocityGrow;
            groundWidth -= widthDifference;
            minVelocity += velocityDifference;
            maxVelocity += velocityDifference;
        }
    }

    private float Deg2Rad(float deg)
    {
        return deg * Mathf.PI / 180;
    }
}