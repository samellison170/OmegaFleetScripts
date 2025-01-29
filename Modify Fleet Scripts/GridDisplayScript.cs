using UnityEngine;

public class GridDisplay : MonoBehaviour
{
    public int gridSize = 10;        // Number of grid cells
    public float lineSpacing = 1.0f; // Space between lines
    public Material lineMaterial;    // Material for the grid lines

    // Define the starting position offset
    private Vector3 gridStartPosition = new Vector3(-100f, 0f, -100f);

    void Start()
    {
        // Draw vertical and horizontal lines of the grid
        for (int x = 0; x <= gridSize; x++)
        {
            // Vertical lines
            DrawLine(
                gridStartPosition + new Vector3(x * lineSpacing, 0, 0),
                gridStartPosition + new Vector3(x * lineSpacing, 0, gridSize * lineSpacing)
            );

            // Horizontal lines
            DrawLine(
                gridStartPosition + new Vector3(0, 0, x * lineSpacing),
                gridStartPosition + new Vector3(gridSize * lineSpacing, 0, x * lineSpacing)
            );
        }
    }

    void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject line = new GameObject("GridLine");
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = 0.25f;  // Set the line thickness
        lr.endWidth = 0.25f;
        lr.material = lineMaterial;
    }
}
