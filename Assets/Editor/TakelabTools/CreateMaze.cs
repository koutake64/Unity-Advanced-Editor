using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreateMaze : EditorWindow
{
    private int width = 10;
    private int height = 10;
    private float complexity = 0.5f; // 0.0 (簡単) 〜 1.0 (複雑)

    private GameObject mazeParent;

    [MenuItem("TakelabTools/Create Maze")]
    public static void ShowWindow()
    {
        GetWindow<CreateMaze>("Maze Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("迷路の設定", EditorStyles.boldLabel);
        width = EditorGUILayout.IntField("Xサイズ", width);
        height = EditorGUILayout.IntField("Zサイズ", height);
        complexity = EditorGUILayout.Slider("複雑さ", complexity, 0.0f, 1.0f);

        if (GUILayout.Button("迷路を生成"))
        {
            GenerateMaze();
        }
    }

    private void GenerateMaze()
    {
        // 既存の迷路を削除
        if (mazeParent != null)
        {
            DestroyImmediate(mazeParent);
        }

        mazeParent = new GameObject("Maze");

        // 迷路データ初期化
        int[,] maze = new int[width, height];
        for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
                maze[x, z] = 1; // 壁で埋める

        // ランダムな開始地点
        System.Random rand = new System.Random();
        int startX = rand.Next(1, width - 1);
        int startZ = rand.Next(1, height - 1);
        maze[startX, startZ] = 0;

        // 迷路の生成（Prim's Algorithm）
        List<Vector2Int> walls = new List<Vector2Int>();
        walls.Add(new Vector2Int(startX, startZ));

        while (walls.Count > 0)
        {
            int index = rand.Next(walls.Count);
            Vector2Int current = walls[index];
            walls.RemoveAt(index);

            int x = current.x;
            int z = current.y;

            List<Vector2Int> neighbors = GetNeighbors(x, z, width, height);

            foreach (Vector2Int n in neighbors)
            {
                int nx = n.x, nz = n.y;
                if (maze[nx, nz] == 1 && CountWallsAround(maze, nx, nz) >= 3)
                {
                    maze[nx, nz] = 0;
                    walls.Add(n);
                }
            }
        }

        // 複雑さに応じた壁の追加
        AddRandomWalls(maze, complexity, rand);

        // 迷路の可視化
        CreateMazeObjects(maze);
    }

    private List<Vector2Int> GetNeighbors(int x, int z, int width, int height)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (x > 1) neighbors.Add(new Vector2Int(x - 1, z));
        if (x < width - 2) neighbors.Add(new Vector2Int(x + 1, z));
        if (z > 1) neighbors.Add(new Vector2Int(x, z - 1));
        if (z < height - 2) neighbors.Add(new Vector2Int(x, z + 1));

        return neighbors;
    }

    private int CountWallsAround(int[,] maze, int x, int z)
    {
        int count = 0;
        if (maze[x - 1, z] == 1) count++;
        if (maze[x + 1, z] == 1) count++;
        if (maze[x, z - 1] == 1) count++;
        if (maze[x, z + 1] == 1) count++;
        return count;
    }

    private void AddRandomWalls(int[,] maze, float complexity, System.Random rand)
    {
        int totalCells = maze.GetLength(0) * maze.GetLength(1);
        int wallCount = (int)(totalCells * complexity);

        for (int i = 0; i < wallCount; i++)
        {
            int x = rand.Next(1, maze.GetLength(0) - 1);
            int z = rand.Next(1, maze.GetLength(1) - 1);
            maze[x, z] = 1;
        }
    }

    private void CreateMazeObjects(int[,] maze)
    {
        GameObject wallPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);

        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int z = 0; z < maze.GetLength(1); z++)
            {
                if (maze[x, z] == 1)
                {
                    GameObject wall = Instantiate(wallPrefab, new Vector3(x, 0, z), Quaternion.identity);
                    wall.transform.parent = mazeParent.transform;
                }
            }
        }

        DestroyImmediate(wallPrefab);
    }
}
