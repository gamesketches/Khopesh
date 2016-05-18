using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class VectorGraphics : MonoBehaviour {

    [SerializeField]
    List<Vector3> points;
    public virtual List<Vector3> Points
    {
        get { return points; }
        set { points = value; }
    }

	[SerializeField] bool connect = true;

	[SerializeField] float width = 1.0f;
	public float Width {
		get { return width; }
		set { 
			width = value;
			if (Line != null) {
				Line.lineWidth = width;
			}
		}
	}

    [SerializeField]
	Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public virtual Color Color
    {
        get
        {
            return color;
        }
        set
        {
            color = value;
            if (Line != null)
            {
                Line.color = color;
            }
        }
    }

    //[SerializeField]
    VectorLine line = null;
    public VectorLine Line
    {
        get { return line; }
        protected set { line = value; }
    }

	[SerializeField] LineType lineType = LineType.Continuous;
	[SerializeField] Joins joins = Joins.Weld;

	[SerializeField] Material material;
	public Material Material {
		get { 
			if (line == null) {
				return material; 
			} else {
				return line.material;
			}
		}
	}

//    PolygonCollider2D m_polygonCollider;
//    PolygonCollider2D polygonCollider
//    {
//        get
//        {
//            if (m_polygonCollider == null)
//            {
//                m_polygonCollider = GetComponent<PolygonCollider2D>();
//            }
//            return m_polygonCollider;
//        }
//    }
    
    void Awake ()
    {
//        if (polygonCollider != null)
//        {
//            var colliderPoints = new List<Vector2>();
//            foreach (Vector3 point in Points)
//            {
//                colliderPoints.Add(new Vector2(point.x, point.y));
//            }
//            polygonCollider.SetPath(0, colliderPoints.ToArray());
//        }

		if (connect) {
			Points.Add(Points[0]);
		}

		Line = new VectorLine("Graphics " + name, Points, null, Width, lineType, joins);
		if (material != null) {
			Line.material = new Material(material);
		}
        Line.drawTransform = transform;
        Line.color = Color;

        VectorLine.SetCanvasCamera(Camera.main);
    }

    void OnDestroy()
    {
        if (Line != null)
        {
            VectorLine.Destroy(ref line);
        }
    }
	
	// Update is called once per frame
	void Update () {
        Line.Draw();
    }

	void OnDrawGizmos() {
		var prevColor = Gizmos.color;
		Gizmos.color = Color;

		if (Points != null) {
			int increment = (lineType == LineType.Continuous) ? 1 : 2;
			for (int i = 0; i < (connect ? Points.Count : Points.Count - 1); i += increment) {
				Vector3 first = transform.TransformPoint(Points[i]);
				Vector3 second = transform.TransformPoint(Points[(i + 1) % Points.Count]);
				Gizmos.DrawLine(first, second);
			}
		}

		Gizmos.color = prevColor;
	}
}
