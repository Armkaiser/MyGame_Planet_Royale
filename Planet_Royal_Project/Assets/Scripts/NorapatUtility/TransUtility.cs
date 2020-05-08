using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorapatUtility
{
    /// This class is made for transform operation.

    public static class TransUtility
    {
        /// <summary>
        /// Smooth rotate to a direction.
        /// </summary>
        /// <param name="trans">Rotate object.</param>
        /// <param name="dir">Direction.</param>
        /// <param name="upward">Define direction up.</param>
        /// <param name="rotSpeed">Speed of rotation.</param>
        /// <param name="offsetRot">Offset rotation.</param>
        public static void RotateSmoothToDir(Transform trans, Vector3 dir, Vector3 upward, float rotSpeed, Quaternion offsetRot)
        {
            dir.Normalize();
            Quaternion lookRot = Quaternion.LookRotation(dir, upward);
            lookRot *= offsetRot;
            Quaternion targetRot = Quaternion.Slerp(trans.rotation, lookRot, rotSpeed);
            trans.rotation = targetRot;
        }

        /// <summary>
        /// Smooth rotate to a direction.
        /// </summary>
        /// <param name="trans">Rotate object.</param>
        /// <param name="dir">Direction.</param>
        /// <param name="upward">Define direction up.</param>
        /// <param name="rotSpeed">Speed of rotation.</param>
        public static void RotateSmoothToDir(Transform trans, Vector3 dir, Vector3 upward, float rotSpeed)
        {
            dir.Normalize();
            Quaternion lookRot = Quaternion.LookRotation(dir, upward);
            Quaternion targetRot = Quaternion.Slerp(trans.rotation, lookRot, rotSpeed);
            trans.rotation = targetRot;
        }

        /// <summary>
        /// RotateToward to a direction.
        /// </summary>
        /// <param name="trans">Rotate object.</param>
        /// <param name="dir">Direction.</param>
        /// <param name="upward">Define direction up.</param>
        /// <param name="rotSpeed">Speed of rotation.</param>
        public static void RotateTowardToDir(Transform trans, Vector3 dir, Vector3 upward, float rotSpeed)
        {
            dir.Normalize();
            Quaternion lookRot = Quaternion.LookRotation(dir, upward);
            Quaternion targetRot = Quaternion.RotateTowards(trans.rotation, lookRot, rotSpeed);
            trans.rotation = targetRot;
        }

        /// <summary>
        /// RotateToward to a direction.
        /// </summary>
        /// <param name="trans">Rotate object.</param>
        /// <param name="dir">Direction.</param>
        /// <param name="upward">Define direction up.</param>
        /// <param name="rotSpeed">Speed of rotation.</param>
        public static void RotateTowardToDir(Transform trans, Vector3 dir, Vector3 upward, float rotSpeed, Quaternion offsetRot)
        {
            dir.Normalize();
            Quaternion lookRot = Quaternion.LookRotation(dir, upward);
            lookRot *= offsetRot;
            Quaternion targetRot = Quaternion.RotateTowards(trans.rotation, lookRot, rotSpeed);
            trans.rotation = targetRot;
        }

        /// <summary>
        /// Get mouse direction by the ray that hit on a object.
        /// </summary>
        /// <param name="originTrans">Origin object.</param>
        /// <param name="maxDist">Max distance.</param>
        /// <param name="layer">Layer able to hit.</param>
        /// <param name="hitInfo">Hit's info.</param>
        /// <returns>Direction of origin transform to mouse position.</returns>
        public static Vector3 GetMouseDirOnWorld(Transform originTrans, float maxDist, out RaycastHit hitInfo, LayerMask layer)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, maxDist, layer))
            {
                Vector3 dirOfOriginToMouse = hitInfo.point - originTrans.position;

                return dirOfOriginToMouse;
            }

            return originTrans.position;
        }

        /// <summary>
        /// Get mouse direction, relative with screen position.
        /// </summary>
        /// <param name="startPos">Start position from world position.</param>
        /// <returns>direction.</returns>
        /// @warning This function only use for 3D-SideScroll or 2D games. Cause screen only have x and y position.
        public static Vector3 GetMouseDirOnScreen(Vector3 startPos)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(startPos);
            Vector3 dir = Input.mousePosition - screenPos;

            dir.z = startPos.z;

            return dir;
        }

        /// <summary>
        /// Check is object outside the screen.
        /// </summary>
        /// <param name="targetPos">Target on world position.</param>
        /// <returns>Is this object outside the screen.</returns>
        public static bool IsObjOffScreen(Vector3 targetPos, float offsetBorder)
        {
            Vector3 targetOnScreenPos = Camera.main.WorldToScreenPoint(targetPos);

            return targetOnScreenPos.x <= offsetBorder || targetOnScreenPos.x >= Screen.width - offsetBorder||
                targetOnScreenPos.y <= offsetBorder || targetOnScreenPos.y >= Screen.height - offsetBorder;
        }
    }
}