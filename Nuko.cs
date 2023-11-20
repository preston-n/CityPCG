using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class Nuko : MonoBehaviour
{
	//Building Settings
	public float GridSize = 5.0f; //Size of floor and wall tiles in units
	public int BHeight = 5; //Maximum height and width of tile map
	public int frontBWidth = 5; //Maximum height and width of tile map
	public int sideBWidth = 5; //Maximum height and width of tile map

    //Current Offset of Building being built
    private float currX = 0.0f;
	private float currY = 0.0f;
	private float currZ = 0.0f;

    //Location in World where we will start generating the city
    public float startX = 0.0f;
	public float startY= 0.0f;
	public float startZ = 0.0f;

	//Window Settings
	private int windowMode = 0;
	private int windowType = 1;
	private int numOfWindows = 1;
	private int[] windowTypesArry;
	private int wSkipH = 0;
	private int wSkipW = 0;

	//Tiles
	public GameObject doorStyle1;
	public GameObject wallStyle1;

	public GameObject windowStyle1;
	public GameObject windowStyle2;
	public GameObject windowStyle3;
	public GameObject windowStyle4;

	public GameObject roofStyle1;
	public GameObject roofStyle2;
	public GameObject roofStyle3;

    public GameObject floorStyle1;

    //Mats
    public Material matStyle1;
	public Material matStyle2;
	public Material matStyle3;
	public Material matStyle4;
	public Material matStyle5;

	//Class Vars
    private Dictionary<string, GameObject> Prefabs; //Dictionary of all PCG prefabs
    public System.Random RNG;

    private int[][] FrontMap;
    private int[][] SideMap;
    private int[][] RoofMap;
    private int[][] FloorMap;

    private List<MeshFilter> MeshFilterL;

    private GameObject[] objs;
    private int walk = 0;

    private bool loaded = false;

	public void Start()
	{
		//Load all the prefabs we need for map generation (note that these must be in a "Resources" folder)
		Prefabs = new Dictionary<string, GameObject>();

		//Add prefabs to Dictionary (should use a list if adding more styles)
		//Optmize the meshes we add
		Prefabs.Add("door", optCurrM(doorStyle1));

		Prefabs.Add("wall", optCurrM(wallStyle1));

		Prefabs.Add("window", optCurrM(windowStyle1));

		Prefabs.Add("windowStyle2", optCurrM(windowStyle2));

		Prefabs.Add("windowStyle3", optCurrM(windowStyle3));

		Prefabs.Add("windowStyle4", optCurrM(windowStyle4));

		Prefabs.Add("roof", optCurrM(roofStyle1));

		Prefabs.Add("roofStyle2", optCurrM(roofStyle2));

		Prefabs.Add("roofStyle3", optCurrM(roofStyle3));

        Prefabs.Add("floor", optCurrM(floorStyle1));

        RNG = new System.Random();
		loaded = true;
	}

	//Make building given specs
	public void CreateBuilding(GameObject building, float sX, float sY, float sZ, int height,int fWidth,int sWidth)
    {
		if (loaded)
		{
			//Init vars
			startX = sX;
			startY = sY;
			startZ = sZ;
			currX = startX;
			currY = startY;
			currZ = startZ;

			BHeight = height;
			frontBWidth = fWidth;
			sideBWidth = sWidth;

			FrontMap = new int[BHeight][];
			SideMap = new int[BHeight][];
			RoofMap = new int[frontBWidth][];
			FloorMap = new int[frontBWidth][];

			//Init Front/Back of Building
			for (int i = 0; i < BHeight; ++i)
			{
				FrontMap[i] = new int[frontBWidth];
				for (int j = 0; j < frontBWidth; ++j)
				{
					FrontMap[i][j] = 0;
				}
			}
			//Init Sides
			for (int i = 0; i < BHeight; ++i)
			{
				SideMap[i] = new int[sideBWidth];
				for (int j = 0; j < sideBWidth; ++j)
				{
					SideMap[i][j] = 0;
				}
			}

			//Init Roof
			for (int i = 0; i < frontBWidth; ++i)
			{
				RoofMap[i] = new int[sideBWidth];
				for (int j = 0; j < sideBWidth; ++j)
				{
					RoofMap[i][j] = 1;
				}
			}

			//Init Floors
			for (int i = 0; i < frontBWidth; ++i)
			{
				FloorMap[i] = new int[sideBWidth];
				for (int j = 0; j < sideBWidth; ++j)
				{
					FloorMap[i][j] = 1;
				}
			}

			//Make list containing all objs used to create building
			objs =new GameObject[sideBWidth*BHeight*2+frontBWidth*BHeight*2 + frontBWidth* sideBWidth*(BHeight+1)];
			
			MeshFilterL = new List<MeshFilter>();
			walk = 0;

		
			//Determine distance between windows (both vert and horz)
			wSkipH = RNG.Next(0, 4);
			wSkipW = RNG.Next(0, 4);

			//Determines the number of diffrent window types we will have
			numOfWindows = RNG.Next(1, 5);
			windowTypesArry = new int[numOfWindows];
			for (int i = 0; i < numOfWindows; ++i)
			{
				windowTypesArry[i]= RNG.Next(3, 7);
			}

			//Make all parts of the building
			MakeFrontFace();
			MakeSideFace();
			MakeRoof();

			//Spawn it in then combine all meshes into one to reduce lag
			SpawnBuilding();
			opt(building);
		}
	}

	//Make front (and back) facing side of building
	void MakeFrontFace()
    {
		int nextWindow = 0;
		for (int i = 1; i < BHeight; ++i)
		{
			//Determine if we are adding windows on this spot and floor
			if ((wSkipH >= 2 && (i + 1) % wSkipH == 0) || wSkipH <= 1)
			{
				//Get current window type and go to the next window type after
				windowType = windowTypesArry[nextWindow];
				++nextWindow;
				if (nextWindow >= numOfWindows)
					nextWindow = 0;

				//Set building to use window
				for (int j = 0; j < frontBWidth; ++j)
				{
					if (wSkipW >= 2)
					{
						if ((j + 1) % wSkipW == 0)
							FrontMap[i][j] = windowType;
					}
					else
					{
						FrontMap[i][j] = windowType;
					}
				}
			}
		}

        //Add Front Door
        int t = frontBWidth / 2;
        FrontMap[0][t] = 1;
		//Add another door if it will be uneven
        if (frontBWidth % 2 == 0)
		{
            FrontMap[0][t - 1] = 1;
        }

	}

	void MakeSideFace()
	{
		int nextWindow = 0;
		for (int i = 1; i < BHeight; ++i)
		{
            //Determine if we are adding windows on this spot and floor
            if ((wSkipH >= 2 && (i + 1) % wSkipH == 0) || wSkipH <= 1)
			{
                //Get current window type and go to the next window type after
                windowType = windowTypesArry[nextWindow];
				++nextWindow;
				if (nextWindow >= numOfWindows)
					nextWindow = 0;

                //Set building to use window
                for (int j = 0; j < sideBWidth; ++j)
				{
                    if (wSkipW >= 2)
					{
						if ((j + 1) % wSkipW == 0)
							SideMap[i][j] = windowType;
					}
					else
					{
						SideMap[i][j] = windowType;
					}
				}
			}
		}



	}

	//Check if spot on Roof is valid to add "roof access" ontop of
	bool checkRoofPos(int x,int y)
    {
		if (x == 0 || y == 0 || x == frontBWidth || y == sideBWidth)
			return false;

        //If all sides around roof are "empty" we can place "roof access" ontop of it
        if (RoofMap[x][y]==1 && RoofMap[x+1][y] == 1 && RoofMap[x-1][y] == 1 &&
			RoofMap[x+1][y+1] == 1 && RoofMap[x-1][y+1] == 1 && RoofMap[x+1][y-1] == 1 &&
			RoofMap[x-1][y-1] == 1 && RoofMap[x][y-1] == 1 && RoofMap[x][y+1] == 1 )
        {
			RoofMap[x][y] = 3;

			RoofMap[x + 1][y] = 2; 
			RoofMap[x - 1][y] = 2;
			RoofMap[x][y - 1] = 2;
			RoofMap[x][y + 1] = 2;
			RoofMap[x + 1][y + 1] = 2; 
			RoofMap[x - 1][y + 1] = 2;
			RoofMap[x + 1][y - 1] = 2; 
			RoofMap[x - 1][y - 1] = 2;

			return true;
		}
		return false;
    }


	//Create Roof of Building
	void MakeRoof()
	{
		for (int i = 1; i < frontBWidth-1; ++i)
		{
			for (int j = 1; j < sideBWidth-1; ++j)
			{
				if (RNG.Next(5, 101) <= 5)
				{
					checkRoofPos(i, j);
					RoofMap[i][j] = RNG.Next(3, 5);
				}
			}
		}

	}

	//Spawn a door, note that this is only for one side of the building
	//Backside will just have a normal wall
	void SpawnDoor(string obj)
	{

		objs[walk] = Instantiate(Prefabs[obj], new Vector3(currX, currY, currZ), Quaternion.identity);
		++walk;
		objs[walk] = Instantiate(Prefabs["wall"], new Vector3(currX, currY, sideBWidth * GridSize + currZ - 1.0f), Quaternion.identity);
		++walk;
	}

	//Spawn object Front and backside of building
	void Spawn(string obj)
	{
		
		objs[walk]= Instantiate(Prefabs[obj], new Vector3(currX,currY, currZ), Quaternion.identity);
		++walk;
		objs[walk] = Instantiate(Prefabs[obj], new Vector3(currX, currY, sideBWidth * GridSize + currZ-1.0f), Quaternion.identity);
		++walk;
	}

    //Spawn object on both sides of building
    //Also make sure object is rotated 90 degrees
    void SpawnSide(string obj)
	{
		objs[walk] = Instantiate(Prefabs[obj], new Vector3(startX - (GridSize / 2.0f) -0.5f, currY, currZ+( GridSize / 2.0f)-0.5f), Quaternion.AngleAxis(90, Vector3.up));
		++walk;
		objs[walk] = Instantiate(Prefabs[obj], new Vector3(GridSize*frontBWidth + startX - (GridSize / 2.0f) - 0.5f, currY, currZ + (GridSize / 2.0f) - 0.5f), Quaternion.AngleAxis(90, Vector3.up));
		++walk;
	}

	//Spawn Roof of building
	void SpawnRoof(string obj)
	{
		objs[walk] = Instantiate(Prefabs[obj], new Vector3(currX-0.5f, currY + GridSize*BHeight - GridSize/2.0f, currZ + GridSize / 2.0f - 0.5f), Quaternion.AngleAxis(90, Vector3.right));
		++walk;
	}

	//Spawn floor inside of building
	void SpawnFloor(string obj)
	{
		objs[walk] = Instantiate(Prefabs[obj], new Vector3(currX - 0.5f,  currY - GridSize / 2.0f, currZ + GridSize / 2.0f - 0.5f), Quaternion.AngleAxis(90, Vector3.right));
		++walk;
	}

	//Spawn the building we have created
	//This will go through the lists and 
	//Spawn items based on their value (if add more items to spawn use a list to itterate through spawns)
	void SpawnBuilding()
	{
		//Spawn Front and Back
		for (int i = 0; i < BHeight; ++i)
		{
			for (int j = 0; j < frontBWidth; ++j)
			{

				//Tiles
				switch (FrontMap[i][j])
				{
					case 0:
						Spawn("wall");
						break;
					case 1:
						SpawnDoor("door");
						break;
					case 2:
						Spawn("wall");
						break;
					case 3:
						Spawn("window");
						break;
					case 4:
						Spawn("windowStyle2");
						break;
					case 5:
						Spawn("windowStyle3");
						break;
					case 6:
						Spawn("windowStyle4");
						break;
					default:
						Spawn("window");
						break;
				}
				currX += GridSize;
			}
			currY += GridSize;
			currX = startX;
		}

        //Spawn Sides (must reset position eachtime)
        currX = startX;
        currY = startY;
        currZ = startZ;
        for (int i = 0; i < BHeight; ++i)
		{
			for (int j = 0; j < sideBWidth; ++j)
			{

				switch (SideMap[i][j])
				{
					case 0:
						SpawnSide("wall");
						break;
					case 1:
						SpawnSide("wall");
						break;
					case 2:
						SpawnSide("wall");
						break;
					case 3:
						SpawnSide("window");
						break;
					case 4:
						SpawnSide("windowStyle2");
						break;
					case 5:
						SpawnSide("windowStyle3");
						break;
					case 6:
						SpawnSide("windowStyle4");
						break;
					default:
						SpawnSide("window");
						break;
				}

				currZ += GridSize;
			}
			currY += GridSize;
			currZ = startZ;
		}

        //Spawn Roof
        currX = startX;
        currY = startY;
        currZ = startZ;
        for (int i = 0; i < frontBWidth; ++i)
		{
			for (int j = 0; j < sideBWidth; ++j)
			{

				switch (RoofMap[i][j])
				{
					case 0:
						SpawnRoof("roof");
						break;
					case 1:
						SpawnRoof("roof");
						break;
					case 2:
						SpawnRoof("roof");
						break;
					case 3:
						SpawnRoof("roofStyle2");
						break;
					case 4:
						SpawnRoof("roofStyle3");
						break;
					default:
						SpawnRoof("roof");
						break;
				}

				currZ += GridSize;
			}
			currX += GridSize;
			currZ = startZ; 
		}

        //Spawn Floors
        currX = startX;
        currY = startY;
        currZ = startZ;
        for (int h = 0; h < BHeight; ++h)
		{
			
			for (int i = 0; i < frontBWidth; ++i)
			{
				for (int j = 0; j < sideBWidth; ++j)
				{

					switch (FloorMap[i][j])
					{
						case 0:
							SpawnFloor("floor");
							break;
						case 1:
							SpawnFloor("floor");
							break;
						default:
							SpawnFloor("floor");
							break;
					}

					currZ += GridSize;
				}
				currX += GridSize;
				currZ = startZ;
			}
			currX = startX;
			currY += GridSize;
		}
	}


	//Optimize Current mesh
	GameObject optCurrM(GameObject curr)
	{
		MeshFilter[] meshFilters = curr.GetComponentsInChildren<MeshFilter>();

		//Get the number of the meshes with a shared mesh 
		int i = 0;
		int realSize = 0;
		while (i < meshFilters.Length)
		{
			if (meshFilters[i] && meshFilters[i].sharedMesh)
			{
				++realSize;
			}
			i++;
		}

		//Return if none
        if (realSize <= 0)
            return curr;

        //Used to combine the mesh into one
        CombineInstance[] combine = new CombineInstance[realSize];

        i = 0;
		realSize = 0;
		//Combine the sharedMeshes together
		while (i < meshFilters.Length)
		{
			if (meshFilters[i] && meshFilters[i].sharedMesh)
			{
				combine[realSize].mesh = meshFilters[i].sharedMesh;
				combine[realSize].transform = meshFilters[i].transform.localToWorldMatrix;
				++realSize;
			}
			i++;
		}

		//New Mesh that will just be one object using all the 
		//combined meshes
        GameObject t = new GameObject();

		//Add meshRenderer and Filter
        t.AddComponent<MeshRenderer>();
		t.AddComponent<MeshFilter>();

		//Add the combined meshes
		t.transform.GetComponent<MeshFilter>().mesh = new Mesh();
		t.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
		t.GetComponent<MeshFilter>().mesh.Optimize();

		//Now set the object to active
		t.transform.gameObject.SetActive(true);

		t.GetComponent<Renderer>().material = matStyle1;

		return t;
	}

	//Optmize the spawned building into one mesh
	void opt(GameObject building)
    {
		for (int index = 0; index < walk; ++index)
		{
			if (objs[index])
			{
				MeshFilterL.AddRange(objs[index].GetComponentsInChildren<MeshFilter>());
			}
		}

		int z = 0;
		int rSize = 0;
        //Get the number of the meshes with a shared mesh 
        while (z < MeshFilterL.Count)
		{
			if (MeshFilterL[z] && MeshFilterL[z].sharedMesh)
			{
				++rSize;
			}
			++z;
		}

		CombineInstance[] combine = new CombineInstance[rSize];
		int i = 0;
		rSize = 0;
        //Combine the sharedMeshes together
        while (i < MeshFilterL.Count)
		{
			if (MeshFilterL[i] && MeshFilterL[i].sharedMesh)		
			{
				combine[rSize].mesh = MeshFilterL[i].sharedMesh;
				combine[rSize].transform = MeshFilterL[i].transform.localToWorldMatrix;
				MeshFilterL[i].gameObject.SetActive(false);
				++rSize;
			}
			i++;
		}

		//Make the building into one giant mesh 
		building.AddComponent<MeshRenderer>();
		building.AddComponent<MeshFilter>();

		//Note the Index Format must be UInt32 to account for a large number of combined Meshes
		building.transform.GetComponent<MeshFilter>().mesh = new Mesh();
		building.transform.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		building.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
		building.GetComponent<MeshFilter>().mesh.Optimize();
		building.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		building.transform.gameObject.SetActive(true);

		//Give the building a random material
		switch (RNG.Next(0,5))
		{
			case 0:
				building.GetComponent<Renderer>().material = matStyle1;
				break;
			case 1:
				building.GetComponent<Renderer>().material = matStyle2;
				break;
			case 2:
				building.GetComponent<Renderer>().material = matStyle3;
				break;
			case 3:
				building.GetComponent<Renderer>().material = matStyle4;
				break;
			case 4:
				building.GetComponent<Renderer>().material = matStyle5;
				break;
			default:
				building.GetComponent<Renderer>().material = matStyle1;
				break;
		}
	}

}
