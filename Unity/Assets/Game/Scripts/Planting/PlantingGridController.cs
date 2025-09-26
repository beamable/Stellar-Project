using System.Collections.Generic;
using Farm.Managers;
using UnityEngine;

namespace Farm.Managers
{
    [System.Serializable]
    public class BlockRows
    {
        public int rowId;
        public List<PlantingBlock> blocks = new List<PlantingBlock>();
    }
    
    public class PlantingGridController : MonoBehaviour
    {
        [SerializeField] private Vector2 offset = new Vector2(0.5f, 0.5f);
        [SerializeField] private Transform minPoint, maxPoint;
        [SerializeField] private Transform blocksParent;
        [SerializeField] private PlantingBlock blockPrefab;
        [SerializeField] private LayerMask blockingLayer;

        private Vector2 _minRounded;
        private Vector2 _maxRounded;
        private List<BlockRows> _rows = new List<BlockRows>();
        private Dictionary<Vector2Int, PlantingBlock> _blocksMap = new Dictionary<Vector2Int, PlantingBlock>();
        
        public List<BlockRows> Rows => _rows;

        public void OnAfterInitialized()
        {
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            _minRounded = new Vector2(Mathf.Round(minPoint.position.x), Mathf.Round(minPoint.position.y));
            _maxRounded = new Vector2(Mathf.Round(maxPoint.position.x), Mathf.Round(maxPoint.position.y));
            var width = _maxRounded.x - _minRounded.x;
            var height = _maxRounded.y - _minRounded.y;
            Debug.Log($"Grid size: {width}x{height}");
            var startingPoint = _minRounded + offset;

            for (int y = 0; y <= height; y++)
            {
                var row = new BlockRows
                {
                    rowId = y
                };
                _rows.Add(row);
                
                for (int x = 0; x <= width; x++)
                {
                    CreateBlockAtPosition(startingPoint, x, y, row);
                }
            }
        }

        private void CreateBlockAtPosition(Vector2 startingPoint, int x, int y, BlockRows row)
        {
            var block = Instantiate(blockPrefab, blocksParent);
            block.transform.position = startingPoint + new Vector2(x, y);
            block.gameObject.name = $"Block_{x}_{y}";
            block.Init();
                    
            if (Physics2D.OverlapBox(block.transform.position, new Vector2(0.9f, 0.9f), 0,
                    blockingLayer))
            {
                block.PreventPlanting();
            }
                    
            row.blocks.Add(block);
            _blocksMap.Add(new Vector2Int(x, y), block);
        }

        public PlantingBlock GetBlock(Vector2Int position)
        {
            var worldX = Mathf.Abs(_minRounded.x) + position.x;
            var worldY = Mathf.Abs(_minRounded.y) + position.y;
            var blockPosition = new Vector2Int(Mathf.FloorToInt(worldX), Mathf.FloorToInt(worldY));
            return _blocksMap.GetValueOrDefault(blockPosition);
        }
    }
}