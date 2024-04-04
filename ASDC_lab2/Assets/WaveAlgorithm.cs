using UnityEngine;
using System.Collections.Generic;

public class WaveAlgorithm : MonoBehaviour
{
    public Vector2Int gridSize = new Vector2Int(10, 10); // Размер сетки
    public Material pathMaterial; // Материал для отображения пути

    private int[,] grid;
    //private Vector2Int startCell = new Vector2Int(0, 0); // Начальная точка по умолчанию
    private Vector2Int startCell; // Начальная точка по умолчанию
    //private Vector2Int targetCell = new Vector2Int(9, 9); // Конечная точка по умолчанию
    private Vector2Int targetCell; // Конечная точка по умолчанию
    private bool pathFound = false;

    GameObject[,] cubes;

    void Start()
    {
        // Инициализация сетки
        grid = new int[gridSize.x, gridSize.y];
        cubes = new GameObject[gridSize.x, gridSize.y];
        // Создание сетки и установка начальной и конечной клеток
        CreateGrid();
    }

    void Update()
    {
        // Проверка на нажатие ЛКМ
        if (Input.GetMouseButtonDown(0))
        {
            // Проверяем, был ли найден путь
            if (!pathFound)
            {
                // Получаем позицию курсора в мировых координатах
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // Преобразуем мировые координаты в координаты сетки
                int x = Mathf.RoundToInt(worldPos.x);
                int y = Mathf.RoundToInt(worldPos.y);
                // Проверяем, находится ли позиция в пределах сетки
                if (x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y)
                {
                    CreateObstacle(x, y);
                }
            }
        }

        // Проверка на нажатие клавиши "Space" (запуск волнового алгоритма)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindPath();
        }

        // установка начальной клетки
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Получаем позицию курсора в мировых координатах
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Преобразуем мировые координаты в координаты сетки
            startCell.x = Mathf.FloorToInt(worldPos.x);
            startCell.y = Mathf.FloorToInt(worldPos.y);
            DrawCell(startCell.x, startCell.y, Color.green);
        }

        // установка конечной клетки
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Получаем позицию курсора в мировых координатах
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Преобразуем мировые координаты в координаты сетки
            targetCell.x = Mathf.FloorToInt(worldPos.x);
            targetCell.y = Mathf.FloorToInt(worldPos.y);
            DrawCell(targetCell.x, targetCell.y, Color.red);
        }
    }

    void CreateGrid()
    {
        // Создаем сетку клеток
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x, y] = 0;
                DrawCell(x, y, Color.white);
            }
        }
        //// Отображаем начальную и конечную клетки
        //DrawCell(startCell.x, startCell.y, Color.green);
        //DrawCell(targetCell.x, targetCell.y, Color.red);
    }

    void DrawCell(int x, int y, Color color)
    {
        // Создаем куб для отображения клетки
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(x, y, 0);
        cube.GetComponent<Renderer>().material.color = color;

        if (cubes[x, y] != null)
        {
            Destroy(cubes[x, y]);
        }  
        cubes[x, y] = cube;
    }

    void CreateObstacle(int x, int y)
    {
        // Создаем преграду в выбранной клетке
        grid[x, y] = 1;
        DrawCell(x, y, Color.black);
    }

    void FindPath()
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        // Массив для отслежевания посещенных клеток
        bool[,] visited = new bool[gridSize.x, gridSize.y];
        // Массив для хранения предыдущих клеток
        Vector2Int[,] parent = new Vector2Int[gridSize.x, gridSize.y];

        Vector2Int start = startCell;
        Vector2Int target = targetCell;
        // Добавляем начальную точку и сразу помечаем как пройденную
        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == target)
            {
                MarkPath(parent, target);
                return;
            }

            foreach (var neighbor in GetNeighbors(current))
            {
                int x = neighbor.x;
                int y = neighbor.y;

                // Проверка соседней клетки на проходимость 
                if (IsValidCell(x,y) && grid[x,y] == 0 && !visited[x,y])
                {
                    queue.Enqueue(neighbor);
                    visited[x,y] = true;
                    parent[x,y] = current;
                }
            }
        }
    }

    void MarkPath(Vector2Int[,] parent , Vector2Int target)
    {
        Vector2Int current = target;
        while (current != startCell)
        {
            DrawCell(current.x, current.y, Color.magenta);
            current = parent[current.x, current.y];
        }
    }

    bool IsValidCell(int x, int y)
    {
        return x>= 0 && x<gridSize.x && y>=0 && y<gridSize.y;
    }

    List<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        int x = cell.x;
        int y = cell.y;

        List<Vector2Int> neighbors = new List<Vector2Int>();

        neighbors.Add(new Vector2Int(x+1,y));
        neighbors.Add(new Vector2Int(x-1,y));
        neighbors.Add(new Vector2Int(x,y+1));
        neighbors.Add(new Vector2Int(x,y-1));

        return neighbors;
    }
}