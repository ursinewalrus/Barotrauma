﻿using FarseerPhysics;
using Microsoft.Xna.Framework;
using System.Xml.Linq;

namespace Barotrauma
{
    class AnimController : Ragdoll
    {
        public enum Animation { None, Climbing, Jumping, UsingConstruction, Struggle, CPR };
        public Animation Anim;

        public LimbType GrabLimb;

        protected Character character;

        protected float walkSpeed, swimSpeed;
        
        protected float walkPos;

        protected readonly Vector2 stepSize;
        protected readonly float legTorque;

        public float RunSpeedMultiplier
        {
            get;
            private set;
        }

        public float SwimSpeedMultiplier
        {
            get;
            private set;
        }
        
        public Vector2 AimSourcePos
        {
            get { return ConvertUnits.ToDisplayUnits(AimSourceSimPos); }
        }

        public virtual Vector2 AimSourceSimPos
        {
            get
            {
                return Collider.SimPosition;
            }
        }

        public AnimController(Character character, XElement element)
            : base(character, element)
        {
            this.character = character;

            stepSize = element.GetAttributeVector2("stepsize", Vector2.One);
            stepSize = ConvertUnits.ToSimUnits(stepSize);

            walkSpeed = element.GetAttributeFloat("walkspeed", 1.0f);
            swimSpeed = element.GetAttributeFloat("swimspeed", 1.0f);

            RunSpeedMultiplier = element.GetAttributeFloat("runspeedmultiplier", 2f);
            SwimSpeedMultiplier = element.GetAttributeFloat("swimspeedmultiplier", 1.5f);
            
            legTorque = element.GetAttributeFloat("legtorque", 0.0f);
        }

        public virtual void UpdateAnim(float deltaTime) { }

        public virtual void HoldItem(float deltaTime, Item item, Vector2[] handlePos, Vector2 holdPos, Vector2 aimPos, bool aim, float holdAngle) { }

        public virtual void DragCharacter(Character target) { }

        public virtual void UpdateUseItem(bool allowMovement, Vector2 handPos) { }

   }
}
