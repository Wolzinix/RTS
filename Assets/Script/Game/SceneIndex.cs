using System.Collections.Generic;

public static class SceneIndex
{
    static readonly Dictionary<int,string> DicoOfScene = new()
    {
        { 0,"MainMenu" },
        { 1,"Firstbattle" },
        { 2,"SecondBattle" },
        { 3, "ScreenLoadFirstSecond" },
        { 4,"ScreenLoadMainFirst" }
    };
    

    public static string GetIndexOfScene(int numOfScene)
    {
        return DicoOfScene[numOfScene];
    }
}
