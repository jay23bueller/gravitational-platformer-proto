using System;
using UnityEngine;

public interface IPullable
{
    public void PullTowardsTransform(Transform t, float gravitySpeed, GravityVolume.VolumeShape volumeShape);
}

public class GravityVolume : MonoBehaviour
{
    public enum VolumeShape
    {
        Cube,
        Sphere,
        Capsule
    }

    [SerializeField]
    private VolumeShape _shape;
    [SerializeField]
    private float _pullForce;

    private void OnTriggerEnter(Collider other)
    {
        if(other != null && other.GetComponent<IPullable>() != null)
        {
            other.GetComponent<IPullable>().PullTowardsTransform(transform, _pullForce, _shape);
        }
    }

    private void OnDrawGizmos()
    {
        Color transparentBlue = Color.blue;
        transparentBlue.a = .15f;
        Gizmos.color = transparentBlue;
        switch(_shape)
        {
            case VolumeShape.Cube:
                Gizmos.DrawCube(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.extents * 2f);
                break;
            case VolumeShape.Sphere:
                Gizmos.DrawSphere(GetComponent<Collider>().bounds.center, GetComponent<SphereCollider>().radius);
                break;
        }
    }



}
