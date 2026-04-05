using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace _Strategy.Runtime.Board
{
    [Serializable]
    public class SquareBoardHexGenerator : IHexGenerator
    {
        [SerializeField] private int _size;

        public SquareBoardHexGenerator(int size)
        {
            _size = size;
        }

        public Hex[] Generate()
        {
            var hexes = new Hex[_size * _size];
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    var hex = new Hex(i, j);
                    hexes[(i * _size) + j] = hex;
                }
            }

            return hexes;
        }
    }

    public class HexBoardGenerator : IHexGenerator
    {
        [SerializeField] private int _size;

        public HexBoardGenerator(int size)
        {
            _size = size;
        }

        public Hex[] Generate()
        {
            var hexes = new List<Hex>();
            Hex startingHex = new Hex(1, 1);
            var hexesToCheck = new Queue<Hex>();
            var newlyAddedHexes = new List<Hex>();
            hexesToCheck.Enqueue(startingHex);
            int safety = 0;
            for (int i = 0; i < _size; i++)
            {
                while (safety < 1000 && hexesToCheck.Count > 0)
                {
                    safety++;
                   if (hexesToCheck.Count == 0)
                       break;

                   var hex = hexesToCheck.Dequeue();
                   foreach (var direction in Hex.Directions)
                   {
                       var neighbor = hex.Neighbor(direction);
                       if (hexes.Contains(neighbor))
                           continue;

                       hexes.Add(neighbor);
                       newlyAddedHexes.Add(neighbor);
                   } 
                }
                
                foreach (var newlyAddedHex in newlyAddedHexes)
                {
                    hexesToCheck.Enqueue(newlyAddedHex);
                }

                newlyAddedHexes.Clear();
            }

            return hexes.ToArray();
        }

        public Hex[] Generate2()
        {
            var hexes = new List<Hex>();
            Hex startingHex = new Hex(1, 1);
            var hexesToCheck = new Queue<Hex>();
            var newlyAddedHexes = new List<Hex>();
            hexesToCheck.Enqueue(startingHex);
            int safety = 0;
            for (int i = 0; i < _size; i++)
            {
                while (safety < 6)
                {
                    safety++;
                    Debug.Log($"[SquareBoardHexGenerator.Generate] {safety}");
                    if (hexesToCheck.Count == 0)
                        break;

                    var hex = hexesToCheck.Dequeue();
                    //foreach (var direction in Hex.Directions)
                    //{
                    //    var neighbor = hex.Neighbor(direction);
                    //    if (hexes.Contains(neighbor))
                    //    {
                    //        Debug.Log($"[SquareBoardHexGenerator.Generate] Didn't add {neighbor} because it's already in the list of hexes");
                    //        continue;
                    //    }
//
                    //    Debug.Log($"[SquareBoardHexGenerator.Generate] Added {neighbor}");
                    hexes.Add(hex);
                    newlyAddedHexes.Add(hex);
                    //}
                }

                // foreach (var newlyAddedHex in newlyAddedHexes)
                // {
                //     hexesToCheck.Enqueue(newlyAddedHex);
                // }

                //newlyAddedHexes.Clear();
            }

            return hexes.ToArray();
        }
    }

    public class PerlinNoiseHexGenerator : IHexGenerator
    {
        [SerializeField] private int _size;
        [SerializeField] private float _scale;
        [SerializeField] private float _threshold;
        [SerializeField] private bool _ensureConnectivity;

        public PerlinNoiseHexGenerator(int size, float scale, float threshold, bool ensureConnectivity = true)
        {
            _size = size;
            _scale = scale;
            _threshold = threshold;
            _ensureConnectivity = ensureConnectivity;
        }

        public Hex[] Generate()
        {
            var randomOffset = UnityEngine.Random.insideUnitSphere * 100;
            var Hexs = new List<Hex>();
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    // Generate Perlin noise-based value
                    float noiseValue = Mathf.PerlinNoise(x / _scale + randomOffset.x, y / _scale + randomOffset.y);
                    if (noiseValue > _threshold)
                    {
                        Hexs.Add(new Hex(x, y));
                    }
                }
            }

            if (_ensureConnectivity)
            {
                Hexs = EnsureConnectivity(Hexs);
            }

            return Hexs.ToArray();
        }

        private List<Hex> EnsureConnectivity(List<Hex> Hexs)
        {
            if (Hexs.Count == 0) return Hexs;

            var connectedHexs = GetLargestConnectedComponent(Hexs);

            // If we lost too many Hexs, try bridging small gaps
            if (connectedHexs.Count < Hexs.Count * 0.7f)
            {
                connectedHexs = BridgeSmallGaps(connectedHexs, Hexs);
            }

            return connectedHexs;
        }

        private List<Hex> GetLargestConnectedComponent(List<Hex> hexes)
        {
            var hexSet = new HashSet<Vector2Int>();
            foreach (var hex in hexes)
            {
                hexSet.Add(new Vector2Int(hex.Q, hex.R));
            }

            var visited = new HashSet<Vector2Int>();
            var largestComponent = new List<Vector2Int>();

            foreach (var HexPos in hexSet)
            {
                if (!visited.Contains(HexPos))
                {
                    var component = new List<Vector2Int>();
                    FloodFill(HexPos, hexSet, visited, component);

                    if (component.Count > largestComponent.Count)
                    {
                        largestComponent = component;
                    }
                }
            }

            var result = new List<Hex>();
            foreach (var pos in largestComponent)
            {
                result.Add(new Hex(pos.x, pos.y));
            }

            return result;
        }

        private void FloodFill(Vector2Int start, HashSet<Vector2Int> hexSet, HashSet<Vector2Int> visited, List<Vector2Int> component)
        {
            var stack = new Stack<Vector2Int>();
            stack.Push(start);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (visited.Contains(current)) continue;

                visited.Add(current);
                component.Add(current);

                // Check 4-directional neighbors
                var neighbors = new Vector2Int[]
                {
                    new Vector2Int(current.x + 1, current.y),
                    new Vector2Int(current.x - 1, current.y),
                    new Vector2Int(current.x, current.y + 1),
                    new Vector2Int(current.x, current.y - 1)
                };

                foreach (var neighbor in neighbors)
                {
                    if (hexSet.Contains(neighbor) && !visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                    }
                }
            }
        }

        private List<Hex> BridgeSmallGaps(List<Hex> connectedHexs, List<Hex> originalHexs)
        {
            var connectedSet = new HashSet<Vector2Int>();
            foreach (var hex in connectedHexs)
            {
                connectedSet.Add(new Vector2Int(hex.Q, hex.R));
            }

            var originalSet = new HashSet<Vector2Int>();
            foreach (var hex in originalHexs)
            {
                originalSet.Add(new Vector2Int(hex.Q, hex.R));
            }

            var result = new List<Hex>(connectedHexs);

            // Find isolated Hexs that are close to the main component
            foreach (var Hex in originalHexs)
            {
                var pos = new Vector2Int(Hex.Q, Hex.R);
                if (!connectedSet.Contains(pos))
                {
                    if (HasNearbyConnectedHex(pos, connectedSet))
                    {
                        result.Add(Hex);
                        connectedSet.Add(pos);
                    }
                }
            }

            return result;
        }

        private bool HasNearbyConnectedHex(Vector2Int pos, HashSet<Vector2Int> connectedSet)
        {
            // Check in a 3x3 area around the Hex
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    var neighbor = new Vector2Int(pos.x + dx, pos.y + dy);
                    if (connectedSet.Contains(neighbor))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class BlobHexGenerator : IHexGenerator
    {
        [SerializeField] private int _radius;
        [SerializeField] private float _holeChance;
        [SerializeField] private int _minHoleDistance;

        public BlobHexGenerator(int radius, float holeChance, int minHoleDistance = 2)
        {
            _radius = radius;
            _holeChance = holeChance;
            _minHoleDistance = minHoleDistance;
        }

        public Hex[] Generate()
        {
            var center = _radius;
            var Hexs = new List<Hex>();
            var holes = new List<Vector2Int>();

            for (int x = 0; x < 2 * _radius; x++)
            {
                for (int y = 0; y < 2 * _radius; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (distance <= _radius)
                    {
                        // Check if this should be a hole
                        bool shouldBeHole = UnityEngine.Random.value < _holeChance;

                        // Ensure holes aren't too close to each other or the edge
                        if (shouldBeHole && distance > _minHoleDistance &&
                            distance < _radius - _minHoleDistance)
                        {
                            if (IsFarFromOtherHoles(new Vector2Int(x, y), holes))
                            {
                                holes.Add(new Vector2Int(x, y));
                                continue;
                            }
                        }

                        Hexs.Add(new Hex(x, y));
                    }
                }
            }

            return Hexs.ToArray();
        }

        private bool IsFarFromOtherHoles(Vector2Int position, List<Vector2Int> existingHoles)
        {
            foreach (var hole in existingHoles)
            {
                if (Vector2Int.Distance(position, hole) < _minHoleDistance)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class CellularAutomataHexGenerator : IHexGenerator
    {
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private float _initialFillPercentage;
        [SerializeField] private int _smoothingIterations;
        [SerializeField] private bool _ensureConnectivity;

        public CellularAutomataHexGenerator(int width, int height, float initialFillPercentage,
            int smoothingIterations, bool ensureConnectivity = true)
        {
            _width = width;
            _height = height;
            _initialFillPercentage = initialFillPercentage;
            _smoothingIterations = smoothingIterations;
            _ensureConnectivity = ensureConnectivity;
        }

        public Hex[] Generate()
        {
            int[,] map = InitializeMap();

            for (int i = 0; i < _smoothingIterations; i++)
            {
                map = SmoothMap(map);
            }

            var Hexs = ConvertToHexs(map);

            if (_ensureConnectivity)
            {
                Hexs = EnsureConnectivity(Hexs);
            }

            return Hexs;
        }

        private int[,] InitializeMap()
        {
            int[,] map = new int[_width, _height];
            var random = new System.Random();

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    map[x, y] = random.NextDouble() < _initialFillPercentage ? 1 : 0;
                }
            }

            return map;
        }

        private int[,] SmoothMap(int[,] map)
        {
            int[,] newMap = new int[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    int neighborCount = CountHexNeighbors(map, x, y);

                    if (neighborCount > 4)
                    {
                        newMap[x, y] = 1;
                    }
                    else if (neighborCount < 4)
                    {
                        newMap[x, y] = 0;
                    }
                    else
                    {
                        newMap[x, y] = map[x, y];
                    }
                }
            }

            return newMap;
        }

        private int CountHexNeighbors(int[,] map, int x, int y)
        {
            int count = 0;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < _width && ny >= 0 && ny < _height)
                    {
                        count += map[nx, ny];
                    }
                    else
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private Hex[] ConvertToHexs(int[,] map)
        {
            var Hexs = new List<Hex>();

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (map[x, y] == 1)
                    {
                        Hexs.Add(new Hex(x, y));
                    }
                }
            }

            return Hexs.ToArray();
        }

        private Hex[] EnsureConnectivity(Hex[] Hexs)
        {
            var HexList = new List<Hex>(Hexs);
            return EnsureConnectivity(HexList).ToArray();
        }

        private List<Hex> EnsureConnectivity(List<Hex> Hexs)
        {
            if (Hexs.Count == 0) return Hexs;

            var HexSet = new HashSet<Vector2Int>();
            foreach (var Hex in Hexs)
            {
                HexSet.Add(new Vector2Int(Hex.Q, Hex.R));
            }

            var visited = new HashSet<Vector2Int>();
            var largestComponent = new List<Vector2Int>();

            foreach (var HexPos in HexSet)
            {
                if (!visited.Contains(HexPos))
                {
                    var component = new List<Vector2Int>();
                    FloodFill(HexPos, HexSet, visited, component);

                    if (component.Count > largestComponent.Count)
                    {
                        largestComponent = component;
                    }
                }
            }

            var result = new List<Hex>();
            foreach (var pos in largestComponent)
            {
                result.Add(new Hex(pos.x, pos.y));
            }

            return result;
        }

        private void FloodFill(Vector2Int start, HashSet<Vector2Int> HexSet, HashSet<Vector2Int> visited, List<Vector2Int> component)
        {
            var stack = new Stack<Vector2Int>();
            stack.Push(start);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (visited.Contains(current)) continue;

                visited.Add(current);
                component.Add(current);

                var neighbors = new Vector2Int[]
                {
                    new Vector2Int(current.x + 1, current.y),
                    new Vector2Int(current.x - 1, current.y),
                    new Vector2Int(current.x, current.y + 1),
                    new Vector2Int(current.x, current.y - 1)
                };

                foreach (var neighbor in neighbors)
                {
                    if (HexSet.Contains(neighbor) && !visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                    }
                }
            }
        }
    }
}