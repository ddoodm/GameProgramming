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

namespace lab4._1
{
    public class Entity // abstract class to hold common variables/utillitys/variable return or calcs
    {
        protected Vector3 position, velocity, acceleration;
        protected Vector3? targetPoint;
        protected Entity targetEntity;
        float mass = 10;//scale model based on mass??
        protected float maxSpeed = 2;
        protected float speed = 2;
        float maxTurn = 0.5f;
        float slowRadius = 32;
        float stopRadius = 8;
        enum aiStates {idle, seeking, arriving, fleeing, tracking, stop};
        aiStates state;
        public Entity()
        {

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
        public void addWayPoint(Vector3 point)
        {
            targetPoint = point;
        }
        public void addTargetEntity(Entity target)
        {
            targetEntity = target;
        }
        public void Update()
        {
            switch (state)
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

            //if (state != aiStates.stop)
            //{
                velocity += acceleration;
                velocity = Vector3.Clamp(velocity, new Vector3(-maxSpeed, -maxSpeed, -maxSpeed), new Vector3(maxSpeed, maxSpeed, maxSpeed));
                position += velocity;
                acceleration *= 0;
            //}
        }
    }
}
