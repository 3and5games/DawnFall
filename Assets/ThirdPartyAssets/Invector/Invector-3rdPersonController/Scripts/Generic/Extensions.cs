using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Invector
{
    public static class Extensions
    {
        /// <summary>
        /// Normalizeds the angle. between -180 and 180 degrees
        /// </summary>
        /// <param Name="eulerAngle">Euler angle.</param>
        public static Vector3 NormalizeAngle(this Vector3 eulerAngle)
        {
            var delta = eulerAngle;

            if (delta.x > 180) delta.x -= 360;
            else if (delta.x < -180) delta.x += 360;

            if (delta.y > 180) delta.y -= 360;
            else if (delta.y < -180) delta.y += 360;

            if (delta.z > 180) delta.z -= 360;
            else if (delta.z < -180) delta.z += 360;

            return new Vector3((int)delta.x, (int)delta.y, (int)delta.z);//round values to angle;
        }

        public static Vector3 Difference(this Vector3 vector, Vector3 otherVector)
        {
            return otherVector - vector;
        }
        public static void SetActiveChildren(this GameObject gameObjet, bool value)
        {
            foreach (Transform child in gameObjet.transform)
                child.gameObject.SetActive(value);
        }

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            do
            {
                if (angle < -360)
                    angle += 360;
                if (angle > 360)
                    angle -= 360;
            } while (angle < -360 || angle > 360);

            return Mathf.Clamp(angle, min, max);
        }


        /// <summary>
        /// Lerp between CameraStates
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="time"></param>
        public static void Slerp(this TPCameraState to, TPCameraState from, float time)
        {
            to.Name = from.Name;
            to.forward = Mathf.Lerp(to.forward, from.forward, time);
            to.right = Mathf.Lerp(to.right, from.right, time);
            to.defaultDistance = Mathf.Lerp(to.defaultDistance, from.defaultDistance, time);
            to.maxDistance = Mathf.Lerp(to.maxDistance, from.maxDistance, time);
            to.minDistance = Mathf.Lerp(to.minDistance, from.minDistance, time);
            to.height = Mathf.Lerp(to.height, from.height, time);
            to.fixedAngle = Vector2.Lerp(to.fixedAngle, from.fixedAngle, time);            
            to.smoothFollow = Mathf.Lerp(to.smoothFollow, from.smoothFollow, time);
            to.yMinLimit = Mathf.Lerp(to.yMinLimit, from.yMinLimit, time);
            to.yMaxLimit = Mathf.Lerp(to.yMaxLimit, from.yMaxLimit, time);
            to.xMinLimit = Mathf.Lerp(to.xMinLimit, from.xMinLimit, time);
            to.xMaxLimit = Mathf.Lerp(to.xMaxLimit, from.xMaxLimit, time);
            to.cullingHeight = Mathf.Lerp(to.cullingHeight, from.cullingHeight, time);
            to.cullingMinDist = Mathf.Lerp(to.cullingMinDist, from.cullingMinDist, time);
            to.cameraMode = from.cameraMode;
            to.useZoom = from.useZoom;
            to.lookPoints = from.lookPoints;
        }
        /// <summary>
        /// Copy of CameraStates
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyState(this TPCameraState to, TPCameraState from)
        {
            to.Name = from.Name;
            to.forward = from.forward;
            to.right = from.right;
            to.defaultDistance = from.defaultDistance;
            to.maxDistance = from.maxDistance;
            to.minDistance = from.minDistance;
            to.height = from.height;
            to.fixedAngle = from.fixedAngle;
            to.lookPoints = from.lookPoints;            
            to.smoothFollow = from.smoothFollow;
            to.yMinLimit = from.yMinLimit;
            to.yMaxLimit = from.yMaxLimit;
            to.xMinLimit = from.xMinLimit;
            to.xMaxLimit = from.xMaxLimit;
            to.cullingHeight = from.cullingHeight;
            to.cullingMinDist = from.cullingMinDist;
            to.cameraMode = from.cameraMode;
            to.useZoom = from.useZoom;
            
        }


        public static ClipPlanePoints NearClipPlanePoints(this Camera camera, Vector3 pos, float clipPlaneMargin)
        {
            var clipPlanePoints = new ClipPlanePoints();

            var transform = camera.transform;
            var halfFOV = (camera.fieldOfView / 2) * Mathf.Deg2Rad;
            var aspect = camera.aspect;
            var distance = camera.nearClipPlane;
            var height = distance * Mathf.Tan(halfFOV);
            var width = height * aspect;
            height *= 1 + clipPlaneMargin;
            width *= 1 + clipPlaneMargin;
            clipPlanePoints.LowerRight = pos + transform.right * width;
            clipPlanePoints.LowerRight -= transform.up * height;
            clipPlanePoints.LowerRight += transform.forward * distance;

            clipPlanePoints.LowerLeft = pos - transform.right * width;
            clipPlanePoints.LowerLeft -= transform.up * height;
            clipPlanePoints.LowerLeft += transform.forward * distance;

            clipPlanePoints.UpperRight = pos + transform.right * width;
            clipPlanePoints.UpperRight += transform.up * height;
            clipPlanePoints.UpperRight += transform.forward * distance;

            clipPlanePoints.UpperLeft = pos - transform.right * width;
            clipPlanePoints.UpperLeft += transform.up * height;
            clipPlanePoints.UpperLeft += transform.forward * distance;

            return clipPlanePoints;
        }
    }

    public struct ClipPlanePoints
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }
}