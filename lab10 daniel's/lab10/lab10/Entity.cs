using System.Linq;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace lab10
{
    public class Entity // abstract class to hold common variables/utillitys/variable return or calcs
    {
        public Vector3 position, velocity, acceleration;
        protected Vector3? targetPoint;
        public List<Vector3> wayPoints;
        protected Entity targetEntity;
        float mass = 10;//scale model based on mass??
        protected float maxSpeed = 2;
        protected float speed = 0.5f;
        float maxTurn = 1f;
        float slowRadius = 32;
        float stopRadius = 8;
        enum aiStates {idle, seeking, arriving, fleeing, tracking, stop};
        aiStates state;
        public Entity()
        {
            wayPoints = new List<Vector3>();
        }
        public void applyForce(Vector3 force)
        {
            acceleration += force/mass;
        }
        public void arrive(Vector3 target)
        {
            float dist = Vector3.Distance(target, position);
            if(dist < slowRadius)
            {
                speed = speed * (dist / slowRadius);//this needs to be done better
            }
            else
            {
                speed = maxSpeed;
            }
            if(dist < stopRadius)
            {
                state = aiStates.stop;
                targetPoint = null;
                velocity = Vector3.Zero;
            }
            seek(target);
        }
        
        public void seek(Vector3 target) 
        {
            Vector3 desired = target - position;
            desired.Normalize();
            desired *= speed;
            Vector3 steer = desired - velocity;
            steer = Vector3.Clamp(steer, new Vector3(-maxTurn, -maxTurn, -maxTurn), new Vector3(maxTurn, maxTurn, maxTurn));
            applyForce(steer);
        }
        public void tracking (Entity target)
        {
            float dist = Vector3.Distance(target.position, position);
            if (dist < slowRadius)
            {
                speed = speed * (dist / slowRadius);//this needs to be done better
            }
            else
            {
                speed = maxSpeed;
            }
            if (dist < stopRadius)
            {
                state = aiStates.stop;
                targetEntity = null;
                velocity = Vector3.Zero;
            }
            Vector3 desired = target.position - position;//predict the future position
            desired.Normalize();
            desired *= speed;
            Vector3 steer = desired - velocity;
            steer = Vector3.Clamp(steer, new Vector3(-maxTurn, -maxTurn, -maxTurn), new Vector3(maxTurn, maxTurn, maxTurn));
            applyForce(steer);
        }
        public void addWayPoint(List<Vector3> points)
        {
            wayPoints = points;
        }
        public void addTargetEntity(Entity target)
        {
            targetEntity = target;
        }
        public void Update()
        {
            /*switch (state)
            {
                case aiStates.idle:
                    if(targetPoint != null) state = aiStates.arriving;
                    if (targetEntity != null) state = aiStates.tracking;
                    break;
                case aiStates.seeking:
                    seek((Vector3)targetPoint);
                    break;
                case aiStates.arriving:
                    arrive((Vector3)targetPoint);
                    break;
                case aiStates.fleeing:
                    break;
                case aiStates.tracking:
                    tracking(targetEntity);
                    break;
                default:
                    state = aiStates.idle;
                    speed = maxSpeed;
                    break;
            }
            */
            //if (state != aiStates.stop)
            //{
            if (wayPoints.Count > 0)
            {
                seek(wayPoints[0]);
                float dist = Vector3.Distance(wayPoints[0], position);
                if (dist < speed)
                {
                    //position = wayPoints[0];
                    wayPoints.RemoveAt(0);
                }
            }
            else
            {
                velocity *= 0;
                acceleration *= 0;
            }
            velocity += acceleration;
            velocity = Vector3.Clamp(velocity, new Vector3(-maxSpeed, -maxSpeed, -maxSpeed), new Vector3(maxSpeed, maxSpeed, maxSpeed));
            position += velocity;
            acceleration *= 0;
            //}
        }
    }
}
