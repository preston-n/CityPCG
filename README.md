### City PCG Tool
  * **Description:** A tool made for generating random cities with different layouts and buildings
  * **Role:** Creator
  * *Made in Unity and with C#*  
![Picture of Generated City](https://github.com/preston-n/CityPCG/blob/main/Screenshots/City3.png?raw=true)

#Options
  * The city is created based on the options the player sets located in ‘Data\Options.txt’ the options are as follows:
    * MaxBuildingHeight [Controls the max height(z) of the building]
    * MinBuildingHeight [Controls the minimum height(z) of the building]
    * MaxBuildingWidth [Controls the max width(y) of the building]
    * MinBuildingWidth [Controls the minimum width(y) of the building]
    * MaxBuildingLength  [Controls the max length(x) of the building]
    * MinBuildingLength [Controls the minimum length(x) of the building]
    * MaxBuildingSpread [Controls how close buildings can be to each other]
    * MinBuildingSpread [Controls how close buildings can be to each other]
    * CityLength  [Controls the size of  actual city map(x) where buildings can spawn]
    * CityWidth [Controls the size of  actual city map(y) where buildings can spawn]
    * numOfBuildings [Controls the number of buildings that will be attempted to be built]
    * startX [Where the bottom left edge of the city will start (x pos)]
    * startY [Where the bottom left edge of the city will start (y pos)]
    * startZ [Where the bottom left edge of the city will start (z pos)]
    * DebugMode [Spreads tiles apart, used for internal testing]
  * Recommended max values for height, length and width is 25 but you can make a tradeoff of size from one section to another or try higher values if you have a powerful enough pc. The city length and width controls how many tiles can be fit in a 2d grid. The building spread option controls how close the buildings can be to each other, for example if buildings were touching each other they would both must have a spread of 0 and if they were 6 tiles away from any other building they would have a spread of 3.The start x,y,z values determine where in the world the city will be generated from.

![Picture of Generated City](https://github.com/preston-n/CityPCG/blob/main/Screenshots/City2.png?raw=true)
