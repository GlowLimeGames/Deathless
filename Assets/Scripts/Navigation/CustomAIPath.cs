using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding.Util;
using Pathfinding;

public class CustomAIPath : AIPath {
    private const float CONSTANT_SPEED_MODIFIER = 30;

    /** Whether the object should rotate towards the direction it moves.*/
    public bool enableRotation;

    public bool enableAcceleration;

    private Vector2 direction;

    public bool isMoving {
        get {
            return Mathf.Abs(velocity2D.x) > 0 || Mathf.Abs(velocity2D.y) > 0;
        }
    }

    /** A modified version of AIPath.Movement Update which allows movement without rotation.*/
    protected override void MovementUpdate(float deltaTime) {
        if (!canMove) return;

        if (!interpolator.valid) {
            velocity2D = Vector3.zero;
        }
        else {
            var currentPosition = tr.position;

            interpolator.MoveToLocallyClosestPoint(currentPosition, true, false);
            interpolator.MoveToCircleIntersection2D(currentPosition, pickNextWaypointDist, movementPlane);
            targetPoint = interpolator.position;
            direction = movementPlane.ToPlane(targetPoint - currentPosition);

            var distanceToEnd = direction.magnitude + interpolator.remainingDistance;
            // How fast to move depending on the distance to the target.
            // Move slower as the character gets closer to the target.
            float slowdown = slowdownDistance > 0 ? distanceToEnd / slowdownDistance : 1;

            // a = v/t, should probably expose as a variable
            float acceleration = speed / 0.4f;
            Vector2 forward = enableRotation ? (Vector2)(rotationIn2D ? tr.up : tr.forward) : direction;

            if (enableAcceleration) {
                velocity2D += MovementUtilities.CalculateAccelerationToReachPoint(direction, direction.normalized * speed, velocity2D, acceleration, speed) * deltaTime;
            }
            else {
                velocity2D = direction.normalized * speed * CONSTANT_SPEED_MODIFIER * deltaTime;
            }
            velocity2D = MovementUtilities.ClampVelocity(velocity2D, speed, slowdown, enableRotation, movementPlane.ToPlane(forward));

            ApplyGravity(deltaTime);

            if (distanceToEnd <= endReachedDistance && !TargetReached) {
                TargetReached = true;
                OnTargetReached();
            }

            // Rotate towards the direction we are moving in
            if (enableRotation) {
                var currentRotationSpeed = rotationSpeed * Mathf.Clamp01((Mathf.Sqrt(slowdown) - 0.3f) / 0.7f);
                RotateTowards(velocity2D, currentRotationSpeed * deltaTime);
            }

            var delta2D = CalculateDeltaToMoveThisFrame(movementPlane.ToPlane(currentPosition), distanceToEnd, deltaTime);
            Move(currentPosition, movementPlane.ToWorld(delta2D, verticalVelocity * deltaTime));

            velocity = movementPlane.ToWorld(velocity2D, verticalVelocity);
        }
    }

    /** Called when the AI has successfully reached its target. */
    public override void OnTargetReached() {
        GetComponent<WorldItem>().OnTargetReached(target);
        StopImmediately();
    }

    public CardinalDirection GetDirection(bool allowNS = true) {
        CardinalDirection cardinalDir;

        if (allowNS && Mathf.Abs(direction.y) > (Mathf.Abs(direction.x) * 2)) {
            if (direction.y > 0) { cardinalDir = CardinalDirection.NORTH; }
            else { cardinalDir = CardinalDirection.SOUTH; }
        }
        else {
            if (direction.x > 0f) { cardinalDir = CardinalDirection.EAST; }
            else { cardinalDir = CardinalDirection.WEST; }
        }

        return cardinalDir;
    }

    public void StopImmediately() {
        velocity2D = Vector2.zero;
        canMove = false;
    }
}