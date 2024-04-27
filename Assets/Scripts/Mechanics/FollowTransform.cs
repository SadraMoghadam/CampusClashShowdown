using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour {


    private Transform _targetTransform;
    private bool _isFollowing;


    public void SetTargetTransform(Transform targetTransform) {
        this._targetTransform = targetTransform;
        _isFollowing = true;
    }
    
    
    public void SetIsFollowing(bool isFollowing)
    {
        _isFollowing = isFollowing;
    }

    private void LateUpdate() {
        if (_targetTransform == null || !_isFollowing) {
            return;
        }

        transform.position = _targetTransform.position;
        transform.rotation = _targetTransform.rotation;
    }

}