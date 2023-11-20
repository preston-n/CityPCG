using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public partial class Automaton : MonoBehaviour
{
	//Tile Settings
	public float GridSize = 5.0f; //Size of floor and wall tiles in units

	//Building Settings
	public int MaxBHeight = 5; //Maximum height and width of tile map
	public int MinBHeight = 5; //Maximum height and width of tile map
	public int MaxBWidth = 5; //Maximum height and width of tile map
	public int MinBWidth = 5; //Maximum height and width of tile map
	public int MaxBLength = 5; //Maximum height and width of tile map
	public int MinBLength = 5; //Maximum height and width of tile map
	public int MaxBSpread = 3; //Maximum height and width of tile map
	public int MinBSpread = 0; //Maximum height and width of tile map

	//City Settings
	public int CityLength = 5; //Maximum height and width of tile map
	public int CityWidth = 5; //Maximum height and width of tile map
	public int numOfBuildings = 100; //Maximum height and width of tile map

	//Class Vars
	public System.Random RNG;

	//Location in World where we will start generating the city
	public float startX = 0.0f;
	public float startY= 0.0f;
	public float startZ = 0.0f;

	//List of spaces occupied by buildings
	private int[][] CityMap;

	//List of Building Objects
	private GameObject[] buildings;

	void Start()
	{
		//Read in Setting from text file
		string[] lines = System.IO.File.ReadAllLines(@"Data\Options.txt");

		//Read Building Options
		MaxBHeight = int.Parse(lines[1]);
		MinBHeight = int.Parse(lines[3]);
		MaxBWidth = int.Parse(lines[5]);
		MinBWidth = int.Parse(lines[7]);
		MaxBLength = int.Parse(lines[9]);
		MinBLength = int.Parse(lines[11]);
		MaxBSpread = int.Parse(lines[13]);
		MinBSpread = int.Parse(lines[15]);

		//Read City Options
		CityLength = int.Parse(lines[17]);
		CityWidth = int.Parse(lines[19]);
		numOfBuildings = int.Parse(lines[21]);

		//Read in Start Location
		startX = int.Parse(lines[23]);
		startY = int.Parse(lines[25]);
		startZ = int.Parse(lines[27]);

		//If debug mode enabled, spread out tiles from one another
		if(bool.Parse(lines[29]))
        {
			GridSize = 14.0f;
			GetComponent<Nuko>().GridSize = 14.0f;
		}
		else
        {
			GridSize = 7.0f;
			GetComponent<Nuko>().GridSize = 7.0f;
		}

		//Create the tile map GameObject[] buildings;
		buildings = new GameObject[numOfBuildings];
		for (int i = 0; i < numOfBuildings; ++i)
		{
			buildings[i] = new GameObject();
		}

        //Init City Grid
        CityMap = new int[CityLength+MaxBLength+MaxBSpread+1][];

		int cLength = CityLength + MaxBLength + MaxBSpread + 1;

        for (int i = 0; i < cLength; ++i)
		{
			CityMap[i]= new int[cLength];
			for (int j = 0; j < cLength; ++j)
			{
				CityMap[i][j] = 0;
			}
		}
		
		RNG = new System.Random();

		//Setup Building System
		GetComponent<Nuko>().Start();

        //Start Generating Buildings using Random Gaussian,
		//Stoping after we reach desired amount of buildings or failed to build too many times
        int numOfTries = 0;
		int currentBuildings = 0;
		while (numOfTries<7500 && currentBuildings < numOfBuildings)
		{
			//Distance from other buildings (determined from options)
			int distFromOther = RNG.Next(MinBSpread, MaxBSpread+1);

			//Randomly generate building width, length and height
			int fW = (int)RandomGaussian(MinBLength, MaxBLength);
			int sW = (int)RandomGaussian(MinBWidth, MaxBWidth);
			int bH = (int)RandomGaussian(MinBHeight, MaxBHeight);

			//Get Random postion on map
			int x = (int)RandomGaussian(MaxBSpread, CityLength - MaxBSpread);
			int y = (int)RandomGaussian(MaxBSpread, CityWidth - MaxBSpread);

			//Check if there is empty space in that location before building it
			if (canMakeBuilding(x - distFromOther, y - distFromOther, x + fW + distFromOther, y + sW + distFromOther, bH))
			{
				GetComponent<Nuko>().CreateBuilding(buildings[currentBuildings], startX + x * GridSize, startY, startZ + y * GridSize, bH, fW, sW);
				++currentBuildings;
			}
			else
			{
				++numOfTries;
			}
		}
	}

	//Check if space to build is unoccupied, if so make it
	bool canMakeBuilding(int sX, int sY, int fWidth, int sWidth,int h)
    {
		for (int i = sX; i < fWidth; ++i)
		{
			for (int j = sY; j <sWidth; ++j)
			{
				if (CityMap[i][j] != 0 )
					return false;
			}
		}

		//Now Make it
		for (int i = sX; i < fWidth; ++i)
		{
			for (int j = sY; j < sWidth; ++j)
			{
				CityMap[i][j] = h;
			}
		}
		return true;
	}

	//Random Gaussian (from https://answers.unity.com/questions/421968/normal-distribution-random.html)
	public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
	{
		float u, v, S;

		do
		{
			u = 2.0f * UnityEngine.Random.value - 1.0f;
			v = 2.0f * UnityEngine.Random.value - 1.0f;
			S = u * u + v * v;
		}
		while (S >= 1.0f);

		// Standard Normal Distribution
		float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

		// Normal Distribution centered between the min and max value
		// and clamped following the "three-sigma rule"
		float mean = (minValue + maxValue) / 2.0f;
		float sigma = (maxValue - mean) / 3.0f;
		return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
	}
}
