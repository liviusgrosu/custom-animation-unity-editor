using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrePhysicalState : MonoBehaviour
{
    public List<GameObject> StateMeshes;
    private GameObject _currentMeshModel;
    private int _currentStateIdx;
    private MeshFilter _mesh;
    private MeshCollider _collider;

    public float IFrameTime = 0.5f;
    private float _currentIFrameTime;
    private bool _invincible;
    private void Awake() {
        _mesh = GetComponent<MeshFilter>();
        _collider = GetComponent<MeshCollider>();
        ChangeModel(0);
    }

    private void Update() {
        if(_invincible) {
            _currentIFrameTime += Time.deltaTime;
            if (_currentIFrameTime >= IFrameTime) {
                _invincible = false;
            }
        }
    }

    public void CrushOre() {
        if (_invincible) {
            return;
        }

        if (_currentStateIdx < StateMeshes.Count - 1) {
            _currentStateIdx++;
            ChangeModel(_currentStateIdx);
        }
    }

    private void ChangeModel(int idx) {
        //Change the mesh
        _mesh.mesh = StateMeshes[idx].GetComponent<MeshFilter>().sharedMesh;
        _collider.sharedMesh = _mesh.mesh;
        // Change to prefabs scale and rotation
        _currentMeshModel = StateMeshes[idx];
        transform.rotation = StateMeshes[idx].transform.rotation;

        _invincible = true;
        _currentIFrameTime = 0f;
    }
}
