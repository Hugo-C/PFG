using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour {
    public GameObject boardGo;

    [Range(0f, 0.5f)]
    public float canSeeMargin;
    
    [Range(1f, 100f)]
    public float speed;


    private Transform center;
    private List<Transform> corners;
    private Camera cameraComponent;

    private void Start() {
        Board board = boardGo.GetComponent<Board>();
        center = board.GetCenterTransform();
        corners = board.GetCornersTransforms();
        cameraComponent = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update() {
        transform.LookAt(center);
        transform.Translate(Vector3.right * Time.deltaTime * speed);
        bool canSeeItAll = true;
        foreach (var corner in corners) {
            if (!CanSee(corner)) {
                canSeeItAll = false;
                break;
            }
        }

        if (!canSeeItAll)
            cameraComponent.orthographicSize += 0.1f;
    }

    /// <summary>
    /// Indicates if the camera can see the given transform
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public bool CanSee(Transform t) {
        Vector3 viewportPoint = cameraComponent.WorldToViewportPoint(t.position);
        return viewportPoint.x - canSeeMargin > 0 && viewportPoint.x + canSeeMargin < 1 && 
               viewportPoint.y - canSeeMargin > 0 && viewportPoint.y + canSeeMargin < 1;
    }
}