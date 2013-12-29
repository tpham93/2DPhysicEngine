using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysicEngine.Object.Shape;
using PhysicEngine.Objects;

namespace PhysicEngine.ObjectData.Objects
{
    class MassData
    {
        private float mass;
        private float iMass;

        private float inertia;
        private float iInertia;

        public float Mass
        {
            get { return mass; }
            set
            {
                if (float.IsInfinity(value))
                {
                    mass = 0;
                    iMass = 0;
                }
                else
                {
                    mass = value;
                    iMass = 1.0f / mass;
                }
            }
        }
        public float IMass
        {
            get { return iMass; }
        }

        public float Inertia
        {
            get { return mass; }
            set
            {
                inertia = value;
                iInertia = 1.0f / mass;
            }
        }
        public float IInertia
        {
            get { return iInertia; }
        }

        public MassData(float mass)
        {
            this.Mass = mass / (PhysicalConstants.PixelToMeter * PhysicalConstants.PixelToMeter);
        }
        public MassData(Shape2D shape, MaterialData material)
            : this(shape, material.Density)
        {
        }
        public MassData(Shape2D shape, float density)
            : this(shape.Area * density)
        {
        }



    }
}
