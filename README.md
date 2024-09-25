# City PCG Tool
  * **Description:** A tool made for creating virtual cities of varying buildings and layouts with procedural content generation systems
  * *Made in Unity and with C#*  
# Info
### Creating the buildings
The buildings use a tile based system for their creation. Tiles are combined one by one in order to create one part of the building, such as the face or the side of the building. Once all the parts of the building have been created, they are connected to each other. Some tiles can only be placed if they meet a certain requirement, such as window tiles can’t be placed on the first layer and roof tiles can only appear on the roof part of the building. This prevents the structures from ever looking strange or random. 

By using tiles to create the buildings we can have a lot of different factors for the buildings creation such as how long the building can be as well as how wide and tall. Using tiles also allows for them to be swapped in and out for other tiles. For example a window tile could be swapped out for a different window tile to change the look of the building. Tiles that have different art/textures could also be swapped to make certain parts of the buildings look more unique. 

### Creating the city
The city is created using a custom algorithm, however some settings can be customized to see different effects when creating the city. These settings allow for different min/max heights, width and lengths of buildings being constructed as well as their distance from one another. There are also settings to control how big the city can be and a option to limit the amount of buildings that can be created 

### Algorithm
The algorithm for generating the city uses gaussian random which is used for two parts of the specific parts of the city creation algorithm. The first part is for controlling the sizes of the buildings, this includes their length, width and height. This is so that most buildings will be close in relative size and gives a better effect than just using  the default ‘rand random’. The second part is for controlling the position of the buildings in the city. This only affects buildings positioning in the x and y direction on the city grid size that is defined by the user. By using gaussian random for the position of buildings we get the nice effect of all the buildings in the middle of the city being close together and dense and the further you go out from the middle to the edges of the city the less buildings there are close to each other and it is more sparse. 

# Screenshots
### Inside the city
![Picture of Generated City](https://github.com/preston-n/CityPCG/blob/main/Screenshots/City2.png?raw=true)
### Close up on buildings
![Picture of Generated City](https://github.com/preston-n/CityPCG/blob/main/Screenshots/City3.png?raw=true)
### Overview of the city
![Picture of Generated City](https://github.com/preston-n/CityPCG/blob/main/Screenshots/City1.png?raw=true)

